using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Sourcecontrol.Telelogic
{
	/// <summary>
	///     CruiseControl.NET SCM plugin for CM Synergy.
	/// </summary>
	/// <remarks>
	///     Tested against CM Synergy 6.3.  Supports integration testing of tasks.  Can use
	///     baselines and/or a shared task folder to publish successfully integrated tasks.
	///     <para />
	///     <notes type="implementnotes">
	///         This type does not subclass <see cref="ProcessSourceControl"/> because
	///         the <see cref="SynergyParser"/> cannot be instantiated without having
	///         the initialized/configured values for the Synergy project specification.
	///     </notes>
	/// </remarks>
	public class SynergyCommand : ISynergyCommand
	{
		/// <summary>
		///     Specifies the remote function call (RFC) address (host:socket) for the CM Synergy engine.
		/// </summary>
		/// <remarks>
		///     See <see href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=defaults81.html#wp997768" />
		///     for details on environment variables used by CM Synergy.
		/// </remarks>
		public const string SessionToken = "CCM_ADDR";

		/// <summary>
		///     Specifies use of the invariant culture for date/time formating and parsing.
		/// </summary>
		/// <remarks>
		///     See <see href="https://support.telelogic.com/en/synergy/docs/docs_63/help_w/wwhelp/wwhimpl/common/html/wwhelp.htm?context=cmsynergy%26file=defaults81.html#wp997768" />
		///     for details on environment variables used by CM Synergy.
		/// </remarks>
		public const string DateTimeFormat = "CCM_NO_LOCALE_TIMES";

		/// <summary>Object used to serialize calls to <see cref="Open"/>.</summary>
		/// <remarks>
		///     The CM Synergy client does not robustly handle concurrent calls to <c>ccm.exe start</c>.
		///     Concurrent calls may cause read/write contention for the <c>ccm_ui.log</c> or other 
		///     similar files, as the clients seem to lock these files during start.
		///     A common startup error message from ccm.exe is "Could not write preferences" 
		///     during the startup.
		/// </remarks>
		private static readonly object PadLock;

		/// <summary>The CCNET process launcher.</summary>
		private ProcessExecutor executor;

		/// <summary>The configured settings for the Synergy server connection.</summary>
		private SynergyConnectionInfo connection;

		/// <summary>The configured settings for the Synergy integration project.</summary>
		private SynergyProjectInfo project;

		/// <summary>Track whether Dispose has been called.</summary>
		private bool disposed;

		/// <summary>Track whether we have an active connection.</summary>
		private bool isOpen;

		/// <summary>
		///     Default constructor.  Initializes all members to their default values.
		/// </summary>
		public SynergyCommand(SynergyConnectionInfo connectionInfo, SynergyProjectInfo projectInfo)
		{
			/*disposed = false*/;
			/*isOpen = false*/;
			executor = new ProcessExecutor();
			connection = connectionInfo;
			project = projectInfo;

			// register for server shutdown, to close all connections
			AppDomain.CurrentDomain.DomainUnload += new EventHandler(AppDomain_Unload);
			AppDomain.CurrentDomain.ProcessExit += new EventHandler(AppDomain_Unload);
		}

		/// <summary>
		///     Type constructor used to the initialize and assign an object reference to
		///     <see cref="PadLock"/>.
		/// </summary>
		static SynergyCommand()
		{
			PadLock = new object();
		}

		/// <summary>
		///     Finalizer that ensures that Synergy connections are eventually closed.
		/// </summary>
		~SynergyCommand()
		{
			Dispose();
		}

		/// <summary>
		///     Event handler for <see cref="AppDomain.DomainUnload"/> and <see cref="AppDomain.ProcessExit"/>.
		///     Ensures that Synergy connections are eventually closed.
		/// </summary>
		public void AppDomain_Unload(object sender, EventArgs e)
		{
			Close();
		}

		/// <overloads>
		///     <summary>
		///         Ensures that the Synergy session has been <see cref="Close">Closed</see>.
		///     </summary>
		///     <remarks>
		///         Based on the implementation suggested by 
		///         <see href="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfSystemIDisposableClassTopic.asp">MSDN</see>
		///     </remarks>
		/// </overloads>
		public void Dispose()
		{
			Close();

			// Check to see if Dispose has already been called.
			// If disposing equals true, close the Synergy session.
			if (! disposed)
			{
				// The Synergy session will be cleaned up by the Dispose method,
				// so we can take this instance off the finalization queue.
				GC.SuppressFinalize(this);
				disposed = true;
			}
		}

		/// <summary>
		///     Starts a new Synergy session.
		/// </summary>
		/// <exception cref="CruiseControlException">
		///     Thrown if <see cref="SynergyCommandBuilder.Start"/> fails to write 
		///     a single line containing the CCM_ADDR value.
		/// </exception>
		private void Open()
		{
			ProcessInfo info;
			ProcessResult result;
			int originalTimeout;
			string temp;
			string message;

			if (! isOpen)
			{
				info = SynergyCommandBuilder.Start(connection, project);

				/* Serialize the calls to <c>ccm.exe start</c>, as the CM Synergy
                 * client does not properly support concurrent calls to this command for the
                 * same CM Synergy username. 
                 * 
                 * TODO: Should we optimize this to have shared locks only for the same CM Synergy
                 *       username?  It's doutbful that large projects would have different credentials,
                 *       so there may be little gain for this functionality */
				Log.Debug("Queued for critical section to open CM Synergy session; blocking until lock is acquired.");
				lock (PadLock)
				{
					Log.Debug("Acquired lock to open a session");
					/* Don't call this.Execute(), as it will cause an infinite loop 
                     * once this.ValidateSession is called. */
					result = executor.Execute(info);
					Log.Debug("Releasing lock to open a session");
				}

				if (result.TimedOut)
				{
					message = String.Format(CultureInfo.CurrentCulture, @"Synergy connection timed out after {0} seconds.", connection.Timeout);
					throw(new CruiseControlException(message));
				}

				// suspend the thread if the database is protected, and the sleep option is enabled
				if (result.Failed)
				{
					if (connection.PollingEnabled)
					{
						if (IsDatabaseProtected(result.StandardError, connection.Host, connection.Database))
						{
							// sleep the thread for timeout until the database is unprotected
							Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Database {0} on Host {1} Is Protected.  Waiting 60 seconds to reconnect.", connection.Host, connection.Database));
							Thread.Sleep(new TimeSpan(0, 1, 0));

							// save the original timeout
							originalTimeout = connection.Timeout;

							// decrement the time to wait for the backup to complete, so that we don't wait forever
							connection.Timeout -= 60;

							//recursively call the open function
							Open();

							// revent the timeout prior to recursion
							connection.Timeout = originalTimeout;

							// exit the call stack, breaking out of the recursive loop
							return;
						}
					}
				}

				temp = result.StandardOutput;

				if (null != temp && temp.Length > 0)
				{
					connection.SessionId = temp.Trim();
					Log.Info(String.Concat("CCM_ADDR set to '", connection.SessionId, "'"));
				}
				else
				{
					throw(new CruiseControlException("CM Synergy logon failed"));
				}

				// read the delimiter fields
				Initialize();

				// update the release setting of the project and all subprojects
				info = SynergyCommandBuilder.GetSubProjects(connection, project);
				info.EnvironmentVariables[SessionToken] = connection.SessionId;
				executor.Execute(info);
				info = SynergyCommandBuilder.SetProjectRelease(connection, project);
				info.EnvironmentVariables[SessionToken] = connection.SessionId;
				executor.Execute(info);

				isOpen = true;
			}
		}

		/// <summary>
		///     Stops the Synergy session, if one was previously opened.
		/// </summary>
		private void Close()
		{
			ProcessInfo info;

			if (isOpen)
            {
				info = SynergyCommandBuilder.Stop(connection);
                
                // set the session id for ccm.exe to use
                info.EnvironmentVariables[SessionToken] = connection.SessionId;
                //Make sure the thread has a name so the ProcessExecuter will not crash
                if ( string.IsNullOrEmpty( Thread.CurrentThread.Name ) )
                {
                    Thread.CurrentThread.Name = connection.SessionId;
                }

				/* This should be a fire-and-forget call.
                 * We don't want an exception thrown if the session cannot be stopped. */
				/* don't call this.Execute(), as it will cause an infinite loop 
                 * once this.ValidateSession is called. */
				executor.Execute(info);

				// reset the CCM_ADDR value and delimiter fields
				connection.Reset();
			}

			// reset the open flag
			isOpen = false;
		}

		/// <summary>
		///     Initializes the <see cref="SynergyConnectionInfo.Delimiter"/> and 
		///     <see cref="SynergyProjectInfo.ObjectName"/>, and 
		///     the <see cref="SynergyProjectInfo.WorkAreaPath"/> fields.
		/// </summary>
		/// <exception cref="CruiseControlException">
		///     If the <see cref="SynergyCommandBuilder.GetDelimiter"/> or 
		///     <see cref="SynergyCommandBuilder.GetDcmDelimiter"/> commands fail 
		///     to return a stdout stream.  Also called if the work area is an invalid
		///     path, which can happen if the project has not been synchronized on
		///     the client machine.
		/// </exception>
		private void Initialize()
		{
			ProcessInfo info;
			ProcessResult result;
			string temp;

			try
			{
				// set the delimiter for the database
				info = SynergyCommandBuilder.GetDelimiter(connection);
				info.EnvironmentVariables[SessionToken] = connection.SessionId;
				result = Execute(info);
				temp = result.StandardOutput;
				if (temp.Length == 0)
					throw(new CruiseControlException("Failed to read the CM Synergy delimiter"));
				connection.Delimiter = temp[0];
			}
			catch (Exception inner)
			{
				throw(new CruiseControlException("Failed to read the CM Synergy database delimiter", inner));
			}

			try
			{
				// set the project spec
				info = SynergyCommandBuilder.GetProjectFullName(connection, project);
				info.EnvironmentVariables[SessionToken] = connection.SessionId;
				result = Execute(info);
				temp = result.StandardOutput.Trim();
				project.ObjectName = temp;
			}
			catch (Exception inner)
			{
				temp = String.Concat(@"CM Synergy Project """, project.ProjectSpecification, @""" not found");
				throw(new CruiseControlException(temp, inner));
			}

			try
			{
				// read the project work area path, to use for the working directory for ccm commands
				info = SynergyCommandBuilder.GetWorkArea(connection, project);
				info.EnvironmentVariables[SessionToken] = connection.SessionId;
				result = Execute(info);

				project.WorkAreaPath = Path.GetFullPath(result.StandardOutput.Trim());
				if (! Directory.Exists(project.WorkAreaPath))
				{
					throw(new CruiseControlException(String.Concat("CM Synergy work area '", result.StandardOutput.Trim(), "' not found.")));
				}

				Log.Info(String.Concat(project.ProjectSpecification, " work area is '", project.WorkAreaPath, "'"));
			}
			catch (Exception inner)
			{
				temp = String.Concat(@"CM Synergy Work Area for Project """, project.ProjectSpecification, @""" could not be determined.");
				throw(new CruiseControlException(temp, inner));
			}
		}

		/// <summary>
		///    Guarantees that a Synergy session is open, alive, and usable.
		/// </summary>
		private void ValidateSession()
		{
			// check that we have a CCM Address
			bool isValid = (null != connection.SessionId && connection.SessionId.Length > 0);

			// if the connection is open, execute the heartbeat command
			// to ensure that the server connection was not lost
			if (isOpen && isValid)
			{
				ProcessInfo info = SynergyCommandBuilder.Heartbeat(connection);
				if (null != project && null != project.WorkAreaPath && project.WorkAreaPath.Length > 0)
				{
					info = new ProcessInfo(info.FileName, info.Arguments, project.WorkAreaPath);
				}

				// set the session id for ccm.exe to use
				if (null != connection && null != connection.SessionId && connection.SessionId.Length > 0)
				{
					info.EnvironmentVariables[SessionToken] = connection.SessionId;
				}

				/* don't call this.Execute(), as it will cause an infinite loop 
                 * once this.ValidateSession is called. */
				ProcessResult result = executor.Execute(info);

				// reset the valid flag, if ccm status does not report the session token
				isValid = IsSessionAlive(result.StandardOutput, connection.SessionId, connection.Database);

				if (! isValid)
				{
					// Call the close method, since there's likely a problem with the client/server
					// connection.  This call will not throw an exception even if it fails.
					Close();
				}
			}

			// (re-)open the session if one could not be found, or if it was killed
			if (! isValid)
			{
				// Now try to re-establish a connection
				Open();
			}
		}

		/// <summary>
		///     Used to check if the current client session still has an open and active
		///     connection with the server.
		/// </summary>
		/// <remarks>
		///     This method has public accessibility so that it can be unit tested.
		///     Also, the session id and database values are sent as parameters, rather
		///     than read directly from the private fields for the same reason.
		/// </remarks>
		/// <param name="status">The output from the <c>ccm status</c> command.</param>
		/// <param name="sessionId">The value of <c>_Connection.SessionId</c></param>
		/// <param name="database">The value of <c>_Connection.Database</c></param>
		/// <returns></returns>
		public bool IsSessionAlive(string status, string sessionId, string database)
		{
			Regex grep;
			const string template = @"(?im:(@\s+{0}[\s\S]*Database:\s+{1}))";
			string pattern;

			pattern = String.Format(CultureInfo.CurrentCulture, template, Regex.Escape(sessionId), Regex.Escape(database));
			grep = new Regex(pattern, RegexOptions.CultureInvariant);

			return (grep.IsMatch(status));
		}

		/// <summary>
		///     Used to check if a session cannot be started because the database is
		///     in protected state.
		/// </summary>
		/// <remarks>
		///     This method can be used to spin wait the integration thread during a
		///     CM Synergy backup.  For long running builds
		/// </remarks>
		/// <param name="status">The output from the <c>ccm status</c> command.</param>
		/// <param name="host">The value of <c>_Connection.Host</c></param>
		/// <param name="database">The value of <c>_Connection.Database</c></param>
		/// <returns>
		///     <see langword="true" /> if the current session is still connected to the
		///     server.  <see langword="false" /> otherwise.
		/// </returns>
		public bool IsDatabaseProtected(string status, string host, string database)
		{
			Regex grep;
			const string template = @"(?im-x:(Warning: Database {0} on host {1} is protected\.\s+Starting a session is not allowed\.))";
			string pattern;
			bool isProtected;

			pattern = String.Format(CultureInfo.CurrentCulture, template, Regex.Escape(database), Regex.Escape(host));

			grep = new Regex(pattern, RegexOptions.CultureInvariant);
			isProtected = grep.IsMatch(status);

			return (isProtected);
		}

		/// <summary>
		///    Executes a CM Synergy command.
		/// </summary>
		/// <exception cref="CruiseControlException">
		///     Thrown if the CM Synergy command exceeds the configured
		///     <see cref="ProcessInfo.TimeOut"/>.
		/// </exception>
		/// <param name="processInfo">
		///     <see langword="true"/> if a <see cref="CruiseControlException"/>
		///     should be thrown if the CM Synergy command does not return
		///     <c>0</c>.
		/// </param>
		/// <returns>The result of the command.</returns>
		public ProcessResult Execute(ProcessInfo processInfo)
		{
			return Execute(processInfo, true);
		}

		/// <summary>
		///     Executes a CM Synergy command.
		/// </summary>
		/// <exception cref="CruiseControlException">
		///     Thrown if the CM Synergy command exceeds the configured
		///     <see cref="ProcessInfo.TimeOut"/>, or if <paramref see="failOnError" />
		///     is <see langword="true"/> and the commands returns non-zero.
		/// </exception>
		/// <param name="processInfo">
		///     <see langword="true"/> if a <see cref="CruiseControlException"/>
		///     should be thrown if the CM Synergy command does not return
		///     <c>0</c>.
		/// </param>
		/// <param name="failOnError">
		///     Indicates if a <see cref="CruiseControlException"/> should be thrown
		///     if non-zero is returned by the command.
		/// </param>
		/// <returns>The result of the command.</returns>
		public ProcessResult Execute(ProcessInfo processInfo, bool failOnError)
		{
			// require an active session
			ValidateSession();

			/* If the work area path is known, use it instead of the working directory.
             * This should be OK, thanks to the ProcessInfo.RepathExecutableIfItIsInWorkingDirectory
             * implementation, which is called by the non-default ProcessInfo(string,string,string)
             * constructor that we use in SynergyCommandBuilder.CreateProcessInfo */
			if (null != project && null != project.WorkAreaPath && project.WorkAreaPath.Length > 0)
			{
				processInfo = new ProcessInfo(processInfo.FileName, processInfo.Arguments, project.WorkAreaPath);
			}

			// set the session id for ccm.exe to use
			processInfo.EnvironmentVariables[SessionToken] = connection.SessionId;
			// always use invariant (EN-US) date/time formats
			processInfo.EnvironmentVariables[DateTimeFormat] = DateTimeFormat;

			// convert from seconds to milliseconds
			processInfo.TimeOut = connection.Timeout*1000;

			ProcessResult result = executor.Execute(processInfo);
			if (result.TimedOut)
			{
				string message = String.Format(CultureInfo.CurrentCulture, @"Synergy source control operation has timed out after {0} seconds. Process command: ""{1}"" {2}", connection.Timeout, processInfo.FileName, processInfo.PublicArguments);
				throw(new CruiseControlException(message));
			}

			if (result.Failed && failOnError)
			{
				string message = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Synergy source control operation failed.\r\n" + "Command: \"{0}\" {1}\r\n" + "Error Code: {2}\r\n" + "Errors:\r\n{3}\r\n{4}", processInfo.FileName, processInfo.PublicArguments, result.ExitCode, result.StandardError, result.StandardOutput);

				if (result.HasErrorOutput)
				{
					Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Synergy wrote output to stderr: {0}", result.StandardError));
				}

				throw(new CruiseControlException(message));
			}
			return result;
		}
	}
}
