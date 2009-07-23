using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Perform an analysis using NDepend.
    /// </summary>
    [ReflectorType("ndepend")]
    public class NDependTask
        : BaseExecutableTask
    {
        #region Private consts
        private const string defaultExecutable = "NDepend.Console";
        #endregion

        #region Private fields
        private string rootPath;
        private IFileSystem fileSystem;
        private ILogger logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="NDependTask"/>.
        /// </summary>
        public NDependTask()
            : this(new ProcessExecutor(), new SystemIoFileSystem(), new DefaultLogger())
        {
        }

        /// <summary>
        /// Initialise a new <see cref="NDependTask"/> with the injection properties.
        /// </summary>
        /// <param name="executor"></param>
        public NDependTask(ProcessExecutor executor, IFileSystem fileSystem, ILogger logger)
        {
            this.executor = executor;
            this.fileSystem = fileSystem;
            this.logger = logger;
            TimeOut = 600;
            Publish = true;
        }
        #endregion

        #region Public properties
        #region ProjectFile
        /// <summary>
        /// The NDepend project file.
        /// </summary>
        [ReflectorProperty("project")]
        public string ProjectFile { get; set; }
        #endregion

        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
        #endregion

        #region EmitXml
        /// <summary>
        /// Whether to emit the XML report data or not.
        /// </summary>
        [ReflectorProperty("emitXml", Required = false)]
        public bool EmitXml { get; set; }
        #endregion

        #region OutputDir
        /// <summary>
        /// The output directory to use.
        /// </summary>
        [ReflectorProperty("outputDir", Required = false)]
        public string OutputDir { get; set; }
        #endregion

        #region InputDirs
        /// <summary>
        /// The input directories to use.
        /// </summary>
        [ReflectorProperty("inputDirs", Required = false)]
        public string[] InputDirs { get; set; }
        #endregion

        #region Silent
        /// <summary>
        /// Whether to hide any output or not.
        /// </summary>
        [ReflectorProperty("silent", Required = false)]
        public bool Silent { get; set; }
        #endregion

        #region ReportXslt
        /// <summary>
        /// The location of a report XSL-T.
        /// </summary>
        [ReflectorProperty("reportXslt", Required = false)]
        public string ReportXslt { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// The time-out period in seconds.
        /// </summary>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region BaseDirectory
        /// <summary>
        /// The base directory to use.
        /// </summary>
        [ReflectorProperty("baseDir", Required = false)]
        public string BaseDirectory { get; set; }
        #endregion

        #region Publish
        /// <summary>
        /// Whether to publish the output files or not.
        /// </summary>
        [ReflectorProperty("publish", Required = false)]
        public bool Publish { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// The file system that is being used.
        /// </summary>
        public IFileSystem FileSystem
        {
            get { return fileSystem; }
        }
        #endregion

        #region Logger
        /// <summary>
        /// The logger that is being used.
        /// </summary>
        public ILogger Logger
        {
            get { return logger; }
        }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Run the task.
        /// </summary>
        /// <param name="result"></param>
        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Executing NDepend");

            // Make sure there is a root directory
            rootPath = BaseDirectory;
            if (string.IsNullOrEmpty(rootPath)) rootPath = result.WorkingDirectory;

            // Take a before snapshot of all the files
            var outputDirectory = RootPath(OutputDir, false);
            var oldFiles = GenerateOriginalFileList(outputDirectory);

            // Run the executable
			var processResult = TryToRun(CreateProcessInfo(result), result);
            result.AddTaskResult(new ProcessTaskResult(processResult, true));

            if (Publish && !processResult.Failed)
            {
                // Check for any new files
                var newFiles = ListFileDifferences(oldFiles, outputDirectory);

                if (newFiles.Length > 0)
                {
                    logger.Debug("Copying {0} new file(s)", newFiles.Length);

                    // Copy all the new files over
                    var publishDir = Path.Combine(result.BaseFromArtifactsDirectory(result.Label), "NDepend");
                    fileSystem.EnsureFolderExists(publishDir);
                    foreach (var newFile in newFiles)
                    {
                        fileSystem.Copy(newFile, 
                            Path.Combine(publishDir, 
                                Path.GetFileName(newFile)));

                        // Merge all XML files
                        if (Path.GetExtension(newFile) == ".xml")
                        {
                            result.AddTaskResult(fileSystem.GenerateTaskResultFromFile(newFile));
                        }
                    }
                }
            }

            return !processResult.Failed;
        }
        #endregion

        #region GetProcessFilename()
        /// <summary>
        /// Retrieve the executable to use.
        /// </summary>
        /// <returns></returns>
        protected override string GetProcessFilename()
        {
            string path;
            if (string.IsNullOrEmpty(Executable))
            {
                path = RootPath(defaultExecutable, true);
            }
            else
            {
                path = RootPath(Executable, true);
            }
            return path;
        }
        #endregion

        #region GetProcessBaseDirectory()
        /// <summary>
        /// Retrieve the base directory.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            string path = StringUtil.AutoDoubleQuoteString(rootPath);
            return path;
        }
        #endregion

        #region GetProcessTimeout()
        /// <summary>
        /// Get the time-out period.
        /// </summary>
        /// <returns></returns>
        protected override int GetProcessTimeout()
        {
            return TimeOut * 1000;
        }
        #endregion

        #region GetProcessArguments()
        /// <summary>
        /// Retrieve the arguments
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected override string GetProcessArguments(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.Append(RootPath(ProjectFile, true));
            buffer.AppendIf(Silent, "/Silent");
            buffer.AppendIf(EmitXml, "/EmitVisualNDependBinXml ");
            if ((InputDirs != null) && (InputDirs.Length > 0))
            {
                List<string> dirs = new List<string>();
                foreach (string dir in InputDirs)
                {
                    dirs.Add(RootPath(dir, true));
                }
                buffer.AppendArgument("/InDirs {0}", string.Join(" ", dirs.ToArray()));
            }
            buffer.AppendArgument("/OutDir {0}", RootPath(OutputDir, true));
            if (!string.IsNullOrEmpty(ReportXslt))
            {
                buffer.AppendArgument("/XslForReport  {0}", RootPath(ReportXslt, true));
            }
            return buffer.ToString();
        }
        #endregion
        #endregion

        #region Private methods
        #region RootPath()
        /// <summary>
        /// Ensures that a path is rooted.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private string RootPath(string path, bool doubleQuote)
        {
            string actualPath;
            if (Path.IsPathRooted(path))
            {
                actualPath = path;
            }
            else
            {
                if (string.IsNullOrEmpty(path))
                {
                    actualPath = Path.Combine(rootPath, "NDependResults");
                }
                else
                {
                    actualPath = Path.Combine(rootPath, path);
                }
            }
            if (doubleQuote) actualPath = StringUtil.AutoDoubleQuoteString(actualPath);
            return actualPath;
        }
        #endregion

        #region ListFileDifferences()
        /// <summary>
        /// Generate a list of differences in files.
        /// </summary>
        /// <param name="originalList"></param>
        /// <param name="newList"></param>
        /// <returns></returns>
        private string[] ListFileDifferences(Dictionary<string, DateTime> originalList, string outputDirectory)
        {
            string[] newList = {};
            if (fileSystem.DirectoryExists(outputDirectory)) newList = fileSystem.GetFilesInDirectory(outputDirectory);

            var differenceList = new List<string>();

            // For each new file, see if it is in the old file list
            foreach (var newFile in newList)
            {
                if (originalList.ContainsKey(newFile))
                {
                    // Check if the times are different
                    if (originalList[newFile] != fileSystem.GetLastWriteTime(newFile))
                    {
                        differenceList.Add(newFile);
                    }
                }
                else
                {
                    // Not in the old file, therefore it's new
                    differenceList.Add(newFile);
                }
            }

            return differenceList.ToArray();
        }
        #endregion

        #region GenerateOriginalFileList()
        /// <summary>
        /// Generate a list of the original files.
        /// </summary>
        /// <param name="outputDirectory"></param>
        /// <returns></returns>
        private Dictionary<string, DateTime> GenerateOriginalFileList(string outputDirectory)
        {
            var originalFiles = new Dictionary<string, DateTime>();
            if (fileSystem.DirectoryExists(outputDirectory))
            {
                var oldFiles = fileSystem.GetFilesInDirectory(outputDirectory);
                foreach (var oldFile in oldFiles)
                {
                    originalFiles.Add(oldFile, fileSystem.GetLastWriteTime(oldFile));
                }
            }
            return originalFiles;
        }
        #endregion
        #endregion
    }
}
