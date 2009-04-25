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
        private string projectFile;
        private string description;
        private string executable;
        private bool emitXml;
        private string[] inputDirs;
        private string outputDir;
        private bool silent;
        private string reportXslt;
        private int timeOut = 600;
        private string baseDirectory;
        private string rootPath;
        private bool publish = true;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="NDependTask"/>.
        /// </summary>
        public NDependTask()
            : this(new ProcessExecutor())
        {
        }

        /// <summary>
        /// Initialise a new <see cref="NDependTask"/> with a <see cref="ProcessExecutor"/>.
        /// </summary>
        /// <param name="executor"></param>
        public NDependTask(ProcessExecutor executor)
        {
            this.executor = executor;
        }
        #endregion

        #region Public properties
        #region ProjectFile
        /// <summary>
        /// The NDepend project file.
        /// </summary>
        [ReflectorProperty("project")]
        public string ProjectFile
        {
            get { return projectFile; }
            set { projectFile = value; }
        }
        #endregion

        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        [ReflectorProperty("executable", Required = false)]
        public string Executable
        {
            get { return executable; }
            set { executable = value; }
        }
        #endregion

        #region Description
        /// <summary>
        /// Description used for the visualisation of the buildstage, if left empty the process name will be shown
        /// </summary>
        [ReflectorProperty("description", Required = false)]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }
        #endregion

        #region EmitXml
        /// <summary>
        /// Whether to emit the XML report data or not.
        /// </summary>
        [ReflectorProperty("emitXml", Required = false)]
        public bool EmitXml
        {
            get { return emitXml; }
            set { emitXml = value; }
        }
        #endregion

        #region OutputDir
        /// <summary>
        /// The output directory to use.
        /// </summary>
        [ReflectorProperty("outputDir", Required = false)]
        public string OutputDir
        {
            get { return outputDir; }
            set { outputDir = value; }
        }
        #endregion

        #region InputDirs
        /// <summary>
        /// The input directories to use.
        /// </summary>
        [ReflectorProperty("inputDirs", Required = false)]
        public string[] InputDirs
        {
            get { return inputDirs; }
            set { inputDirs = value; }
        }
        #endregion

        #region Silent
        /// <summary>
        /// Whether to hide any output or not.
        /// </summary>
        [ReflectorProperty("silent", Required = false)]
        public bool Silent
        {
            get { return silent; }
            set { silent = value; }
        }
        #endregion

        #region ReportXslt
        /// <summary>
        /// The location of a report XSL-T.
        /// </summary>
        [ReflectorProperty("reportXslt", Required = false)]
        public string ReportXslt
        {
            get { return reportXslt; }
            set { reportXslt = value; }
        }
        #endregion

        #region TimeOut
        /// <summary>
        /// The time-out period in seconds.
        /// </summary>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut
        {
            get { return timeOut; }
            set { timeOut = value; }
        }
        #endregion

        #region BaseDirectory
        /// <summary>
        /// The base directory to use.
        /// </summary>
        [ReflectorProperty("baseDir", Required = false)]
        public string BaseDirectory
        {
            get { return baseDirectory; }
            set { baseDirectory = value; }
        }
        #endregion

        #region Publish
        /// <summary>
        /// Whether to publish the output files or not.
        /// </summary>
        [ReflectorProperty("publish", Required = false)]
        public bool Publish
        {
            get { return publish; }
            set { publish = value; }
        }
        #endregion
        #endregion

        #region Public methods
        #region Run()
        /// <summary>
        /// Run the task.
        /// </summary>
        /// <param name="result"></param>
        public override void Run(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Executing NDepend");

            // Make sure there is a root directory
            rootPath = baseDirectory;
            if (string.IsNullOrEmpty(rootPath)) rootPath = result.WorkingDirectory;

            // Take a before snapshot of all the files
            DirectoryInfo outputDirectory = new DirectoryInfo(RootPath(outputDir, false));
            Dictionary<string, DateTime> oldFiles = GenerateOriginalFileList(outputDirectory);

            // Run the executable
            ProcessResult processResult = TryToRun(CreateProcessInfo(result));
            result.AddTaskResult(new ProcessTaskResult(processResult));

            if (publish)
            {
                // Check for any new files
                FileInfo[] newFiles = ListFileDifferences(oldFiles, outputDirectory);

                if (newFiles.Length > 0)
                {
                    // Copy all the new files over
                    string publishDir = result.BaseFromArtifactsDirectory(result.Label);
                    if (!Directory.Exists(publishDir)) Directory.CreateDirectory(publishDir);
                    foreach (FileInfo newFile in newFiles)
                    {
                        newFile.CopyTo(Path.Combine(publishDir, newFile.Name));

                        // Merge all XML files
                        if (newFile.Extension == ".xml")
                        {
                            result.AddTaskResult((new FileTaskResult(newFile)));
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Protected methods
        #region GetProcessFilename()
        /// <summary>
        /// Retrieve the executable to use.
        /// </summary>
        /// <returns></returns>
        protected override string GetProcessFilename()
        {
            string path;
            if (string.IsNullOrEmpty(executable))
            {
                path = RootPath(defaultExecutable, true);
            }
            else
            {
                path = RootPath(executable, true);
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
            string path = RootPath(rootPath, true);
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
            return timeOut * 1000;
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
            buffer.Append(RootPath(projectFile, true));
            buffer.AppendIf(silent, "/Silent");
            buffer.AppendIf(emitXml, "/EmitVisualNDependBinXml ");
            if ((inputDirs != null) && (inputDirs.Length > 0))
            {
                List<string> dirs = new List<string>();
                foreach (string dir in inputDirs)
                {
                    dirs.Add(RootPath(dir, true));
                }
                buffer.AppendArgument("/InDirs {0}", string.Join(" ", dirs.ToArray()));
            }
            buffer.AppendArgument("/OutDir {0}", RootPath(outputDir, true));
            if (!string.IsNullOrEmpty(reportXslt))
            {
                buffer.AppendArgument("/XslForReport  {0}", RootPath(reportXslt, true));
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
        private FileInfo[] ListFileDifferences(Dictionary<string, DateTime> originalList, DirectoryInfo outputDirectory)
        {
            FileInfo[] newList = {};
            if (outputDirectory.Exists) newList = outputDirectory.GetFiles();

            List<FileInfo> differenceList = new List<FileInfo>();

            // For each new file, see if it is in the old file list
            foreach (FileInfo newFile in newList)
            {
                if (originalList.ContainsKey(newFile.Name))
                {
                    // Check if the times are different
                    if (originalList[newFile.Name] != newFile.LastWriteTime)
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
        private Dictionary<string, DateTime> GenerateOriginalFileList(DirectoryInfo outputDirectory)
        {
            Dictionary<string, DateTime> originalFiles = new Dictionary<string, DateTime>();
            if (outputDirectory.Exists)
            {
                FileInfo[] oldFiles = outputDirectory.GetFiles();
                foreach (FileInfo oldFile in oldFiles)
                {
                    originalFiles.Add(oldFile.Name, oldFile.LastWriteTime);
                }
            }
            return originalFiles;
        }
        #endregion
        #endregion
    }
}
