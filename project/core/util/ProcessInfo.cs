using System;
using System.Collections.Specialized;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessInfo
	{
		internal ProcessStartInfo startInfo = new ProcessStartInfo();

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
	}
}
