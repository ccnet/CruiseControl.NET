namespace CruiseControl.Core.Utilities
{
    using System.Collections.Generic;
    using System.IO;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// ProcessResult holds the results of a Process' execution.  This class is returned from the ProcessExecutor
    /// once the Process has finished executing (terminating either normally or abnormally).  
    /// ProcessResult indicates if the process executed successfully or if it timed out.
    /// It also indicates what the process wrote to its standard output and error streams.
    /// </summary>
    public class ProcessResult
    {
        #region Constants
        public const int SuccessfulExitCode = 0;
        public const int TimedOutExitCode = -1;
        #endregion

        #region Private fields
        private readonly IFileSystem fileSystem;
        private readonly string outputPath;
        private readonly long standardOutputCount;
        private readonly long standardErrorCount;
        private readonly int exitCode;
        private readonly bool timedOut;
        private readonly bool failed;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessResult"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="timedOut">The timed out.</param>
        public ProcessResult(IFileSystem fileSystem, string outputPath, int exitCode, bool timedOut)
            : this(fileSystem, outputPath, exitCode, timedOut, exitCode != SuccessfulExitCode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessResult"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="outputPath">The output path.</param>
        /// <param name="exitCode">The exit code.</param>
        /// <param name="timedOut">The timed out.</param>
        /// <param name="failed">The failed.</param>
        public ProcessResult(IFileSystem fileSystem, string outputPath, int exitCode, bool timedOut, bool failed)
        {
            this.fileSystem = fileSystem;
            this.outputPath = outputPath;
            this.exitCode = exitCode;
            this.timedOut = timedOut;
            this.failed = failed;

            using (var stream = this.fileSystem.OpenFileForRead(this.outputPath))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        switch (line[0])
                        {
                            case 'E':
                                this.standardErrorCount++;
                                break;

                            case 'O':
                                this.standardOutputCount++;
                                break;
                        }
                    }
                }
            }
        }
        #endregion

        #region Public properties
        #region OutputPath
        /// <summary>
        /// Gets the output path.
        /// </summary>
        public string OutputPath
        {
            get { return this.outputPath; }
        }
        #endregion

        #region ExitCode
        /// <summary>
        /// Gets the exit code.	
        /// </summary>
        public int ExitCode
        {
            get { return this.exitCode; }
        }
        #endregion

        #region TimedOut
        /// <summary>
        /// Gets the timed out.	
        /// </summary>
        public bool TimedOut
        {
            get { return this.timedOut; }
        }
        #endregion

        #region Failed
        /// <summary>
        /// Gets the failed.	
        /// </summary>
        public bool Failed
        {
            get { return this.failed; }
        }
        #endregion

        #region Succeeded
        /// <summary>
        /// Returns true if the task completed without failing or timing out.
        /// </summary>
        public bool Succeeded
        {
            get { return !(this.failed || this.timedOut); }
        }
        #endregion

        #region HasErrorOutput
        /// <summary>
        /// Gets the has error output.	
        /// </summary>
        public bool HasErrorOutput
        {
            get { return this.standardErrorCount > 0; }
        }
        #endregion

        #region NumberOfOutputLines
        /// <summary>
        /// Gets the number of output lines.
        /// </summary>
        public long NumberOfOutputLines
        {
            get { return this.standardOutputCount; }
        }
        #endregion

        #region NumberOfErrorLines
        /// <summary>
        /// Gets the number of error lines.
        /// </summary>
        public long NumberOfErrorLines
        {
            get { return this.standardErrorCount; }
        }
        #endregion
        #endregion

        #region Public methods
        #region ReadStandardOutput()
        /// <summary>
        /// Reads the standard output lines.
        /// </summary>
        /// <returns>
        /// The lines from standard output.
        /// </returns>
        public IEnumerable<string> ReadStandardOutput()
        {
            using (var stream = this.fileSystem.OpenFileForRead(this.outputPath))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("O"))
                        {
                            yield return line.Substring(1);
                        }
                    }
                }
            }
        }
        #endregion

        #region ReadStandardError()
        /// <summary>
        /// Reads the standard error lines.
        /// </summary>
        /// <returns>
        /// The lines from standard error.
        /// </returns>
        public IEnumerable<string> ReadStandardError()
        {
            using (var stream = this.fileSystem.OpenFileForRead(this.outputPath))
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("E"))
                        {
                            yield return line.Substring(1);
                        }
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
