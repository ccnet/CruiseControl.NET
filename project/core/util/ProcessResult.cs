using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	/// <summary>
	/// ProcessResult holds the results of a Process' execution.  This class is returned from the ProcessExecutor
	/// once the Process has finished executing (teriminating either normally or abnormally).  
	/// ProcessResult indicates if the process executed successfully or if it timed out.
	/// It also indicates what the process wrote to its standard output and error streams.
	/// </summary>
	public class ProcessResult
	{
		public const int SUCCESSFUL_EXIT_CODE = 0;
		public const int TIMED_OUT_EXIT_CODE = -1;

		private readonly string standardOutput;
		private readonly string standardError;
		private readonly int exitCode;
		private readonly bool timedOut;

		public ProcessResult(string standardOutput, string standardError, int errorCode, bool timedOut)
		{
			this.standardOutput = (standardOutput == null ? "" : standardOutput);
			this.standardError = (standardError == null ? "" : standardError);
			this.exitCode = errorCode;
			this.timedOut = timedOut;
		}

		public string StandardOutput
		{
			get { return standardOutput; }
		}

		public string StandardError
		{
			get { return standardError; }
		}

		public int ExitCode
		{
			get { return exitCode; }
		}

		public bool TimedOut
		{
			get { return timedOut; }
		}

		/// <summary>
		/// A non-zero exit code is the best indication of a process' success or failure.  Not all applications adhere to this, however.
		/// Applications may write to stderr even if the process succeeds.
		/// </summary>
		public bool Failed
		{
			get { return exitCode != SUCCESSFUL_EXIT_CODE; }	
		}

		public bool HasErrorOutput
		{
			get { return standardError.Trim() != string.Empty; }
		}
	}
}