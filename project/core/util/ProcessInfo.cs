
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessInfo
	{
		public const int DefaultTimeout = 120000;
		public const int InfiniteTimeout = 0;

        private readonly PrivateArguments arguments;
		private readonly ProcessStartInfo startInfo = new ProcessStartInfo();
		private string standardInputContent;
		private int timeout = DefaultTimeout;

        private readonly int[] successExitCodes;

	    public ProcessInfo(string filename) : 
			this(filename, null, null, null){}

		public ProcessInfo(string filename, PrivateArguments arguments) : 
			this(filename, arguments, null, null){}

        public ProcessInfo(string filename, PrivateArguments arguments, string workingDirectory) : 
			this(filename, arguments, workingDirectory, null){}

        public ProcessInfo(string filename, PrivateArguments arguments, string workingDirectory, int[] successExitCodes)
		{
            this.arguments = arguments;
			startInfo.FileName = StringUtil.StripQuotes(filename);
			startInfo.Arguments = arguments == null ? null : arguments.ToString(SecureDataMode.Private);
			startInfo.WorkingDirectory = StringUtil.StripQuotes(workingDirectory);
			startInfo.UseShellExecute = false;
			startInfo.CreateNoWindow = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;
			startInfo.RedirectStandardInput = false;
			RepathExecutableIfItIsInWorkingDirectory();
            this.successExitCodes = successExitCodes ?? new int[] { 0 };
		}

		private void RepathExecutableIfItIsInWorkingDirectory()
		{
			if (WorkingDirectory == null) 
				return;

			string executableInWorkingDirectory = Path.Combine(WorkingDirectory, FileName);
			if (File.Exists(executableInWorkingDirectory))
				startInfo.FileName = executableInWorkingDirectory;
		}

		public StringDictionary EnvironmentVariables
		{
			get { return startInfo.EnvironmentVariables; }
		}

		public bool ProcessSuccessful(int exitCode)
		{
			return Array.IndexOf(successExitCodes, exitCode) > -1;
		}

		public string FileName
		{
			get { return startInfo.FileName; }
		}

		public string Arguments
		{
            get { return startInfo.Arguments; }
		}

        public string PublicArguments
        {
            get
            {
                if (this.arguments == null)
                {
                    return null;
                }
                else
                {
                    return this.arguments.ToString();
                }
            }
        }

        public string WorkingDirectory
		{
			get { return startInfo.WorkingDirectory; }
			set { startInfo.WorkingDirectory = value; }
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
			set { timeout = (value == InfiniteTimeout) ? 0x7fffffff : value; }
		}

	    public Encoding StreamEncoding
	    {
	        get
	        {
	            return startInfo.StandardOutputEncoding;
	        }
	        set
	        {
	             startInfo.StandardOutputEncoding = value;
	             startInfo.StandardErrorEncoding = value;
	        }
	    }

	    public Process CreateProcess()
		{
			if (!string.IsNullOrEmpty(WorkingDirectory) && !Directory.Exists(WorkingDirectory)) 
				throw new DirectoryNotFoundException("Directory does not exist: " + WorkingDirectory);

			Process process = new Process();
			process.StartInfo = startInfo;
			return process;
		}

		public override bool Equals(object obj)
		{
			ProcessInfo otherProcessInfo = obj as ProcessInfo;
			if (otherProcessInfo == null)
				return false;

			return (FileName == otherProcessInfo.FileName
				&& Arguments == otherProcessInfo.Arguments
				&& WorkingDirectory == otherProcessInfo.WorkingDirectory
				&& TimeOut == otherProcessInfo.TimeOut
				&& StandardInputContent == otherProcessInfo.StandardInputContent);
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public override string ToString()
		{
			return string.Format(
				"FileName: [{0}] -- Arguments: [{1}] -- WorkingDirectory: [{2}] -- StandardInputContent: [{3}] -- Timeout: [{4}]",
			    FileName, Arguments, WorkingDirectory, StandardInputContent, TimeOut);
		}
	}
}
