using System;
using System.IO;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

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
	/// <include file="example.xml" path="/example" />
	[ReflectorType("synergy")]
	public class Synergy : ISourceControl, IDisposable
	{
		/// <summary>The execution client for the Synergy process.</summary>
		private ISynergyCommand command;

		/// <summary>The configured settings for the Synergy server connection.</summary>
		private SynergyConnectionInfo connection;

		/// <summary>The configured settings for the Synergy integration project.</summary>
		private SynergyProjectInfo project;

		/// <summary>The optional ChangeSynergy URL builder.</summary>
		private IModificationUrlBuilder urlBuilder;

		private SynergyParser parser;

		/// <summary>
		///     Default constructor.  Initializes all members to their default values.
		/// </summary>
		public Synergy() : this(new SynergyConnectionInfo(), new SynergyProjectInfo())
		{}

		public Synergy(SynergyConnectionInfo connection, SynergyProjectInfo project) : this(connection, project, new SynergyCommand(connection, project), new SynergyParser())
		{}

		public Synergy(SynergyConnectionInfo connection, SynergyProjectInfo project, ISynergyCommand command, SynergyParser parser)
		{
			this.connection = connection;
			this.project = project;
			this.command = command;
			this.parser = parser;
		}

		/// <summary>
		///     Finalizer that ensures that Synergy connections are eventually closed.
		/// </summary>
		~Synergy()
		{
			Dispose();
		}

		/// <summary>
		///     Connection info to create a session.
		/// </summary>
		[ReflectorProperty("connection", InstanceType=typeof (SynergyConnectionInfo))]
		public SynergyConnectionInfo Connection
		{
			get { return connection; }
			set
			{
				connection = value;
				OpenNewCommand();
			}
		}

		private void OpenNewCommand()
		{
			command.Dispose();
			command = new SynergyCommand(connection, project);
		}

		/// <summary>
		///     The info for the integration testing project.
		/// </summary>
		[ReflectorProperty("project", InstanceType=typeof (SynergyProjectInfo))]
		public SynergyProjectInfo Project
		{
			get { return project; }
			set
			{
				project = value;
				OpenNewCommand();
			}
		}

		/// <summary>
		///     The Web Url builder to use.  Generally this should be 
		///     a <see cref="ChangeSynergyUrlBuilder"/> instance.
		/// </summary>
		[ReflectorProperty("changeSynergy", InstanceType=typeof (ChangeSynergyUrlBuilder), Required=false)]
		public IModificationUrlBuilder UrlBuilder
		{
			get { return urlBuilder; }
			set
			{
				// save the reference, even if it is not of concrete type SynergyUrlBuilder
				urlBuilder = value;
				// assume we have a reference to a ChangeSynergy URL builder
				ChangeSynergyUrlBuilder temp = urlBuilder as ChangeSynergyUrlBuilder;
				// check if the cast succeeded
				if (null != temp)
				{
					// initialize the ChangeSynergy credentials 
					temp.SetCredentials(connection);
				}
			}
		}

		/// <summary>
		///     Ensures that the Synergy session has been <see cref="SynergyCommand.Close">Closed</see>.
		/// </summary>
		/// <remarks>
		///     Based on the implementation suggested by 
		///     <see href="http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpref/html/frlrfSystemIDisposableClassTopic.asp">MSDN</see>
		/// </remarks>
		public void Dispose()
		{
			command.Dispose();
		}

		public void Initialize(IProject project)
		{}

		/// <summary>
		///     Performs a CM Synergy "Reconfigure"/"Update Members" for a forced build.
		/// </summary>
		/// <remarks>
		/// 	<see cref="GetModifications"/> will also reconfigure when modifications are detected
		///     which explains why this method is a no-op unless we have a forced build.
		/// </remarks>
		/// <param name="integration">Not used.</param>
		/// <url>element://model:project::CCNet.Synergy.Plugin/design:view:::ax60xur0dt7rg6h_v</url>
		public void GetSource(IIntegrationResult integration)
		{
            integration.BuildProgressInformation.SignalStartRunTask("Getting source from Telelogic Synergy");

			// reconcile any work area paths specificed by the config file
			Reconcile();

			// reconfigure the project
			ProcessInfo info = SynergyCommandBuilder.Reconfigure(connection, project);
			command.Execute(info);
			/* UNDO -- refactored out - no longer need to count the number of objects replaced 
              int objectsReplaced = SynergyParser.GetReconfigureCount(processResult.StandardOutput); */

			// update the timestamp of the reconfigure
			project.LastReconfigureTime = GetReconfigureTime();
		}

		/// <summary>
		///     No implmentation.
		/// </summary>
		/// <param name="project">Not used.</param>
		public void Purge(IProject project)
		{}

		/// <summary>
		///     Interface implementation to get the list of changes since the last integration
		///     run.
		/// </summary>
		/// <remarks>
		/// 	<notes type="implementnotes">
		///         Automatically updates the source configuration by calling <see cref="GetSource"/>.
		///         This must be done here, rather than in the <see cref="GetSource"/> implmentation
		///         because the Synergy <c>finduse</c> query command depends on the project having
		///         been reconfigured.
		///     </notes>
		/// </remarks>
		/// <param name="from">The date of the last integration run.</param>
		/// <param name="to">Not used.</param>
		/// <returns>
		///     An empty array of modifications by by default.
		///     If changes have occurred since the last integration attempt, an array containing
		///     each new modification is returned.
		/// </returns>
		/// <url>element://model:project::CCNet.Synergy.Plugin/design:view:::qmbr0gle9x4bzse_v</url>
		/// <url>element://model:project::CCNet.Synergy.Plugin/design:view:::zs45gn0dmb8iufh_v</url>
		/// <url>element://model:project::CCNet.Synergy.Plugin/design:view:::vt4zadwko_v</url>
		public Modification[] GetModifications(IIntegrationResult from, IIntegrationResult to)
		{
			return GetModifications(from.LastModificationDate);
		}

		private Modification[] GetModifications(DateTime from)
		{
			Modification[] modifications = new Modification[0];

			if (project.TemplateEnabled)
			{
				// setup the project to reconfigure using the default template
				command.Execute(SynergyCommandBuilder.UseReconfigureTemplate(connection, project));
			}

			// refresh the query based folders in the reconfigure properties
			command.Execute(SynergyCommandBuilder.UpdateReconfigureProperites(connection, project));

			// this may fail, if a build was forced, and no changes were found
			ProcessResult result = command.Execute(SynergyCommandBuilder.GetNewTasks(connection, project, from), false);
			if (! result.Failed)
			{
				// cache the output of the task/comment query
				string comments = result.StandardOutput;

				// populate the selection set with the objects associated with the detected tasks
				result = command.Execute(SynergyCommandBuilder.GetTaskObjects(connection, project), false);

				if (! result.Failed)
				{
					// Get the path information for each object associated with the tasks
					result = command.Execute(SynergyCommandBuilder.GetObjectPaths(connection, project), false);
					if (! result.Failed)
					{
						modifications = parser.Parse(comments, result.StandardOutput, from);

						if (null != urlBuilder)
						{
							urlBuilder.SetupModification(modifications);
						}
					}
				}
			}
            
            FillIssueUrl(modifications);
			return modifications;
		}

		/// <summary>
		///     Adds tasks to a shared task folder, if configured, and creates a baseline
		///     if requested by the configuration.
		/// </summary>
		/// <remarks>
		/// 	<note type="implementnotes">
		///         This method makes use of CM Synergy selection commands, in order to pipe the 
		///         results of one query to another command.  If the CM Synergy session is lost
		///         during the course of execution in this method, the selection set is also lost.
		///         An exception will be thrown when the next CM Synergy command is executed,
		///         because the selection set is empty. This should be a very rare case, and the
		///         performance gains of piping resultsets are worthwhile.
		///     </note>
		/// </remarks>
		/// <exception cref="CruiseControlException">
		///     Thrown if an external process has reconfigured the project since 
		///     <see cref="GetModifications"/> was called.
		/// </exception>
		/// <param name="result">Not used.</param>
		/// <url>element://model:project::CCNet.Synergy.Plugin/design:view:::ow43bejw6wm4was_v</url>
		public void LabelSourceControl(IIntegrationResult result)
		{
			DateTime currentReconfigureTime = GetReconfigureTime();
			if (currentReconfigureTime != project.LastReconfigureTime)
			{
				string message = String.Format(@"Invalid project state.  Cannot add tasks to shared folder '{0}' because " + @"the integration project '{1}' was internally reconfigured at '{2}' " + @"and externally reconfigured at '{3}'.  Projects cannot be reconfigured " + @"during an integration run.", project.TaskFolder, project.ProjectSpecification, project.LastReconfigureTime, currentReconfigureTime);
				throw(new CruiseControlException(message));
			}

			/* Populate the query selection set with a list of ALL tasks 
             * not in the manual folder. This includes all tasks for this integration,
             * and any prior failed integrations.
             * We find these by passing the the maximum range of dates to GetModifications */
			result.Modifications = GetModifications(DateTime.MinValue);

			// skip this step if a build was forced, and no changes were found
			if (null != result.Modifications && result.Modifications.Length > 0)
			{
				// comment those tasks with the "label", for both shared folders and baselines
				command.Execute(SynergyCommandBuilder.AddLabelToTaskComment(connection, project, result));

				// append tasks to the shared folder, if one was specified
				if (SynergyProjectInfo.DefaultTaskFolder != project.TaskFolder)
				{
					// append those tasks in the selection set to the shared build folder
					command.Execute(SynergyCommandBuilder.AddTasksToFolder(connection, project, result));
				}
			}

			// create a baseline, if requested
			if (project.BaseliningEnabled)
			{
				command.Execute(SynergyCommandBuilder.CreateBaseline(connection, project, result));
			}
		}

		/// <summary>
		///     If enabled, discards changes to specified work area paths.
		/// </summary>
		/// <remarks>
		///     Supports both file and directory paths.  Useful if you build process
		///     emits artifacts under source control.  Changes to controlled files can cause
		///     reconfigure commands to fail.
		/// </remarks>
		private void Reconcile()
		{
			// force a connection to be established, if it hasn't already
			// need in case of a forced build
			command.Execute(SynergyCommandBuilder.Heartbeat(connection));

			if (null != project.ReconcilePaths)
			{
				string fullPath;
				foreach (string path in project.ReconcilePaths)
				{
					// normalize the path
					if (! Path.IsPathRooted(path))
						fullPath = Path.Combine(project.WorkAreaPath, path);
					else
						fullPath = path;

					fullPath = Path.GetFullPath(fullPath);
					Log.Info(String.Concat("Reconciling work area path '", path, "'"));

					// sync the work area to discard work area changes
					command.Execute(SynergyCommandBuilder.Reconcile(connection, project, path));
				}
			}
		}

		/// <summary>
		///     Gets the date of the project's last reconfigure time to ensure consistency.
		/// </summary>
		/// <exception cref="CruiseControlException">
		///     Thrown if the last reconfigure time cannot be read or parsed successfully.
		/// </exception>
		/// <returns></returns>
		public DateTime GetReconfigureTime()
		{
			// setup the project to reconfigure using the default template
			ProcessResult result = command.Execute(SynergyCommandBuilder.GetLastReconfigureTime(connection, project));
			try
			{
				return DateTime.Parse(result.StandardOutput.Trim(), connection.FormatProvider);
			}
			catch (Exception inner)
			{
				throw(new CruiseControlException("Failed to read the project's last reconfigure time.", inner));
			}
		}

        [ReflectorProperty("issueUrlBuilder", InstanceTypeKey = "type", Required = false)]
        public IModificationUrlBuilder IssueUrlBuilder;


        private void FillIssueUrl(Modification[] modifications)
        {
            if (IssueUrlBuilder != null)
            {
                IssueUrlBuilder.SetupModification(modifications);
            }
        }

	}
}