using System;
using System.Diagnostics;

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

		public ProcessResult Execute(ProcessInfo processInfo)
		{
			System.Diagnostics.Process process = null;
			try
			{
				process = System.Diagnostics.Process.Start(processInfo.startInfo);

				ProcessReader standardOutput = new ProcessReader(process.StandardOutput);
				ProcessReader standardError = new ProcessReader(process.StandardError);

				standardOutput.Start();
				standardError.Start();

				if (process.WaitForExit(Timeout))
				{
					standardOutput.WaitForExit();
					standardError.WaitForExit();
					return new ProcessResult(standardOutput.Output, standardError.Output, process.ExitCode, ! process.HasExited);
				}
				else
				{
					standardOutput.Abort();
					standardError.Abort();
					process.Kill();
					return new ProcessResult(standardOutput.Output, standardError.Output, KILLED_PROCESS_EXIT_CODE, ! process.HasExited);
				}
			}
			finally
			{
				if (process != null) process.Close();
			}
		}
	}
}