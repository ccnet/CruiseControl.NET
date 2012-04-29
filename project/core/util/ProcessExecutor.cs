
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// The ProcessExecutor serves as a simple, injectable facade for executing external processes.  The ProcessExecutor
	/// spawns a new <see cref="RunnableProcess" /> using the properties specified in the input <see cref="ProcessInfo" />.
	/// All output from the executed process is contained within the returned <see cref="ProcessResult" />.
	/// </summary>
	public class ProcessExecutor
	{
        /// <summary>
        /// Occurs when [process output].	
        /// </summary>
        /// <remarks></remarks>
		public event EventHandler<ProcessOutputEventArgs> ProcessOutput;

        /// <summary>
        /// Executes the specified process info.	
        /// </summary>
        /// <param name="processInfo">The process info.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public virtual ProcessResult Execute(ProcessInfo processInfo)
		{
			string projectName = Thread.CurrentThread.Name;
			using (RunnableProcess p = new RunnableProcess(processInfo, projectName, processInfo == null ? null : processInfo.PublicArguments))
			{
				p.ProcessOutput += ((sender, e) => OnProcessOutput(e));

				ProcessMonitor.MonitorProcessForProject(p.Process, projectName);
				ProcessResult run = p.Run();
				ProcessMonitor.RemoveMonitorForProject(projectName);
				return run;
			}
		}

        /// <summary>
        /// Kills the process currently running for project.	
        /// </summary>
        /// <param name="name">The name.</param>
        /// <remarks></remarks>
		public static void KillProcessCurrentlyRunningForProject(string name)
		{
			ProcessMonitor monitor = ProcessMonitor.ForProject(name);
			if (monitor == null)
			{
				Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Request to abort process currently running for project {0}, but no process is currently running.", name));
			}
			else
			{
				monitor.KillProcess();				
			}
		}

        /// <summary>
        /// Raises the <see cref="E:ProcessOutput" /> event.	
        /// </summary>
        /// <param name="eventArgs">The <see cref="ProcessOutputEventArgs" /> instance containing the event data.</param>
        /// <remarks></remarks>
		protected virtual void OnProcessOutput(ProcessOutputEventArgs eventArgs)
		{
			EventHandler<ProcessOutputEventArgs> handler = this.ProcessOutput;
			if (handler == null)
				return;

			handler(this, eventArgs);
		}

		private class RunnableProcess : IDisposable
		{
			public event EventHandler<ProcessOutputEventArgs> ProcessOutput;

			private readonly string projectName;
			private readonly ProcessInfo processInfo;
			private readonly Process process;
            private readonly string publicArgs;
			// TODO: Move towards injecting WaitHandles.
			private readonly StringBuilder stdOutput = new StringBuilder();
			private readonly EventWaitHandle outputStreamClosed = new ManualResetEvent(false);
			private readonly StringBuilder stdError = new StringBuilder();
			private readonly EventWaitHandle errorStreamClosed = new ManualResetEvent(false);
			private readonly EventWaitHandle processExited = new ManualResetEvent(false);
			private Thread supervisingThread;


            public RunnableProcess(ProcessInfo processInfo, string projectName, string publicArgs)
			{
				this.projectName = projectName;
				this.processInfo = processInfo;
                this.publicArgs = publicArgs;
				process = processInfo.CreateProcess();
			}

			public ProcessResult Run()
			{
				bool hasTimedOut = false;
				bool hasExited = false;

				StartProcess();

				try
				{
					hasExited = WaitHandle.WaitAll(new WaitHandle[] { errorStreamClosed, outputStreamClosed, processExited }, processInfo.TimeOut, true);
					hasTimedOut = !hasExited;
					if (hasTimedOut) Log.Warning(string.Format(
                        CultureInfo.CurrentCulture, "Process timed out: {0} {1}.  Process id: {2}. This process will now be killed.", 
                        processInfo.FileName, 
                        processInfo.PublicArguments, 
                        process.Id));
				}
				catch (ThreadAbortException)
				{
					// Thread aborted. This is the server trying to exit. Abort needs to continue.
					Log.Info(string.Format(
						CultureInfo.CurrentCulture, "Thread aborted while waiting for '{0} {1}' to exit. Process id: {2}. This process will now be killed.", 
                        processInfo.FileName,
                        processInfo.PublicArguments, 
                        process.Id));
					throw;
				} 
				catch (ThreadInterruptedException)
				{
					// If one of the output handlers catches an exception, it will interrupt this thread to wake it.
					// The finally block handles clean-up.
					Log.Debug(string.Format(
						CultureInfo.CurrentCulture, "Process interrupted: {0} {1}.  Process id: {2}. This process will now be killed.", 
                        processInfo.FileName,
                        processInfo.PublicArguments, 
                        process.Id));
				}
				finally
				{
					if (!hasExited)
					{
						Kill();
					}
				}

				int exitcode = process.ExitCode;
				bool failed = !processInfo.ProcessSuccessful(exitcode);

				return new ProcessResult(stdOutput.ToString(), stdError.ToString(), exitcode, hasTimedOut, failed);
			}

			private void StartProcess()
			{
                Log.Debug("Starting process [{0}] in working directory [{1}] with arguments [{2}]",
                    process.StartInfo.FileName,
                    process.StartInfo.WorkingDirectory,
                    this.publicArgs);
				process.OutputDataReceived += StandardOutputHandler;
				process.ErrorDataReceived += ErrorOutputHandler;
				process.Exited += ExitedHandler;
				process.EnableRaisingEvents = true;
				supervisingThread = Thread.CurrentThread;

                string filename = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);

				try
				{
					bool isNewProcess = process.Start();
					if (!isNewProcess) Log.Warning("Reusing existing process...");

                    // avoid useless setting of the default
                    if (processInfo.Priority != System.Diagnostics.Process.GetCurrentProcess().PriorityClass)
                    {
                        try
                        {
                            Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Setting PriorityClass on [{0}] to {1}", filename, processInfo.Priority));
                            process.PriorityClass = processInfo.Priority;
                        }
                        catch (Exception ex)
                        {
                            if (!process.HasExited)
                            {
                                Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unable to set PriorityClass on [{0}]: {1}", filename, ex.ToString()));
                            }
                        }
                    }
                    else
                    {
                        Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Not setting PriorityClass on [{0}] to default {1}", filename, processInfo.Priority));
                    }
				}
				catch (Win32Exception e)
				{
					string msg = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Unable to execute file [{0}].  The file may not exist or may not be executable. ({1})", filename, e.Message);
					throw new IOException(msg, e);
				}

				WriteToStandardInput();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
			}

			private void Kill()
			{
				const int waitForKilledProcessTimeout = 10000;

				Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Sending kill to process {0} and waiting {1} seconds for it to exit.", process.Id, waitForKilledProcessTimeout / 1000));
				CancelEventsAndWait();
				try
				{
					KillUtil.KillPid(process.Id);
					if (!process.WaitForExit(waitForKilledProcessTimeout))
						throw new CruiseControlException(
							string.Format(CultureInfo.CurrentCulture, @"The killed process {0} did not terminate within the allotted timeout period {1}.  The process or one of its child processes may not have died.  This may create problems when trying to re-execute the process.  It may be necessary to reboot the server to recover.",
								process.Id,
								waitForKilledProcessTimeout));
					Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"The process has been killed: {0}", process.Id));
				}
				catch (InvalidOperationException)
				{
					Log.Warning(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Process has already exited before getting killed: {0}", process.Id));
				}
			}

			private void CancelEventsAndWait()
			{
				process.EnableRaisingEvents = false;
				process.Exited -= ExitedHandler;

				process.CancelErrorRead();
				process.CancelOutputRead();
				WaitHandle.WaitAll(new WaitHandle[] { errorStreamClosed, outputStreamClosed }, 1000, true);
			}

			private void WriteToStandardInput()
			{
				if (process.StartInfo.RedirectStandardInput)
				{
					process.StandardInput.Write(processInfo.StandardInputContent);
					process.StandardInput.Flush();
					process.StandardInput.Close();
				}
			}

			private void ExitedHandler(object sender, EventArgs e)
			{
                Log.Debug("[{0} {1}] process exited event received", projectName, processInfo.FileName);
				processExited.Set();
			}

			private void StandardOutputHandler(object sender, DataReceivedEventArgs outLine)
			{
				try
				{
					CollectOutput(outLine.Data, stdOutput, outputStreamClosed, "standard-output");

                    if (!string.IsNullOrEmpty(outLine.Data))
                    {
                        OnProcessOutput(new ProcessOutputEventArgs(ProcessOutputType.StandardOutput, outLine.Data));
                    }
				}
				catch (Exception e)
				{
					Log.Error(e);
					Log.Error(string.Format(System.Globalization.CultureInfo.CurrentCulture,"[{0} {1}] Exception while collecting standard output", projectName, processInfo.FileName));
					supervisingThread.Interrupt();
				}
			}

			private void ErrorOutputHandler(object sender, DataReceivedEventArgs outLine)
			{
				try
				{
					CollectOutput(outLine.Data, stdError, errorStreamClosed, "standard-error");

                    if (!string.IsNullOrEmpty(outLine.Data))
                    {
                        OnProcessOutput(new ProcessOutputEventArgs(ProcessOutputType.ErrorOutput, outLine.Data));
                    }
				}
				catch (Exception e)
				{
					Log.Error(e);
					Log.Error(string.Format(System.Globalization.CultureInfo.CurrentCulture,"[{0} {1}] Exception while collecting error output", projectName, processInfo.FileName));
					supervisingThread.Interrupt();
				}
			}

			private void CollectOutput(string output, StringBuilder collector, EventWaitHandle streamReadComplete, string streamLabel)
			{
				if (output == null)
				{
                    Log.Debug("[{0} {1}] {2} stream closed -- null received in event", projectName, processInfo.FileName, streamLabel);
					streamReadComplete.Set();
					return;
				}

				collector.AppendLine(output);
				Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"[{0} {1}] {2}", projectName, processInfo.FileName, output));
			}

			void IDisposable.Dispose()
			{
				outputStreamClosed.Close();
				errorStreamClosed.Close();
				processExited.Close();
				process.Dispose();
			}

			// TODO: Smelly. ProcessMonitor doesn't seem like the right abstraction.
			public Process Process
			{
				get { return process; }
			}

			protected virtual void OnProcessOutput(ProcessOutputEventArgs eventArgs)
			{
				EventHandler<ProcessOutputEventArgs> handler = this.ProcessOutput;
				if (handler == null)
					return;

				handler(this, eventArgs);
			}
		}

		/// <summary>
		/// A Process-Monitor receives the currently active process of a specific project
		/// and stores a reference to it.
		/// It can be used to abort a running build.
		/// </summary>
		private class ProcessMonitor
		{
			private static readonly IDictionary<string, ProcessMonitor> processMonitors = new Dictionary<string, ProcessMonitor>();
            private static object lockObject = new object();

			// Return an existing Processmonitor
			public static ProcessMonitor ForProject(string projectName)
			{
                Monitor.TryEnter(lockObject, 60000);
                try
                {
                    return processMonitors.ContainsKey(projectName) ? processMonitors[projectName] : null;
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
			}

            public static void MonitorProcessForProject(Process process, string projectName)
            {
                Monitor.TryEnter(lockObject, 60000);
                try
                {
                    processMonitors[projectName] = new ProcessMonitor(process, projectName);
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }

            public static void RemoveMonitorForProject(string projectName)
            {
                Monitor.TryEnter(lockObject, 60000);
                try
                {
                    processMonitors.Remove(projectName);
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }

			private readonly Process process;
			private readonly string projectName;

			private ProcessMonitor(Process process, string projectName)
			{
				this.process = process;
				this.projectName = projectName;
			}

			// Kill the process
			public void KillProcess()
			{
				KillUtil.KillPid(process.Id);
				Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}: ------------------------------------------------------------------", projectName));
				Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}: ---------The Build Process was successfully aborted---------------", projectName));
				Log.Info(string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}: ------------------------------------------------------------------", projectName));
			}
		}
	}
}
