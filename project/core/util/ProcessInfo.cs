using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessInfo
	{
		private const int DEFAULT_TIMEOUT = 120000;
		public const int INFINITE_TIMEOUT = 0;
		private ProcessStartInfo startInfo = new ProcessStartInfo();
		private string standardInputContent = null;
		private int timeout = DEFAULT_TIMEOUT;

		public ProcessInfo(string filename) : this(filename, null)
		{
		}

		public ProcessInfo(string filename, string arguments) : this(filename, arguments, null)
		{
		}

		public ProcessInfo(string filename, string arguments, string workingDirectory)
		{
			startInfo.FileName = filename;
			startInfo.Arguments = arguments;
			startInfo.WorkingDirectory = workingDirectory;
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.RedirectStandardInput = false;
			RepathExecutableIfItIsInWorkingDirectory();
		}

		private void RepathExecutableIfItIsInWorkingDirectory()
		{
			if (WorkingDirectory != null)
			{
				string exectubleInWorkingDirectory = Path.Combine(WorkingDirectory, FileName);
				if (File.Exists(exectubleInWorkingDirectory))
				{
					startInfo.FileName = exectubleInWorkingDirectory;
				}
			}
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

		public string StandardInputContent
		{
			get { return standardInputContent; }
			set
			{
				startInfo.RedirectStandardInput = true;
				startInfo.UseShellExecute = false;
				standardInputContent = value;
			}
		}

		public int TimeOut
		{
			get { return timeout; }
			set { timeout = (value == INFINITE_TIMEOUT) ? 0x7fffffff : value; }
		}

		public Process CreateProcess()
		{
			Process process = new Process();
			process.StartInfo = startInfo;
			return process;
		}

		public override bool Equals(object obj)
		{
			ProcessInfo otherProcessInfo = obj as ProcessInfo;
			if (otherProcessInfo == null)
			{
				return false;
			}

			return (FileName == otherProcessInfo.FileName
				&& Arguments == otherProcessInfo.Arguments
				&& WorkingDirectory == otherProcessInfo.WorkingDirectory
				&& StandardInputContent == otherProcessInfo.StandardInputContent);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("FileName: [{0}] -- Arguments: [{1}] -- WorkingDirectory: [{2}] -- StandardInputContent: [{3}] ",
			                     FileName, Arguments, WorkingDirectory, StandardInputContent);
		}
	}
}