namespace CruiseControl.Core.Utilities
{
    using System;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using CruiseControl.Core.Interfaces;
    using NLog;

    public class ProcessInfo
    {
        #region Constants
        public const ProcessPriorityClass DefaultPriority = ProcessPriorityClass.Normal;
        public readonly TimeSpan InfiniteTimeout = TimeSpan.MaxValue;
        #endregion

        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly SecureArguments arguments;
        private readonly ProcessPriorityClass priority;
        private readonly ProcessStartInfo startInfo = new ProcessStartInfo();
        private readonly int[] successExitCodes;
        private readonly IFileSystem fileSystem;
        private string standardInputContent;
        private TimeSpan timeout = TimeSpan.FromMinutes(2);
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="filename">The filename.</param>
        public ProcessInfo(IFileSystem fileSystem, string filename) :
            this(fileSystem, filename, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="arguments">The arguments.</param>
        public ProcessInfo(IFileSystem fileSystem, string filename, SecureArguments arguments) :
            this(fileSystem, filename, arguments, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        public ProcessInfo(IFileSystem fileSystem, string filename, SecureArguments arguments, string workingDirectory) :
            this(fileSystem, filename, arguments, workingDirectory, DefaultPriority)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="priority">The priority.</param>
        public ProcessInfo(IFileSystem fileSystem, string filename, SecureArguments arguments, string workingDirectory, ProcessPriorityClass priority) :
            this(fileSystem, filename, arguments, workingDirectory, priority, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessInfo"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="workingDirectory">The working directory.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="successExitCodes">The success exit codes.</param>
        public ProcessInfo(IFileSystem fileSystem, string filename, SecureArguments arguments, string workingDirectory, ProcessPriorityClass priority, int[] successExitCodes)
        {
            if (fileSystem == null)
            {
                throw new ArgumentNullException("fileSystem");
            }

            this.fileSystem = fileSystem;
            this.arguments = arguments;
            this.priority = priority;
            startInfo.FileName = filename.StripQuotes();
            startInfo.Arguments = arguments == null ? null : arguments.ToString(SecureDataMode.Private);
            startInfo.WorkingDirectory = workingDirectory.StripQuotes();
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardInput = false;
            this.RepathExecutableIfItIsInWorkingDirectory();
            this.successExitCodes = successExitCodes ?? new[] { 0 };
        }
        #endregion

        #region Public properties
        #region WorkingDirectory
        /// <summary>
        /// Gets or sets the working directory.	
        /// </summary>
        /// <value>The working directory.</value>
        public string WorkingDirectory
        {
            get { return startInfo.WorkingDirectory; }
            set
            {
                startInfo.WorkingDirectory = value;
                this.RepathExecutableIfItIsInWorkingDirectory();
            }
        }
        #endregion

        #region
        /// <summary>
        /// Gets the name of the file.	
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return startInfo.FileName; }
        }
        #endregion

        #region Arguments
        /// <summary>
        /// Gets the arguments.	
        /// </summary>
        public SecureArguments Arguments
        {
            get { return this.arguments; }
        }
        #endregion

        #region PrivateArguments
        /// <summary>
        /// Gets the private arguments.	
        /// </summary>
        public string PrivateArguments
        {
            get { return startInfo.Arguments; }
        }
        #endregion

        #region PublicArguments
        /// <summary>
        /// Gets the public arguments.	
        /// </summary>
        public string PublicArguments
        {
            get
            {
                return this.arguments == null ? null : this.arguments.ToString();
            }
        }
        #endregion

        #region Priority
        /// <summary>
        /// Gets the priority.
        /// </summary>
        public ProcessPriorityClass Priority
        {
            get { return this.priority; }
        }
        #endregion

        #region SuccessExitCodes
        /// <summary>
        /// Gets the success exit codes.
        /// </summary>
        public int[] SuccessExitCodes
        {
            get { return this.successExitCodes; }
        }
        #endregion

        #region EnvironmentVariables
        /// <summary>
        /// Gets the environment variables.	
        /// </summary>
        public StringDictionary EnvironmentVariables
        {
            get { return startInfo.EnvironmentVariables; }
        }
        #endregion

        #region StreamEncoding
        /// <summary>
        /// Gets or sets the stream encoding.	
        /// </summary>
        /// <value>The stream encoding.</value>
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
        #endregion

        #region StandardInputContent
        /// <summary>
        /// Gets or sets the content of the standard input.	
        /// </summary>
        /// <value>The content of the standard input.</value>
        public string StandardInputContent
        {
            get { return this.standardInputContent; }
            set
            {
                this.startInfo.RedirectStandardInput = true;
                this.startInfo.UseShellExecute = false;
                this.standardInputContent = value;
            }
        }
        #endregion

        #region TimeOut
        /// <summary>
        /// Gets or sets the time out.
        /// </summary>
        /// <value>
        /// The time out.
        /// </value>
        public TimeSpan TimeOut
        {
            get { return this.timeout; }
            set { this.timeout = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region CheckIfSuccess()
        /// <summary>
        /// Checks if an exit code is a success code.
        /// </summary>
        /// <param name="exitCode">The exit code.</param>
        /// <returns>
        /// <c>true</c> if the code is a successful code; <c>false</c> otherwise.
        /// </returns>
        public bool CheckIfSuccess(int exitCode)
        {
            var success = this.successExitCodes.Contains(exitCode);
            return success;
        }
        #endregion

        #region CreateProcess()
        /// <summary>
        /// Creates the process.	
        /// </summary>
        /// <returns>
        /// The new <see cref="Process"/>.
        /// </returns>
        public Process CreateProcess()
        {
            // if WorkingDirectory is filled in, check that it exists
            if (!string.IsNullOrEmpty(WorkingDirectory) &&
                !this.fileSystem.CheckIfDirectoryExists(WorkingDirectory))
            {
                throw new DirectoryNotFoundException("Directory does not exist: " + WorkingDirectory);
            }

            logger.Debug("Creating process for '{0}'", this.FileName);
            var process = new Process { StartInfo = startInfo };
            return process;
        }
        #endregion
        #endregion

        #region Private methods
        #region RepathExecutableIfItIsInWorkingDirectory()
        /// <summary>
        /// Rebases the executable if it is in working directory.
        /// </summary>
        private void RepathExecutableIfItIsInWorkingDirectory()
        {
            var executableInWorkingDirectory = Path.Combine(WorkingDirectory ?? string.Empty, FileName);
            if (this.fileSystem.CheckIfFileExists(executableInWorkingDirectory))
            {
                startInfo.FileName = executableInWorkingDirectory;
            }
        }
        #endregion
        #endregion
    }
}
