using System;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessExecutor
	{
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
			using(Process process = processInfo.CreateAndStartNewProcess())
			{
				ProcessReader standardOutput = new ProcessReader(process.StandardOutput);
				ProcessReader standardError = new ProcessReader(process.StandardError);
				standardOutput.Start();
				standardError.Start();

				bool hasExited = process.WaitForExit(processInfo.TimeOut);
				if (! hasExited)
				{
					Log.Warning(string.Format("Process timed out: {0} {1}.  Process id: {2}", processInfo.FileName, processInfo.Arguments, process.Id));
					process.Kill();
					Log.Warning(string.Format("The timed out process has been killed: {0}", process.Id));
				}
				standardOutput.WaitForExit();
				standardError.WaitForExit();

				return new ProcessResult(standardOutput.Output, standardError.Output, process.ExitCode, ! hasExited);
			}				
		}
	}
}