using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
	public class ProcessResult
	{
		private string standardOutput;
		private string standardError;
		private int errorCode;
		private bool timedOut;

		public ProcessResult(string standardOutput, string standardError, int errorCode, bool timedOut)
		{
			this.standardOutput = standardOutput;
			this.standardError = standardError;
			this.errorCode = errorCode;
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
			get { return errorCode; }
		}

		public bool TimedOut
		{
			get { return timedOut; }
		}

		public bool HasError
		{
			get { return standardError != null && standardError.Trim() != string.Empty; }	
		}
	}
}