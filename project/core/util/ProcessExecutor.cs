using System;
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
				return p.Run();
			}
		}

		// BuildTasks receive a ProcessMonitor to monitor their build process
		public virtual ProcessResult Execute(ProcessInfo processInfo, string projectName)
		{
			ProcessMonitor processMonitor = ProcessMonitor.ForProject(projectName);
			using (p = new RunnableProcess(processInfo, projectName))
			{
				processMonitor.MonitorNewProcess(p.Process);
				return p.Run();
			}
		}

		public void Kill()
		{
			Log.Info(string.Format("The process will be killed: {0}", p));
			try
			{
				using (p)
				{
					p.Kill();
				}
			}
			catch (InvalidOperationException)
			{
				Log.Info(string.Format("The process can't be killed because it got disposed already: {0}", p));
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
					hasExited = WaitHandle.WaitAll(new WaitHandle[] {errorStreamClosed, outputStreamClosed, processExited}, processInfo.TimeOut, true);
					hasTimedOut = !hasExited;
					if (hasTimedOut) Log.Warning(string.Format(
						"Process timed out: {0} {1}.  Process id: {2}.  This process will now be killed.", processInfo.FileName, processInfo.Arguments, process.Id));												
				}
				catch (ThreadAbortException)
				{
					// Thread aborted.
					Log.Info(string.Format(
						"Thread aborted while waiting for '{0} {1}' to exit. Process id: {2}", processInfo.FileName, processInfo.Arguments, process.Id));
					// Will still do best to record output.
					CancelOutputAndWait();
					// Integration should now be stopped. We can continue here as the task will report a failure and the current build will stop.
					Thread.ResetAbort();
				}

				if (!hasExited)
				{
					Kill();
				}

				int exitcode = process.ExitCode;
				bool failed = !processInfo.ProcessSuccessful(exitcode);
				process.Close();

				return new ProcessResult(stdOutput.ToString(), stdError.ToString(), exitcode, hasTimedOut, failed);
			}

			private void CancelOutputAndWait()
			{
				process.CancelErrorRead();
				process.CancelOutputRead();
				WaitHandle.WaitAll(new WaitHandle[] { errorStreamClosed, outputStreamClosed }, 1000, true);
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

			private void ExitedHandler(object sender, EventArgs e)
			{
				processExited.Set();
			}

			public void Kill()
			{
				const int WAIT_FOR_KILLED_PROCESS_TIMEOUT = 10000;

				Log.Debug(string.Format("Sending kill to process {0} and waiting {1} seconds for it to exit.", process.Id, WAIT_FOR_KILLED_PROCESS_TIMEOUT / 1000));
				CancelOutputAndWait();
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

			private void WriteToStandardInput()
			{
				if (process.StartInfo.RedirectStandardInput)
				{
					process.StandardInput.Write(processInfo.StandardInputContent);
					process.StandardInput.Flush();
					process.StandardInput.Close();
				}
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
					supervisingThread.Abort();
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
					supervisingThread.Abort();
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
				process.Dispose();
			}

			// TODO: Smelly. ProcessMonitor doesn't seem like the right abstraction.
			public Process Process
			{
				get { return process; }
			}
		}
	}
}
