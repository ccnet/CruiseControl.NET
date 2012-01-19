namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// Updates the configuration for the server.
    /// </summary>
    /// <title>Update Configuration Task</title>
    /// <version>1.6</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;updateConfig /&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;updateConfig /&gt;
    /// </code>
    /// </example>
    [ReflectorType("updateConfig")]
    public class UpdateConfigurationTask
        : BaseExecutableTask
    {
        #region Private fields
        private string fileToValidate;
        private string validationLogFile;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConfigurationTask"/> class.
        /// </summary>
        public UpdateConfigurationTask()
            : this(new ProcessExecutor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateConfigurationTask"/> class.
        /// </summary>
        /// <param name="executor">The executor to use.</param>
        public UpdateConfigurationTask(ProcessExecutor executor)
        {
            this.executor = executor;
            this.TimeOut = 600;
            this.Priority = ProcessPriorityClass.Normal;
            this.ValidateFile = true;
        }
        #endregion

        #region Public properties
        #region SourceFile
        /// <summary>
        /// The source of the configuration file.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("source")]
        public string SourceFile { get; set; }
        #endregion

        #region AlwaysUpdate
        /// <summary>
        /// Whether to always update the file, even if there are no changes.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("always", Required=false)]
        public bool AlwaysUpdate { get; set; }
        #endregion

        #region ValidateFile
        /// <summary>
        /// Whether to validate the file before it is applied or not.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("validate", Required = false)]
        public bool ValidateFile { get; set; }
        #endregion

        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>ccvalidator</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
        #endregion

        #region Priority
        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// The time-out period in seconds.
        /// </summary>
        /// <version>1.5</version>
        /// <default>600</default>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
        public IFileSystem FileSystem { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>
        /// <c>true</c> if the task was successful; <c>false</c> otherwise.
        /// </returns>
        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description)
                                                                   ? Description
                                                                   : "Updating configuration");
            var fileSystem = this.FileSystem ?? new SystemIoFileSystem();

            // Check if the config file exists
            var configFile = this.FindConfigFile(result, fileSystem);
            if (configFile == null)
            {
                return true;
            }

            // Check for any differences
            var currentFile = PathUtils.ConfigFileLocation;
            var updateFile = this.AlwaysUpdate || HasDifferences(fileSystem, configFile, currentFile);
            if (!updateFile)
            {
                Log.Info("Skipping validation and copy - nothing has changed");
                return true;
            }

            // Validate the file
            if (this.ValidateFile)
            {
                this.fileToValidate = configFile;
                this.validationLogFile = Path.GetTempFileName();
                Log.Info("Executing CCValidator");
                var info = this.CreateProcessInfo(result);
                var processResult = this.TryToRun(info, result);
                if (processResult.TimedOut)
                {
                    Log.Warning("CCValidator timed out");
                    result.AddTaskResult(MakeTimeoutBuildResult(info));
                    return false;
                }

                // Check the results
                result.AddTaskResult(new ProcessTaskResult(processResult, false));
                result.AddTaskResult(fileSystem.GenerateTaskResultFromFile(this.validationLogFile, true));
                if (processResult.Failed)
                {
                    Log.Warning("CCValidator failed");
                    return false;
                }
            }

            // Update the file
            Log.Info("Copying config file to '{0}'", currentFile);
            fileSystem.Copy(configFile, currentFile);
            return true;
        }
        #endregion

        #region GetProcessFilename()
        /// <summary>
        /// Gets the process filename.
        /// </summary>
        /// <returns></returns>
        protected override string GetProcessFilename()
        {
            return this.Executable ?? 
                Path.Combine(Environment.CurrentDirectory, "ccvalidator");
        }
        #endregion

        #region GetProcessArguments()
        /// <summary>
        /// Gets the process arguments.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected override string GetProcessArguments(IIntegrationResult result)
        {
            var builder = new ProcessArgumentBuilder();
            builder.Append("\"" + this.fileToValidate + "\"");
            builder.AppendArgument("-n");
            builder.AppendArgument("-f=x");
            builder.AppendArgument("-l=\"{0}\"", this.validationLogFile);
            return builder.ToString();
        }
        #endregion

        #region GetProcessBaseDirectory()
        /// <summary>
        /// Gets the process base directory.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            var path = result.WorkingDirectory;
            return path;
        }
        #endregion

        #region GetProcessPriorityClass()
        /// <summary>
        /// Gets the process priority class.
        /// </summary>
        /// <returns></returns>
        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return this.Priority;
        }
        #endregion

        #region GetProcessTimeout()
        /// <summary>
        /// Gets the process timeout.
        /// </summary>
        /// <returns></returns>
        protected override int GetProcessTimeout()
        {
            return this.TimeOut * 1000;
        }
        #endregion
        #endregion

        #region Private methods
        #region FindConfigFile()
        /// <summary>
        /// Finds the config file.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <returns>
        /// The path to the file if it exists; <c>null</c> otherwise.
        /// </returns>
        private string FindConfigFile(IIntegrationResult result, IFileSystem fileSystem)
        {
            // Generate the initial file to check
            var configFile = this.SourceFile;
            if (!Path.IsPathRooted(configFile))
            {
                configFile = result.BaseFromWorkingDirectory(configFile);
            }

            // Check if th file exists
            Log.Debug("Checking for configuration file '{0}'", configFile);
            var fileExists = fileSystem.FileExists(configFile);
            if (!fileExists && !configFile.EndsWith("ccnet.config", StringComparison.OrdinalIgnoreCase))
            {
                // Try adding ccnet.config and see if it exists
                configFile = Path.Combine(configFile, "ccnet.config");
                Log.Debug("Checking for configuration file '{0}'", configFile);
                fileExists = fileSystem.FileExists(configFile);
            }

            if (!fileExists)
            {
                Log.Info("Unable to find find config file");
                return null;
            }

            Log.Info("Found config file '{0}'", configFile);
            return configFile;
        }
        #endregion

        #region HasDifferences()
        /// <summary>
        /// Determines whether the specified config file has differences.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="newFile">The new file.</param>
        /// <param name="oldFile">The old file.</param>
        /// <returns>
        ///   <c>true</c> if the specified config file has differences; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasDifferences(IFileSystem fileSystem, string newFile, string oldFile)
        {
            Log.Debug("Checking if new file is different from current");
            var hashProvider = new SHA1CryptoServiceProvider();
            if (!fileSystem.FileExists(oldFile))
            {
                Log.Info("No current file");
                return true;
            }

            var newHash = GenerateFileHash(fileSystem, newFile, hashProvider);
            var oldHash = GenerateFileHash(fileSystem, oldFile, hashProvider);
            if (newHash.Length != oldHash.Length)
            {
                Log.Info("New file has differences");
                return true;
            }

            if (newHash.Where((t, loop) => t != oldHash[loop]).Any())
            {
                Log.Info("New file has differences");
                return true;
            }

            Log.Info("No differences found");
            return false;
        }
        #endregion

        #region GenerateFileHash
        /// <summary>
        /// Generates the file hash.
        /// </summary>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="filePath">The file path.</param>
        /// <param name="hashProvider">The hash provider.</param>
        /// <returns>
        /// An SHA1 hash for the file.
        /// </returns>
        private static byte[] GenerateFileHash(IFileSystem fileSystem, string filePath, HashAlgorithm hashProvider)
        {
            using (var stream = fileSystem.OpenInputStream(filePath))
            {
                var hash = hashProvider.ComputeHash(stream);
                return hash;
            }
        }
        #endregion
        #endregion
    }
}
