using System;
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
		public virtual ProcessResult Execute(ProcessInfo processInfo)
		{
			using (RunnableProcess p = new RunnableProcess(processInfo))
			{
				return p.Run();
			}
		}

		/// <summary>
		/// The RunnableProcess class encapsulates the state from an executing <see cref="Process" />.  It is 
		/// designed specifically to deal with processes that redirect the results of both 
		/// the standard output and the standard error streams.  Reading from these streams is performed in
		/// a separate thread using the <see cref="ProcessReader"/> class, in order to prevent deadlock while 
		/// waiting for the process to finish. RunnableProcess also subscribes to the <see cref="Process.Exited" /> event
		/// to receive asynchronous notification that the process has terminated; the executing thread then blocks on an
		/// ManualResetEvent latch until the event is raised.  Using the <see cref="Process.Exited" /> event is preferrable to
		/// using the synchronous <see cref="Process.WaitForExit()" /> method as <see cref="Process.WaitForExit()" /> does not respond
		/// immediately if the executing thread is aborted. If the process does not complete executing within the specified timeout period, 
		/// or if the waiting thread is aborted, the RunnableProcess will attempt to kill the process. Because the <see cref="Process.Exited" />
		/// is fired asynchronously from a thread in the thread pool, it may fire after the RunnableProcess object has been disposed (but
		/// before it has been garbage collected).  Therefore it is important to ensure that latch is not set if the object has already been disposed.
		/// As process termination is asynchronous, the RunnableProcess will wait for the process to die using.  Under certain circumstances, 
		/// the process does not terminate gracefully after being killed, causing the RunnableProcess to throw an exception.
		/// </summary>
		private class RunnableProcess : IDisposable
		{
			private const int WAIT_FOR_KILLED_PROCESS_TIMEOUT = 5000;
			private readonly ProcessInfo processInfo;
			private readonly Process process;
			private readonly ManualResetEvent latch = new ManualResetEvent(false);
			private bool hasTimedOut = false;
			private bool disposed = false;

			public RunnableProcess(ProcessInfo processInfo)
			{
				this.processInfo = processInfo;
				process = processInfo.CreateProcess();
				process.EnableRaisingEvents = true;
				process.Exited += new EventHandler(process_Exited);
			}

			public ProcessResult Run()
			{
				StartProcess();
				// Process must be started before StandardOutput and StandardError streams are accessible
				using (ProcessReader standardOutput = new ProcessReader(process.StandardOutput), standardError = new ProcessReader(process.StandardError))
				{
					try
					{
						WriteToStandardInput();
						WaitForProcessToExit();
					}
					finally
					{
						// Guarantee that process will be killed if it has not exited cleanly
						if (! process.HasExited)
						{
							Kill();
						}
						// Read in the remainder of the redirected streams
						standardOutput.WaitForExit();
						standardError.WaitForExit();
					}
					return new ProcessResult(standardOutput.Output, standardError.Output, process.ExitCode, hasTimedOut);
				}
			}

			private void StartProcess()
			{
				Log.Debug(string.Format("Starting process [{0}] in working directory [{1}] with arguments [{2}]", process.StartInfo.FileName, process.StartInfo.WorkingDirectory, process.StartInfo.Arguments));
				try
				{
					bool isNewProcess = process.Start();
					if (! isNewProcess) Log.Warning("Reusing existing process...");
				}
				catch (Win32Exception e)
				{
					string filename = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);
					string msg = string.Format("Unable to execute file [{0}].  The file may not exist or may not be executable.", filename);
					throw new IOException(msg, e);
				}
			}

			private void WaitForProcessToExit()
			{
				bool released = latch.WaitOne(processInfo.TimeOut, false);
				process.Refresh();
				if (released && ! process.HasExited)
					Log.Error("Latch released but process has not exited!  ** Process may have exited abnormally before timeout expired **");

				hasTimedOut = ! process.HasExited;
				if (hasTimedOut)
					Log.Warning(string.Format("Process timed out: {0} {1}.  Process id: {2}.  This process will now be killed.", processInfo.FileName, processInfo.Arguments, process.Id));
			}

			private void process_Exited(object sender, EventArgs e)
			{
				if (! disposed) latch.Set();
			}

			private void Kill()
			{
				try
				{
                   // TODO: Come back here some day when MS fixed the Process.Kill() bug (see CCNET-815)
                   // process.Kill();
                    KillUtil.KillPid(process.Id);
                    if (!process.WaitForExit(WAIT_FOR_KILLED_PROCESS_TIMEOUT))
						throw new CruiseControlException(string.Format(@"The killed process {0} did not terminate within the allotted timeout period {1}.  The process or one of its child processes may not have died.  This may create problems when trying to re-execute the process.  It may be necessary to reboot the server to recover.", process.Id, WAIT_FOR_KILLED_PROCESS_TIMEOUT));
					Log.Warning(string.Format("The process has been killed: {0}", process.Id));
				}
				catch (InvalidOperationException)
				{
					Log.Warning(string.Format("Process has already exited before getting killed: {0}", process.Id));
				}
			}

			private void WriteToStandardInput()
			{
				// TODO - maybe we actually need to do this line-by-line. In that case we should probably extract this 
				//   to a 'ProcessWriter' and do the thread stuff like the Readers do. -- Mike R
				if (process.StartInfo.RedirectStandardInput)
				{
					process.StandardInput.Write(processInfo.StandardInputContent);
					process.StandardInput.Flush();
					process.StandardInput.Close();
				}
			}

			void IDisposable.Dispose()
			{
				disposed = true;
				process.Dispose();
				latch.Close();
			}
		}
	}
}