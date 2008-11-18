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
			private readonly ProcessInfo processInfo;
			public readonly Process process;

			public RunnableProcess(ProcessInfo processInfo)
			{
				this.processInfo = processInfo;
				process = processInfo.CreateProcess();
			}

			public ProcessResult Run()
			{
                bool hasTimedOut = false;
                bool failed;
                bool hasExited = false;
                int exitcode;
                                              
                try
                {
                    bool isNewProcess = process.Start();
                    if (!isNewProcess) Log.Warning("Reusing existing process...");

                    WriteToStandardInput();

                    try
                    {
                        hasExited = process.WaitForExit(processInfo.TimeOut);
                    }
                    catch (ThreadAbortException)
                    {
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

				string stdOut = process.StandardOutput.ReadToEnd();
				string stdErr = process.StandardError.ReadToEnd();
                process.Close();

				WriteToLog(stdOut);
				WriteToLog(stdErr);
				return new ProcessResult(stdOut, stdErr, exitcode, hasTimedOut, failed);
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

			private static void WriteToLog(string lines)
			{
				foreach(string line in lines.Split(Environment.NewLine.ToCharArray()))
				{
					if(!String.IsNullOrEmpty(line))
					{
						Log.Debug(line);
					}
				}
			}      

            void IDisposable.Dispose()
			{
				process.Dispose();       
			}
		}
	}
}
