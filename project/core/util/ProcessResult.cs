
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
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const int SUCCESSFUL_EXIT_CODE = 0;
        /// <summary>
        /// 	
        /// </summary>
        /// <remarks></remarks>
		public const int TIMED_OUT_EXIT_CODE = -1;

		private readonly string standardOutput;
		private readonly string standardError;
		private readonly int errorCode;
		private readonly bool timedOut;
		private readonly bool failed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessResult" /> class.	
        /// </summary>
        /// <param name="standardOutput">The standard output.</param>
        /// <param name="standardError">The standard error.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="timedOut">The timed out.</param>
        /// <remarks></remarks>
		public ProcessResult(string standardOutput, string standardError, int errorCode, bool timedOut)
			: this(standardOutput, standardError, errorCode, timedOut, errorCode != SUCCESSFUL_EXIT_CODE)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessResult" /> class.	
        /// </summary>
        /// <param name="standardOutput">The standard output.</param>
        /// <param name="standardError">The standard error.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="timedOut">The timed out.</param>
        /// <param name="failed">The failed.</param>
        /// <remarks></remarks>
		public ProcessResult(string standardOutput, string standardError, int errorCode, bool timedOut, bool failed)
		{
			this.standardOutput = (standardOutput ??string.Empty);
			this.standardError = (standardError ??string.Empty);
			this.errorCode = errorCode;
			this.timedOut = timedOut;
			this.failed = failed;
		}

        /// <summary>
        /// Gets the standard output.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string StandardOutput
		{
			get { return standardOutput; }
		}

        /// <summary>
        /// Gets the standard error.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public string StandardError
		{
			get { return standardError; }
		}

        /// <summary>
        /// Gets the exit code.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public int ExitCode
		{
			get { return errorCode; }
		}

        /// <summary>
        /// Gets the timed out.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public bool TimedOut
		{
			get { return timedOut; }
		}

        /// <summary>
        /// Gets the failed.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public bool Failed
		{
			get { return failed; }	
		}

		/// <summary>
		/// Returns true if the task completed without failing or timing out.
		/// </summary>
		/// <value></value>
		/// <remarks></remarks>
		public bool Succeeded
		{
			get { return !(failed || timedOut); }
		}

        /// <summary>
        /// Gets the has error output.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		public bool HasErrorOutput
		{
			get { return !(standardError.Trim() != null && standardError.Trim().Length == 0); }
		}
	}
}
