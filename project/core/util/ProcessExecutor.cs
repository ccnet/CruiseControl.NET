using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// The ProcessExecutor serves as a simple, injectable facade for executing external processes.  The ProcessExecutor
	/// spawns a new <see cref="RunnableProcess" /> using the properties specified in the input <see cref="ProcessInfo" />.
	/// All output from the executed process is contained within the returned <see cref="ProcessResult" />.
	/// </summary>
	public class ProcessExecutor
	{
		private RunnableProcess p;

		public virtual ProcessResult Execute(ProcessInfo processInfo)
		{
			string projectName = Thread.CurrentThread.Name;
			using (p = new RunnableProcess(processInfo, projectName))
			{
				ProcessMonitor.MonitorProcessForProject(p.Process, projectName);
				ProcessResult run = p.Run();
				ProcessMonitor.RemoveMonitorForProject(projectName);
				return run;
			}
		}

		public static void KillProcessCurrentlyRunningForProject(string name)
		{
			ProcessMonitor monitor = ProcessMonitor.ForProject(name);
			if (monitor == null)
			{
				Log.Debug(string.Format("Request to abort process currently running for project {0}, but no process is currently running.", name));
			}
			else
			{
				monitor.KillProcess();				
			}
		}

		private class RunnableProcess : IDisposable
		{
			private readonly string projectName;
			private readonly ProcessInfo processInfo;
			private readonly Process process;
			// TODO: Move towards injecting WaitHandles.
			private readonly StringBuilder stdOutput = new StringBuilder();
			private readonly EventWaitHandle outputStreamClosed = new ManualResetEvent(false);
			private readonly StringBuilder stdError = new StringBuilder();
			private readonly EventWaitHandle errorStreamClosed = new ManualResetEvent(false);
			private readonly EventWaitHandle processExited = new ManualResetEvent(false);
			private Thread supervisingThread;


			public RunnableProcess(ProcessInfo processInfo, string projectName)
			{
				this.projectName = projectName;
				this.processInfo = processInfo;
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
						"Process timed out: {0} {1}.  Process id: {2}. This process will now be killed.", processInfo.FileName, processInfo.Arguments, process.Id));
				}
				catch (ThreadAbortException)
				{
					// Thread aborted. This is the server trying to exit. Abort needs to continue.
					Log.Info(string.Format(
						"Thread aborted while waiting for '{0} {1}' to exit. Process id: {2}. This process will now be killed.", processInfo.FileName, processInfo.Arguments, process.Id));
					throw;
				} 
				catch (ThreadInterruptedException)
				{
					// If one of the output handlers catches an exception, it will interrupt this thread to wake it.
					// The finally block handles clean-up.
					Log.Debug(string.Format(
						"Process interrupted: {0} {1}.  Process id: {2}. This process will now be killed.", processInfo.FileName, processInfo.Arguments, process.Id));
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
				Log.Debug(string.Format(
							"Starting process [{0}] in working directory [{1}] with arguments [{2}]", process.StartInfo.FileName, process.StartInfo.WorkingDirectory, process.StartInfo.Arguments));
				process.OutputDataReceived += StandardOutputHandler;
				process.ErrorDataReceived += ErrorOutputHandler;
				process.Exited += ExitedHandler;
				process.EnableRaisingEvents = true;
				supervisingThread = Thread.CurrentThread;

				try
				{
					bool isNewProcess = process.Start();
					if (!isNewProcess) Log.Warning("Reusing existing process...");
				}
				catch (Win32Exception e)
				{
					string filename = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);
					string msg = string.Format("Unable to execute file [{0}].  The file may not exist or may not be executable.", filename);
					throw new IOException(msg, e);
				}

				WriteToStandardInput();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
			}

			private void Kill()
			{
				const int WAIT_FOR_KILLED_PROCESS_TIMEOUT = 10000;

				Log.Debug(string.Format("Sending kill to process {0} and waiting {1} seconds for it to exit.", process.Id, WAIT_FOR_KILLED_PROCESS_TIMEOUT / 1000));
				CancelEventsAndWait();
				try
				{
					KillUtil.KillPid(process.Id);
					if (!process.WaitForExit(WAIT_FOR_KILLED_PROCESS_TIMEOUT))
						throw new CruiseControlException(
							string.Format(@"The killed process {0} did not terminate within the allotted timeout period {1}.  The process or one of its child processes may not have died.  This may create problems when trying to re-execute the process.  It may be necessary to reboot the server to recover.",
								process.Id,
								WAIT_FOR_KILLED_PROCESS_TIMEOUT));
					Log.Warning(string.Format("The process has been killed: {0}", process.Id));
				}
				catch (InvalidOperationException)
				{
					Log.Warning(string.Format("Process has already exited before getting killed: {0}", process.Id));
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
				processExited.Set();
			}

			private void StandardOutputHandler(object sender, DataReceivedEventArgs outLine)
			{
				try
				{
					CollectOutput(outLine.Data, stdOutput, outputStreamClosed);
				}
				catch (Exception e)
				{
					Log.Error(e);
					Log.Error(string.Format("[{0} {1}] Exception while collecting standard output", projectName, processInfo.FileName));
					supervisingThread.Interrupt();
				}
			}

			private void ErrorOutputHandler(object sender, DataReceivedEventArgs outLine)
			{
				try
				{
					CollectOutput(outLine.Data, stdError, errorStreamClosed);
				}
				catch (Exception e)
				{
					Log.Error(e);
					Log.Error(string.Format("[{0} {1}] Exception while collecting error output", projectName, processInfo.FileName));
					supervisingThread.Interrupt();
				}
			}

			private void CollectOutput(string output, StringBuilder collector, EventWaitHandle streamReadComplete)
			{
				if (output == null)
				{
					// Null indicates the process has closed the stream
					streamReadComplete.Set();
					return;
				}

				collector.AppendLine(output);
				Log.Debug(string.Format("[{0} {1}] {2}", projectName, processInfo.FileName, output));
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
		}

		/// <summary>
		/// A Process-Monitor receives the currently active process of a specific project
		/// and stores a reference to it.
		/// It can be used to abort a running build.
		/// </summary>
		private class ProcessMonitor
		{
			private static readonly IDictionary<string, ProcessMonitor> processMonitors = new Dictionary<string, ProcessMonitor>();

			// Return an existing Processmonitor
			[MethodImpl(MethodImplOptions.Synchronized)]
			public static ProcessMonitor ForProject(string projectName)
			{
				return processMonitors.ContainsKey(projectName) ? processMonitors[projectName] : null;
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public static void MonitorProcessForProject(Process process, string projectName)
			{
				processMonitors[projectName] = new ProcessMonitor(process, projectName);				
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public static void RemoveMonitorForProject(string projectName)
			{
				processMonitors.Remove(projectName);
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
				Log.Info(string.Format("{0}: ------------------------------------------------------------------", projectName));
				Log.Info(string.Format("{0}: ---------The Build Process was successfully aborted---------------", projectName));
				Log.Info(string.Format("{0}: ------------------------------------------------------------------", projectName));
			}
		}
	}
}
