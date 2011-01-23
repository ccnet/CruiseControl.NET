namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Xml.Linq;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// Runs an NDepend analysis.
    /// </para>
    /// <para>
    /// NDepend is a tool that simplifies managing a complex .NET code base. Architects and developers can analyze code structure, specify
    /// design rules, plan massive refactoring, do effective code reviews and master evolution by comparing different versions of the code.
    /// </para>
    /// <para>
    /// This application is available from www.ndepend.com. There is both an open source/academic/evaluation version and a professional
    /// version.
    /// </para>
    /// </summary>
    /// <title>NDepend Task</title>
    /// <version>1.4.4</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;ndepend&gt;
    /// &lt;project&gt;NDepend-Project.xml&lt;/project&gt;
    /// &lt;/ndepend&gt;
    /// </code>
    /// <code title="Full example">
    /// &lt;ndepend&gt;
    /// &lt;project&gt;NDepend-Project.xml&lt;/project&gt;
    /// &lt;executable&gt;tools\NDepend.Console.exe&lt;/executable&gt;
    /// &lt;description&gt;Run the NDepend analysis.&lt;/description&gt;
    /// &lt;emitXml&gt;true&lt;/emitXml&gt;
    /// &lt;outputDir&gt;NDepend-Reports&lt;/outputDir&gt;
    /// &lt;inputDirs&gt;
    /// &lt;inputDir&gt;bin\&lt;/inputDir&gt;
    /// &lt;inputDir&gt;deploy\&lt;/inputDir&gt;
    /// &lt;/inputDirs&gt;
    /// &lt;silent&gt;false&lt;/silent&gt;
    /// &lt;reportXslt&gt;custom-report.xsl&lt;/reportXslt&gt;
    /// &lt;timeout&gt;120&lt;/timeout&gt;
    /// &lt;baseDir&gt;project\&lt;/baseDir&gt;
    /// &lt;publish&gt;true&lt;/publish&gt;
    /// &lt;/ndepend&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This task works in two stages:
    /// </para>
    /// <para>
    /// 1. Run the NDepend executable
    /// </para>
    /// <para>
    /// 2. Publish the results
    /// </para>
    /// <para>
    /// Running the NDepend executable is what generates the actual analysis, and as such cannot be skipped (otherwise there is no point to
    /// this task.) The results of the analysis will be saved in the folder specified by outputDir. If this parameter is omitted, then the
    /// results will be stored in a folder called NDependResults under the baseDir.
    /// </para>
    /// <para>
    /// In order for these results to be displayed in the dashboard they must be stored in a folder in the artefacts directory. To achieve
    /// this, this task will publish the results. This involves copying all the results files from the output directory to a folder in the
    /// artefacts directory. This folder will have the same name as the build label. Additionally any XML files will be merged with the build
    /// log (this makes them available for the dashboard plugins).
    /// </para>
    /// <para>
    /// If the publishing behaviour is not required it can be turned off by setting the publish property to false. By default this is set to
    /// true so the results can be displayed in the dashboard.
    /// </para>
    /// </remarks>
    [ReflectorType("ndepend")]
    public class NDependTask
        : BaseExecutableTask
    {
        #region Private consts
        private const string defaultExecutable = "NDepend.Console";

        /// <summary>Default priority class</summary>
        private const ProcessPriorityClass DefaultPriority = ProcessPriorityClass.Normal;
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
        /// <param name="fileSystem"></param>
        /// <param name="logger"></param>
        public NDependTask(ProcessExecutor executor, IFileSystem fileSystem, ILogger logger)
        {
            this.executor = executor;
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.TimeOut = 600;
            this.Publish = true;
            this.Priority = ProcessPriorityClass.Normal;
        }
        #endregion

        #region Public properties
        #region ProjectFile
        /// <summary>
        /// The NDepend project file. This is generated from VisualNDepend. 
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>n/a</default>
        [ReflectorProperty("project")]
        public string ProjectFile { get; set; }
        #endregion

        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>NDepend.Console</default>
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

        #region EmitXml
        /// <summary>
        /// Whether to emit the XML report data or not.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("emitXml", Required = false)]
        public bool EmitXml { get; set; }
        #endregion

        #region OutputDir
        /// <summary>
        /// The output directory to use.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>NDependResults</default>
        [ReflectorProperty("outputDir", Required = false)]
        public string OutputDir { get; set; }
        #endregion

        #region InputDirs
        /// <summary>
        /// The input directories to use.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>None</default>
        [ReflectorProperty("inputDirs", Required = false)]
        public string[] InputDirs { get; set; }
        #endregion

        #region Silent
        /// <summary>
        /// Whether to hide any output or not.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("silent", Required = false)]
        public bool Silent { get; set; }
        #endregion

        #region ReportXslt
        /// <summary>
        /// The location of a custom report XSL-T.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>None</default>
        [ReflectorProperty("reportXslt", Required = false)]
        public string ReportXslt { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// The time-out period in seconds.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>false</default>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region BaseDirectory
        /// <summary>
        /// The base directory to use. If omitted this will default to the working directory of the project. 
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("baseDir", Required = false)]
        public string BaseDirectory { get; set; }
        #endregion

        #region Publish
        /// <summary>
        /// Whether to publish the output files or not.
        /// </summary>
        /// <version>1.4.4</version>
        /// <default>true</default>
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
            var info = this.CreateProcessInfo(result);
            var processResult = this.TryToRun(info, result);
            result.AddTaskResult(new ProcessTaskResult(processResult, false));
            if (processResult.TimedOut)
            {
                result.AddTaskResult(MakeTimeoutBuildResult(info));
            }
            
            if (Publish && processResult.Succeeded)
            {
                // Check for any new files
                var newFiles = ListFileDifferences(oldFiles, outputDirectory);

                if (newFiles.Length > 0)
                {
                    logger.Debug("Copying {0} new file(s)", newFiles.Length);

                    // Copy all the new files over
                    var publishDir = Path.Combine(result.BaseFromArtifactsDirectory(result.Label), "NDepend");
                    var oldLen = outputDirectory.Length + 1;
                    var lastDir = string.Empty;
                    foreach (var newFile in newFiles)
                    {
                        // Ensure the directory exists
                        var newPath = Path.Combine(publishDir, newFile.Substring(oldLen));
                        var newDir = Path.GetDirectoryName(newPath);
                        if (lastDir != newDir)
                        {
                            lastDir = newDir;
                            if (!fileSystem.DirectoryExists(newDir))
                            {
                                fileSystem.CreateDirectory(newDir);
                            }
                        }

                        // Copy the file and marge it if XML
                        if (fileSystem.FileExists(newFile))
                        {
                            fileSystem.Copy(newFile, newPath);
                            if (Path.GetExtension(newFile) == ".xml")
                            {
                                result.AddTaskResult(fileSystem.GenerateTaskResultFromFile(newFile));
                            }
                        }
                        else
                        {
                            logger.Warning("Unable to find file: " + newFile);
                        }
                    }
                }
            }

            return processResult.Succeeded;
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
                path = RootPath(defaultExecutable, false);
            }
            else
            {
                path = RootPath(Executable, false);
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
            return rootPath;
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

        #region GetProcessPriorityClass()
        /// <summary>
        /// Gets the requested priority class value for this Task.
        /// </summary>
        protected override ProcessPriorityClass GetProcessPriorityClass()
        {
            return this.Priority;
        }
        #endregion
        #endregion

        #region Private methods
        #region RootPath()
        /// <summary>
        /// Ensures that a path is rooted.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="doubleQuote"></param>
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
                var basePath = rootPath ?? this.BaseDirectory ?? string.Empty;
                if (string.IsNullOrEmpty(path))
                {
                    actualPath = Path.Combine(basePath, "NDependResults");
                }
                else
                {
                    actualPath = Path.Combine(basePath, path);
                }
            }

            if (doubleQuote)
            {
                actualPath = StringUtil.AutoDoubleQuoteString(actualPath);
            }

            return actualPath;
        }
        #endregion

        #region ListFileDifferences()
        /// <summary>
        /// Generate a list of differences in files.
        /// </summary>
        /// <param name="originalList"></param>
        ///<param name="outputDirectory"></param>
        /// <returns></returns>
        private string[] ListFileDifferences(Dictionary<string, DateTime> originalList, string outputDirectory)
        {
            var contentsFile = Path.Combine(outputDirectory, "ReportResources.xml");
            if (fileSystem.FileExists(contentsFile))
            {
                using (var reader = fileSystem.Load(contentsFile))
                {
                    XDocument document = null;
                    try
                    {
                        document = XDocument.Load(reader);
                    }
                    catch (Exception error)
                    {
                        throw new CruiseControlException(
                            "Unable to load contents manifest - " + error.Message ?? string.Empty,
                            error);
                    }

                    var rootEl = document.Element("ReportResources");
                    if (rootEl == null)
                    {
                        throw new CruiseControlException("Unable to load contents manifest - unable to find root node");
                    }

                    var files = new List<string>();
                    foreach (var fileEl in rootEl.Elements("File"))
                    {
                        files.Add(Path.Combine(outputDirectory, fileEl.Value));
                    }

                    foreach (var folder in rootEl.Elements("Directory"))
                    {
                        var fullPath = Path.Combine(outputDirectory, folder.Value);
                        files.AddRange(fileSystem.GetFilesInDirectory(fullPath, true));
                    }

                    return files.ToArray();
                }
            }
            else
            {
                string[] newList = { };
                if (fileSystem.DirectoryExists(outputDirectory))
                {
                    newList = fileSystem.GetFilesInDirectory(outputDirectory);
                }

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
