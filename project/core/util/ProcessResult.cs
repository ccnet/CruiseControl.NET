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
		private readonly int errorCode;
		private readonly bool timedOut;
		private readonly bool failed;

		public ProcessResult(string standardOutput, string standardError, int errorCode, bool timedOut)
			: this(standardOutput, standardError, errorCode, timedOut, errorCode != SUCCESSFUL_EXIT_CODE)
		{
		}

		public ProcessResult(string standardOutput, string standardError, int errorCode, bool timedOut, bool failed)
		{
			this.standardOutput = (standardOutput ?? "");
			this.standardError = (standardError ?? "");
			this.errorCode = errorCode;
			this.timedOut = timedOut;
			this.failed = failed;
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
			get { return errorCode; }
		}

		public bool TimedOut
		{
			get { return timedOut; }
		}

		public bool Failed
		{
			get { return failed; }	
		}

		public bool HasErrorOutput
		{
			get { return standardError.Trim() != string.Empty; }
		}
	}
}
