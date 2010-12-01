namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System.Diagnostics;
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Config;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// <para>
    /// Profiles the performance of an application using Reg Gate's ANTS Performance Profiler.
    /// </para>
    /// <para>
    /// ANTS Performance Profiler is a tool to profile the performance of an application.
    /// </para>
    /// <para>
    /// This application is available from http://www.red-gate.com/. Pro edition of 1.6 or later is required.
    /// </para>
    /// </summary>
    /// <title>ANTS Performance Profiler Task</title>
    /// <version>1.6</version>
    /// <example>
    /// <code title="Minimalist example">
    /// &lt;antsPerformance&gt;
    /// &lt;app&gt;someapp.exe&lt;/app&gt;
    /// &lt;/antsPerformance&gt;
    /// </code>
    /// </example>
    [ReflectorType("antsPerformance")]
    public class AntsPerformanceProfilerTask
        : BaseExecutableTask
    {
        #region Private consts
        /// <summary>
        /// The default executable to use.
        /// </summary>
        private const string defaultExecutable = "Profile";

        /// <summary>Default priority class</summary>
        private const ProcessPriorityClass DefaultPriority = ProcessPriorityClass.Normal;

        /// <summary>
        /// The default output file.
        /// </summary>
        private const string defaultOutput = "AntsPerformanceAnalysis.txt";
        #endregion

        #region Private fields
        private string rootPath;
        private IFileSystem fileSystem;
        private ILogger logger;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="AntsPerformanceProfilerTask"/>.
        /// </summary>
        public AntsPerformanceProfilerTask()
            : this(new ProcessExecutor(), new SystemIoFileSystem(), new DefaultLogger())
        {
        }

        /// <summary>
        /// Initialise a new <see cref="AntsPerformanceProfilerTask"/> with the injection properties.
        /// </summary>
        /// <param name="executor"></param>
        /// <param name="fileSystem"></param>
        /// <param name="logger"></param>
        public AntsPerformanceProfilerTask(ProcessExecutor executor, IFileSystem fileSystem, ILogger logger)
        {
            this.executor = executor;
            this.fileSystem = fileSystem;
            this.logger = logger;
            this.TimeOut = 600;
            this.ProfilerTimeOut = 120;
            this.ForceOverwrite = true;
            this.Priority = ProcessPriorityClass.Normal;
            this.TraceLevelValue = TraceLevel.Method;
            this.IncludeSource = true;
            this.AllowInlining = true;
            this.Compensate = true;
            this.SimplifyStackTraces = true;
            this.AvoidTrivial = true;
            this.PublishFiles = true;
        }
        #endregion

        #region Public properties
        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <version>1.6</version>
        /// <default>Profile</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
        #endregion

        #region Application
        /// <summary>
        /// The application to profile.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("app", Required = false)]
        public string Application { get; set; }
        #endregion

        #region Service
        /// <summary>
        /// The name of the windows service to profile.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("service", Required = false)]
        public string Service { get; set; }
        #endregion

        #region ComPlus
        /// <summary>
        /// The name of the COM+ service to profile.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("comPlus", Required = false)]
        public string ComPlus { get; set; }
        #endregion

        #region Silverlight
        /// <summary>
        /// The URL of a site containing a silverlight application to profile.
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("silverlight", Required = false)]
        public string Silverlight { get; set; }
        #endregion

        #region ApplicationArguments
        /// <summary>
        /// The arguments to pass to the application. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("appArgs", Required = false)]
        public string ApplicationArguments { get; set; }
        #endregion

        #region Priority
        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.6</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// The time-out period in seconds for the entire task.
        /// </summary>
        /// <version>1.6</version>
        /// <default>600</default>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region ProfilerTimeOut
        /// <summary>
        /// The time-out period in seconds for the profiler.
        /// </summary>
        /// <version>1.6</version>
        /// <default>120</default>
        [ReflectorProperty("profilerTimeout", Required = false)]
        public int ProfilerTimeOut { get; set; }
        #endregion

        #region Quiet
        /// <summary>
        /// Whether to disable all output or not.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("quiet", Required = false)]
        public bool Quiet { get; set; }
        #endregion

        #region Verbose
        /// <summary>
        /// Whether to display verbose output or not.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("verbose", Required = false)]
        public bool Verbose { get; set; }
        #endregion

        #region ForceOverwrite
        /// <summary>
        /// Whether to overwrite any existing files or not.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("overwrite", Required = false)]
        public bool ForceOverwrite { get; set; }
        #endregion

        #region IncludeSubProcesses
        /// <summary>
        /// Whether to include sub-processes.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("subProcesses", Required = false)]
        public bool IncludeSubProcesses { get; set; }
        #endregion

        #region TraceLevelValue
        /// <summary>
        /// The level to trace at.
        /// </summary>
        /// <version>1.6</version>
        /// <default>Method</default>
        [ReflectorProperty("level", Required = false)]
        public TraceLevel TraceLevelValue { get; set; }
        #endregion

        #region MethodLevel
        /// <summary>
        /// Perform method level profiling.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("methodLevel", Required = false)]
        public bool MethodLevel { get; set; }
        #endregion

        #region OnlyWithSource
        /// <summary>
        /// Only profile methods that have source code.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("onlyWithSource", Required = false)]
        public bool OnlyWithSource { get; set; }
        #endregion

        #region UseSampling
        /// <summary>
        /// Whether to use sampling for generating approximate results quickly.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("useSampling", Required = false)]
        public bool UseSampling { get; set; }
        #endregion

        #region IncludeSource
        /// <summary>
        /// Whether to include source code in the results.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("includeSource", Required = false)]
        public bool IncludeSource { get; set; }
        #endregion

        #region AllowInlining
        /// <summary>
        /// Whether to allow .NET to inline functions.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("inlining", Required = false)]
        public bool AllowInlining { get; set; }
        #endregion

        #region Compensate
        /// <summary>
        /// Whether to get the profiler to compensate for its own overhead.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("compensate", Required = false)]
        public bool Compensate { get; set; }
        #endregion

        #region SimplifyStackTraces
        /// <summary>
        /// Whether to simplify certain complicated stack traces.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("simplify", Required = false)]
        public bool SimplifyStackTraces { get; set; }
        #endregion

        #region AvoidTrivial
        /// <summary>
        /// Whether to avoid trivial functions or not.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        [ReflectorProperty("avoidTrivial", Required = false)]
        public bool AvoidTrivial { get; set; }
        #endregion

        #region RecordSqlAndIO
        /// <summary>
        /// Whether to try to record SQL and File I/O events.
        /// </summary>
        /// <version>1.6</version>
        /// <default>false</default>
        [ReflectorProperty("sqlIO", Required = false)]
        public bool RecordSqlAndIO { get; set; }
        #endregion

        #region PublishFiles
        /// <summary>
        /// Whether to publish all files generated from this task.
        /// </summary>
        /// <version>1.6</version>
        /// <default>true</default>
        /// <remarks>
        /// If the files are not published then they will not be viewable in the dashboard unless
        /// copied over by another mechanism.
        /// </remarks>
        [ReflectorProperty("publish", Required = false)]
        public bool PublishFiles { get; set; }
        #endregion

        #region BaseDirectory
        /// <summary>
        /// The base directory to use. If omitted this will default to the working directory of the project. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("baseDir", Required = false)]
        public string BaseDirectory { get; set; }
        #endregion

        #region XmlArgsFile
        /// <summary>
        /// A file containing the args for the profiler in an XML specification. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("xmlArgs", Required = false)]
        public string XmlArgsFile { get; set; }
        #endregion

        #region SummaryCsvFile
        /// <summary>
        /// The location to write the summary file to - uses CSV format. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("summaryScv", Required = false)]
        public string SummaryCsvFile { get; set; }
        #endregion

        #region SummaryXmlFile
        /// <summary>
        /// The location to write the summary file to - uses XML format. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("summaryXml", Required = false)]
        public string SummaryXmlFile { get; set; }
        #endregion

        #region SummaryHtmlFile
        /// <summary>
        /// The location to write the summary file to - uses HTML format. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("summaryHtml", Required = false)]
        public string SummaryHtmlFile { get; set; }
        #endregion

        #region CallTreeXmlFile
        /// <summary>
        /// The location to write the calltree file to - uses XML format. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("callTreeXml", Required = false)]
        public string CallTreeXmlFile { get; set; }
        #endregion

        #region CallTreeHtmlFile
        /// <summary>
        /// The location to write the calltree file to - uses HTML format. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("callTreeHtml", Required = false)]
        public string CallTreeHtmlFile { get; set; }
        #endregion

        #region DataFile
        /// <summary>
        /// The location to write the data file to (requires desktop application to view).
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("data", Required = false)]
        public string DataFile { get; set; }
        #endregion

        #region OutputFile
        /// <summary>
        /// The output analysis file. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>AntsPerformanceAnalysis.txt</default>
        [ReflectorProperty("output", Required = false)]
        public string OutputFile { get; set; }
        #endregion

        #region Threshold
        /// <summary>
        /// The threshold level. 
        /// </summary>
        /// <version>1.6</version>
        /// <default>none</default>
        [ReflectorProperty("threshold", Required = false)]
        public int? Threshold { get; set; }
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

        #region Public methods
        #region Validate()
        /// <summary>
        /// Checks the internal validation of the item.
        /// </summary>
        /// <param name="configuration">The entire configuration.</param>
        /// <param name="parent">The parent item for the item being validated.</param>
        /// <param name="errorProcesser">The error processer to use.</param>
        public override void Validate(IConfiguration configuration, ConfigurationTrace parent, IConfigurationErrorProcesser errorProcesser)
        {
            base.Validate(configuration, parent, errorProcesser);
            if (this.Verbose && this.Quiet)
            {
                errorProcesser.ProcessError("Cannot have both verbose and quiet set");
            }

            if (this.ProfilerTimeOut < 0)
            {
                errorProcesser.ProcessError("profilerTimeout cannot be negative");
            }
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
            this.logger.Debug("Starting ANTS Performance Profiler task");
            result.BuildProgressInformation
                    .SignalStartRunTask(!string.IsNullOrEmpty(this.Description) ? this.Description : "Executing ANTS Performance Profiler");

            // Make sure there is a root directory
            this.rootPath = this.BaseDirectory;
            if (string.IsNullOrEmpty(this.rootPath))
            {
                this.rootPath = result.WorkingDirectory;
            }

            // Run the executable
            var info = this.CreateProcessInfo(result);
            var processResult = this.TryToRun(info, result);
            result.AddTaskResult(new ProcessTaskResult(processResult, false));
            if (processResult.TimedOut)
            {
                result.AddTaskResult(MakeTimeoutBuildResult(info));
            }

            // Publish the results
            if (this.PublishFiles) // && !processResult.Failed) - TODO: only publish files if successful
            {
                var publishDir = Path.Combine(result.BaseFromArtifactsDirectory(result.Label), "AntsPerformance");
                this.PublishFile(string.IsNullOrEmpty(this.OutputFile) ? defaultOutput : this.OutputFile, publishDir);
                this.PublishFile(this.SummaryCsvFile, publishDir);
                this.PublishFile(this.SummaryXmlFile, publishDir);
                this.PublishFile(this.SummaryHtmlFile, publishDir);
                this.PublishFile(this.CallTreeXmlFile, publishDir);
                this.PublishFile(this.CallTreeHtmlFile, publishDir);
                this.PublishFile(this.DataFile, publishDir);
            }

            return processResult.Succeeded;
        }
        #endregion

        #region
        /// <summary>
        /// Gets the valid success codes.
        /// </summary>
        /// <returns>The valid success codes.</returns>
        /// <remarks>
        /// Due to a bug in the profiler this returns 1. According to the documentation this is a
        /// general failure, so other errors may occur and be falsely missed.
        /// </remarks>
        protected override int[] GetProcessSuccessCodes()
        {
            var fileName = this.GetProcessFilename();
            var fileExists = this.FileSystem.FileExists(fileName);
            if (!fileExists)
            {
                fileName += ".exe";
                fileExists = this.FileSystem.FileExists(fileName);
            }

            if (fileExists)
            {
                var version = this.FileSystem.GetFileVersion(fileName);
                Logger.Debug("Profiler version is " + version);

                // TODO: When Red Gate fix the profiler, add an override here to return null for the
                // newer versions
            }

            return new[] { 1 };
        }
        #endregion

        #region GetProcessFilename()
        /// <summary>
        /// Retrieve the executable to use.
        /// </summary>
        /// <returns></returns>
        protected override string GetProcessFilename()
        {
            var path = string.IsNullOrEmpty(this.Executable) ? defaultExecutable : this.Executable;
            path = this.RootPath(path, false);
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
            return this.rootPath;
        }
        #endregion

        #region GetProcessTimeout()
        /// <summary>
        /// Get the time-out period.
        /// </summary>
        /// <returns></returns>
        protected override int GetProcessTimeout()
        {
            return this.TimeOut * 1000;
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
            var buffer = new ProcessArgumentBuilder();

            buffer.AppendArgument("/wd:{0}", this.QuoteSpaces(this.rootPath));
            buffer.AppendArgument("/out:{0}", this.RootPath(string.IsNullOrEmpty(this.OutputFile) ? defaultOutput : this.OutputFile, true));
            buffer.AppendArgument("/t:{0}", this.ProfilerTimeOut.ToString());
            switch (this.TraceLevelValue)
            {
                case TraceLevel.Line:
                    buffer.AppendArgument("/ll");
                    break;

                case TraceLevel.Method:
                    buffer.AppendArgument("/ml");
                    break;
            }

            buffer.AppendIf(this.ForceOverwrite, "/f");
            buffer.AppendIf(this.OnlyWithSource, "/ows");
            buffer.AppendIf(this.UseSampling, "/sm");
            buffer.AppendIf(this.IncludeSubProcesses, "/sp");
            buffer.AppendIf(this.RecordSqlAndIO, "/rs");
            buffer.AppendArgument("/is:{0}", GenerateOnOff(this.IncludeSource));
            buffer.AppendArgument("/in:{0}", GenerateOnOff(this.AllowInlining));
            buffer.AppendArgument("/comp:{0}", GenerateOnOff(this.Compensate));
            buffer.AppendArgument("/simp:{0}", GenerateOnOff(this.SimplifyStackTraces));
            buffer.AppendArgument("/notriv:{0}", GenerateOnOff(this.AvoidTrivial));
            buffer.AppendIf(!string.IsNullOrEmpty(this.Service), "/service:{0}", this.QuoteSpaces(this.Service));
            buffer.AppendIf(!string.IsNullOrEmpty(this.ComPlus), "/complus:{0}", this.QuoteSpaces(this.ComPlus));
            buffer.AppendIf(!string.IsNullOrEmpty(this.Silverlight), "/silverlight:{0}", this.QuoteSpaces(this.Silverlight));
            buffer.AppendIf(!string.IsNullOrEmpty(this.Application), "/e:{0}", this.RootPath(this.Application, true));
            buffer.AppendIf(!string.IsNullOrEmpty(this.ApplicationArguments), "/args:{0}", this.ApplicationArguments);
            buffer.AppendIf(this.Quiet, "/q");
            buffer.AppendIf(this.Verbose, "/v");
            buffer.AppendIf(!string.IsNullOrEmpty(this.XmlArgsFile), "/argfile:{0}", this.RootPath(this.XmlArgsFile, true));
            buffer.AppendIf(this.Threshold.HasValue, "/threshold:{0}", this.Threshold.ToString());
            buffer.AppendIf(!string.IsNullOrEmpty(this.SummaryCsvFile), "/csv:{0}", this.RootPath(this.SummaryCsvFile, true));
            buffer.AppendIf(!string.IsNullOrEmpty(this.SummaryXmlFile), "/xml:{0}", this.RootPath(this.SummaryXmlFile, true));
            buffer.AppendIf(!string.IsNullOrEmpty(this.SummaryHtmlFile), "/h:{0}", this.RootPath(this.SummaryHtmlFile, true));
            buffer.AppendIf(!string.IsNullOrEmpty(this.CallTreeXmlFile), "/calltree:{0}", this.RootPath(this.CallTreeXmlFile, true));
            buffer.AppendIf(!string.IsNullOrEmpty(this.CallTreeHtmlFile), "/cth:{0}", this.RootPath(this.CallTreeHtmlFile, true));
            buffer.AppendIf(!string.IsNullOrEmpty(this.DataFile), "/data:{0}", this.RootPath(this.DataFile, true));

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
        #region GenerateOnOff()
        /// <summary>
        /// Generates a text string containing either on or off.
        /// </summary>
        /// <param name="value">The value indicating whether this is on or off.</param>
        /// <returns>"on" if value is <c>true</c>; "off" otherwise.</returns>
        private static string GenerateOnOff(bool value)
        {
            return value ? "on" : "off";
        }
        #endregion

        #region PublishFile()
        private void PublishFile(string fileName, string publishDir)
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                var fullPath = this.RootPath(fileName, false);
                if (this.fileSystem.FileExists(fullPath))
                {
                    var newPath = Path.Combine(publishDir, Path.GetFileName(fullPath));
                    fileSystem.Copy(fullPath, newPath);
                }
            }
        }
        #endregion

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
                    actualPath = Path.Combine(basePath, "ANTSPerformanceResults");
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

        #region QuoteSpaces()
        /// <summary>
        /// Adds quotes to a string if it contains spaces.
        /// </summary>
        /// <param name="value">The string to check.</param>
        /// <returns>The string with quotes if needed.</returns>
        protected string QuoteSpaces(string value)
        {
            if ((value != null) && 
                value.Contains(" ") && 
                !value.StartsWith("\"") && 
                !value.EndsWith("\""))
            {
                return "\"" + value + "\"";
            }
            else
            {
                return value;
            }
        }
        #endregion
        #endregion

        #region Public enums
        #region TraceLevel
        /// <summary>
        /// The trace level to use.
        /// </summary>
        public enum TraceLevel
        {
            /// <summary>
            /// Trace at the line level.
            /// </summary>
            Line,

            /// <summary>
            /// Trace at the method level.
            /// </summary>
            Method
        }
        #endregion
        #endregion
    }
}
