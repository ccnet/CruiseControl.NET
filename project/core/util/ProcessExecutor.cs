using System;
using System.Diagnostics;
using System.ComponentModel;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessExecutor
	{
		private const int KILLED_PROCESS_EXIT_CODE = -1;
		private const int DEFAULT_TIMEOUT = 120000;
		public int Timeout = DEFAULT_TIMEOUT;

		public ProcessResult Execute(string filename, string arguments)
		{
			return Execute(new ProcessInfo(filename, arguments));
		}

		public ProcessResult Execute(string filename, string arguments, string workingDirectory)
		{
			return Execute(new ProcessInfo(filename, arguments, workingDirectory));
		}

		public virtual ProcessResult Execute(ProcessInfo processInfo)
		{
			using(Process process = Process.Start(processInfo.startInfo))
			{
				ProcessReader standardOutput = new ProcessReader(process.StandardOutput);
				ProcessReader standardError = new ProcessReader(process.StandardError);
				standardOutput.Start();
				standardError.Start();

				bool hasExited = process.WaitForExit(Timeout);
				if (hasExited)
				{
					standardOutput.WaitForExit();
					standardError.WaitForExit();
				}
				else
				{
					Log.Warning(string.Format("Process timed out: {0} {1}.  Process id: {2}", processInfo.FileName, processInfo.Arguments, process.Id));
					process.Kill();
					standardOutput.Abort();		// streams must be aborted after the process is killed -- otherwise there is a risk that the process will
					standardError.Abort();		// already be dead when process.Kill is invoked
					process.WaitForExit();
					Log.Warning(string.Format("The timed out process has been killed: {0}", process.Id));
				}
				return new ProcessResult(standardOutput.Output, standardError.Output, process.ExitCode, ! hasExited);
			}				
		}
	}
}