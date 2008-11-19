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
			using (p = new RunnableProcess(processInfo))
			{
				return p.Run();
			}
		}
		
		// BuildTasks receive a ProcessMonitor to monitor their build process
		public virtual ProcessResult Execute(ProcessInfo processInfo, ProcessMonitor processMonitor)
		{
			using(p = new RunnableProcess(processInfo))
			{
				processMonitor.MonitorNewProcess(p.process);
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
            private static string processName = string.Empty;
			private readonly ProcessInfo processInfo;
			public readonly Process process;

            private readonly StringBuilder stdOutput = new StringBuilder();
            private readonly StringBuilder stdError = new StringBuilder();
            
			public RunnableProcess(ProcessInfo processInfo)
			{
                processName = Thread.CurrentThread.Name;
				this.processInfo = processInfo;
				process = processInfo.CreateProcess();
			}

			public ProcessResult Run()
			{
                bool hasTimedOut = false;
                bool failed = false;
                bool hasExited = false;
                int exitcode = 0;
                                              
                try
                {
                    process.OutputDataReceived += SortOutputHandler;
                    process.ErrorDataReceived += SortErrorOutputHandler;

					Log.Debug(string.Format("Starting process [{0}] in working directory [{1}] with arguments [{2}]", process.StartInfo.FileName, process.StartInfo.WorkingDirectory, process.StartInfo.Arguments));
                    bool isNewProcess = process.Start();
                    if (!isNewProcess) Log.Warning("Reusing existing process...");

                    WriteToStandardInput();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    try
                    {
                        hasExited = process.WaitForExit(processInfo.TimeOut);
                    }
                    catch (ThreadAbortException)
                    {
                        process.CancelErrorRead();
                        process.CancelOutputRead();
                        Thread.ResetAbort();
                    }                    

                    if (!hasExited)
                    {
                        hasTimedOut = true;
                        Kill();
                    }

                    exitcode = process.ExitCode;

                    failed = !processInfo.ProcessSuccessful(exitcode);
                    
                }
                catch (Win32Exception e)
                {
                    string filename = Path.Combine(process.StartInfo.WorkingDirectory, process.StartInfo.FileName);
                    string msg = string.Format("Unable to execute file [{0}].  The file may not exist or may not be executable.", filename);
                    throw new IOException(msg, e);
                }
				
				process.WaitForExit();
                process.Close();
				
				return new ProcessResult(stdOutput.ToString(), stdError.ToString(), exitcode, hasTimedOut, failed);
			}
            
            public void Kill()
            {
                try
                {
                    KillUtil.KillPid(process.Id);
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

            private void SortOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
            {
                // Collect the sort command output.
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = processName;
                    stdOutput.AppendLine(outLine.Data);
                    Log.Debug(outLine.Data);
                }
            }

            private void SortErrorOutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
            {
                // Collect the sort command output.
                if (!String.IsNullOrEmpty(outLine.Data))
                {
                    if (Thread.CurrentThread.Name == null) Thread.CurrentThread.Name = processName;
                    stdError.AppendLine(outLine.Data);
                    Log.Debug(outLine.Data);
                }
            }           

            void IDisposable.Dispose()
			{
				process.Dispose();       
			}
		}
	}
}
