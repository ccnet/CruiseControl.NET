using System;
using System.Collections.Specialized;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessInfo
	{
		private const int DEFAULT_TIMEOUT = 120000;
		private ProcessStartInfo startInfo = new ProcessStartInfo();

		public ProcessInfo(string filename, string arguments) : this(filename, arguments, null) { }

		public ProcessInfo(string filename, string arguments, string workingDirectory)
		{
			startInfo.FileName = filename;
			startInfo.Arguments = arguments;
			startInfo.WorkingDirectory = workingDirectory;
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
		}

		public StringDictionary EnvironmentVariables
		{
			get { return startInfo.EnvironmentVariables; }
		}

		public string FileName
		{
			get { return startInfo.FileName; }
		}

		public string Arguments 
		{
			get { return startInfo.Arguments; }
		}

		public string WorkingDirectory
		{
			get { return startInfo.WorkingDirectory; }
		}

		public int TimeOut = DEFAULT_TIMEOUT;

		public Process CreateAndStartNewProcess()
		{
			return Process.Start(startInfo);
		}
	}
}
