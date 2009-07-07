using System;
using System.Collections.Generic;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.IO;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// Perform an analysis using NCover 3.0.
    /// </summary>
    [ReflectorType("ncoverProfile")]
    public class NCoverProfileTask
        : BaseExecutableTask
    {
        #region Private consts
        private const string defaultExecutable = "NCover.Console";
        #endregion

        #region Private fields
        private string rootPath;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="NCoverProfileTask"/>.
        /// </summary>
        public NCoverProfileTask()
            : this(new ProcessExecutor())
        {
        }

        /// <summary>
        /// Initialise a new <see cref="NCoverProfileTask"/> with a <see cref="ProcessExecutor"/>.
        /// </summary>
        /// <param name="executor"></param>
        public NCoverProfileTask(ProcessExecutor executor)
        {
            this.executor = executor;
            this.Publish = true;
            this.TimeOut = 600;
            this.LogLevel = NCoverLogLevel.Default;
        }
        #endregion

        #region Public properties
        #region ProgramToCover
        /// <summary>
        /// The program to execute and collect coverage stats from.
        /// </summary>
        [ReflectorProperty("program", Required = false)]
        public string ProgramToCover { get; set; }
        #endregion

        #region TestProject
        /// <summary>
        /// The project that contains the tests.
        /// </summary>
        [ReflectorProperty("testProject", Required = false)]
        public string TestProject { get; set; }
        #endregion

        #region ProgramParameters
        /// <summary>
        /// The parameters to pass to the program.
        /// </summary>
        [ReflectorProperty("programParameters", Required = false)]
        public string ProgramParameters { get; set; }
        #endregion

        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
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

        #region WorkingDirectory
        /// <summary>
        /// The working directory to use.
        /// </summary>
        [ReflectorProperty("workingDir", Required = false)]
        public string WorkingDirectory { get; set; }
        #endregion

        #region Publish
        /// <summary>
        /// Whether to publish the output files or not.
        /// </summary>
        [ReflectorProperty("publish", Required = false)]
        public bool Publish { get; set; }
        #endregion

        #region LogFile
        /// <summary>
        /// The location of the NCover log file.
        /// </summary>
        [ReflectorProperty("logFile", Required = false)]
        public string LogFile { get; set; }
        #endregion

        #region LogLevel
        /// <summary>
        /// The profiler log level.
        /// </summary>
        [ReflectorProperty("logLevel", Required = false)]
        public NCoverLogLevel LogLevel { get; set; }
        #endregion

        #region ProjectName
        /// <summary>
        /// The name of the project (used in the HTML report).
        /// </summary>
        [ReflectorProperty("projectName", Required = false)]
        public string ProjectName { get; set; }
        #endregion

        #region CoverageFile
        /// <summary>
        /// The location to write the coverage file to.
        /// </summary>
        [ReflectorProperty("coverageFile", Required = false)]
        public string CoverageFile { get; set; }
        #endregion

        #region CoverageMetric
        /// <summary>
        /// The coverage metric to use.
        /// </summary>
        [ReflectorProperty("coverageMetric", Required = false)]
        public string CoverageMetric { get; set; }
        #endregion

        #region ExcludedAttributes
        /// <summary>
        /// The attributes to exclude.
        /// </summary>
        [ReflectorProperty("excludedAttributes", Required = false)]
        public string ExcludedAttributes { get; set; }
        #endregion

        #region ExcludedAssemblies
        /// <summary>
        /// The assemblies to exclude.
        /// </summary>
        [ReflectorProperty("excludedAssemblies", Required = false)]
        public string ExcludedAssemblies { get; set; }
        #endregion

        #region ExcludedFiles
        /// <summary>
        /// The files to exclude.
        /// </summary>
        [ReflectorProperty("excludedFiles", Required = false)]
        public string ExcludedFiles { get; set; }
        #endregion

        #region ExcludedMethods
        /// <summary>
        /// The methods to exclude.
        /// </summary>
        [ReflectorProperty("excludedMethods", Required = false)]
        public string ExcludedMethods { get; set; }
        #endregion

        #region ExcludedTypes
        /// <summary>
        /// The types to exclude.
        /// </summary>
        [ReflectorProperty("excludedTypes", Required = false)]
        public string ExcludedTypes { get; set; }
        #endregion

        #region IncludedAttributes
        /// <summary>
        /// The attributes to include.
        /// </summary>
        [ReflectorProperty("includedAttributes", Required = false)]
        public string IncludedAttributes { get; set; }
        #endregion

        #region IncludedAssemblies
        /// <summary>
        /// The assemblies to include.
        /// </summary>
        [ReflectorProperty("includedAssemblies", Required = false)]
        public string IncludedAssemblies { get; set; }
        #endregion

        #region IncludedFiles
        /// <summary>
        /// The files to include.
        /// </summary>
        [ReflectorProperty("includedFiles", Required = false)]
        public string IncludedFiles { get; set; }
        #endregion

        #region IncludedTypes
        /// <summary>
        /// The types to include.
        /// </summary>
        [ReflectorProperty("includedTypes", Required = false)]
        public string IncludedTypes { get; set; }
        #endregion

        #region DisableAutoexclusion
        /// <summary>
        /// Whether to turn off autoexclusion or not.
        /// </summary>
        [ReflectorProperty("disableAutoexclusion", Required = false)]
        public bool DisableAutoexclusion { get; set; }
        #endregion

        #region ProcessModule
        /// <summary>
        /// The module to process.
        /// </summary>
        [ReflectorProperty("processModule", Required = false)]
        public string ProcessModule { get; set; }
        #endregion

        #region SymbolSearch
        /// <summary>
        /// The symbol search policy to use.
        /// </summary>
        [ReflectorProperty("symbolSearch", Required = false)]
        public string SymbolSearch { get; set; }
        #endregion

        #region TrendFile
        /// <summary>
        /// The location to write the trend file to.
        /// </summary>
        [ReflectorProperty("trendFile", Required = false)]
        public string TrendFile { get; set; }
        #endregion

        #region BuildId
        /// <summary>
        /// A custom build id to attach.
        /// </summary>
        [ReflectorProperty("buildId", Required = false)]
        public string BuildId { get; set; }
        #endregion

        #region SettingsFile
        /// <summary>
        /// The location to read the settings from.
        /// </summary>
        [ReflectorProperty("settingsFile", Required = false)]
        public string SettingsFile { get; set; }
        #endregion

        #region Register
        /// <summary>
        /// Temporarily enable NCover.
        /// </summary>
        [ReflectorProperty("register", Required = false)]
        public bool Register { get; set; }
        #endregion

        #region ApplicationLoadWait
        /// <summary>
        /// The amount of time that NCover will wait for the application to start up.
        /// </summary>
        [ReflectorProperty("applicationLoadWait", Required = false)]
        public int ApplicationLoadWait { get; set; }
        #endregion

        #region CoverIis
        /// <summary>
        /// Whether to cover IIS or not.
        /// </summary>
        [ReflectorProperty("iis", Required = false)]
        public bool CoverIis { get; set; }
        #endregion

        #region ServiceTimeout
        /// <summary>
        /// The timeout period for covering a service.
        /// </summary>
        [ReflectorProperty("serviceTimeout", Required = false)]
        public int ServiceTimeout { get; set; }
        #endregion

        #region WindowsService
        /// <summary>
        /// The windows service to cover.
        /// </summary>
        [ReflectorProperty("windowsService", Required = false)]
        public string WindowsService { get; set; }
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
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Running NCover profile");

            // Make sure there is a root directory
            rootPath = BaseDirectory;
            if (string.IsNullOrEmpty(rootPath)) rootPath = result.WorkingDirectory;

            // Run the executable
			var processResult = TryToRun(CreateProcessInfo(result), result);
            result.AddTaskResult(new ProcessTaskResult(processResult));

            if (Publish && !processResult.Failed)
            {
                var coverageFile = string.IsNullOrEmpty(CoverageFile) ? "coverage.xml" : CoverageFile;
                result.AddTaskResult(new FileTaskResult(RootPath(coverageFile, false)));
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
            string path = string.IsNullOrEmpty(WorkingDirectory) ? RootPath(rootPath, true) : RootPath(WorkingDirectory, true);
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
            buffer.Append(RootPath(ProgramToCover, true));
            if (!string.IsNullOrEmpty(TestProject))
            {
                string testProject;
                if (!string.IsNullOrEmpty(WorkingDirectory))
                {
                    testProject = Path.Combine(RootPath(WorkingDirectory, false), TestProject);
                    testProject = StringUtil.AutoDoubleQuoteString(testProject);
                }
                else
                {
                    testProject = RootPath(TestProject, true);
                }
                buffer.AppendArgument(testProject);
            }
            buffer.AppendArgument(ProgramParameters);

            // Add all the NCover arguments
            buffer.AppendIf(!string.IsNullOrEmpty(LogFile), "//l \"{0}\"", RootPath(LogFile, false));
            buffer.AppendIf(LogLevel != NCoverLogLevel.Default, "//ll {0}", LogLevel.ToString());
            buffer.AppendIf(!string.IsNullOrEmpty(ProjectName), "//p \"{0}\"", ProjectName);
            buffer.AppendIf(!string.IsNullOrEmpty(CoverageFile), "//x \"{0}\"", RootPath(CoverageFile, false));
            buffer.AppendIf(string.IsNullOrEmpty(CoverageFile), "//x \"{0}\"", RootPath("Coverage.xml", false));
            buffer.AppendIf(!string.IsNullOrEmpty(CoverageMetric), "//ct \"{0}\"", CoverageMetric);
            buffer.AppendIf(!string.IsNullOrEmpty(ExcludedAttributes), "//ea \"{0}\"", ExcludedAttributes);
            buffer.AppendIf(!string.IsNullOrEmpty(ExcludedAssemblies), "//eas \"{0}\"", ExcludedAssemblies);
            buffer.AppendIf(!string.IsNullOrEmpty(ExcludedFiles), "//ef \"{0}\"", ExcludedFiles);
            buffer.AppendIf(!string.IsNullOrEmpty(ExcludedMethods), "//em \"{0}\"", ExcludedMethods);
            buffer.AppendIf(!string.IsNullOrEmpty(ExcludedTypes), "//et \"{0}\"", ExcludedTypes);
            buffer.AppendIf(!string.IsNullOrEmpty(IncludedAttributes), "//ia \"{0}\"", IncludedAttributes);
            buffer.AppendIf(!string.IsNullOrEmpty(IncludedAssemblies), "//ias \"{0}\"", IncludedAssemblies);
            buffer.AppendIf(!string.IsNullOrEmpty(IncludedFiles), "//if \"{0}\"", IncludedFiles);
            buffer.AppendIf(!string.IsNullOrEmpty(IncludedTypes), "//it \"{0}\"", IncludedTypes);
            buffer.AppendIf(DisableAutoexclusion, "//na");
            buffer.AppendIf(!string.IsNullOrEmpty(ProcessModule), "//pm \"{0}\"", ProcessModule);
            buffer.AppendIf(!string.IsNullOrEmpty(SymbolSearch), "//ssp \"{0}\"", SymbolSearch);
            buffer.AppendIf(!string.IsNullOrEmpty(TrendFile), "//at \"{0}\"", RootPath(TrendFile, false));
            buffer.AppendArgument("//bi \"{0}\"", !string.IsNullOrEmpty(BuildId) ? BuildId : result.Label);
            buffer.AppendIf(!string.IsNullOrEmpty(SettingsFile), "//cr \"{0}\"", RootPath(SettingsFile, false));
            buffer.AppendIf(Register, "//reg");
            buffer.AppendIf(!string.IsNullOrEmpty(WorkingDirectory), "//w \"{0}\"", RootPath(WorkingDirectory, false));
            buffer.AppendIf(ApplicationLoadWait > 0, "//wal {0}", ApplicationLoadWait.ToString());
            buffer.AppendIf(CoverIis, "//iis");
            buffer.AppendIf(ServiceTimeout > 0, "//st {0}", ServiceTimeout.ToString());
            buffer.AppendIf(!string.IsNullOrEmpty(WindowsService), "//svc {0}", WindowsService);

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
                    actualPath = Path.Combine(rootPath, "NCoverResults");
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
        #endregion

        #region Enumerations
        #region NCoverLogLevel
        /// <summary>
        /// The allowed logging levels.
        /// </summary>
        public enum NCoverLogLevel
        {
            Default,
            None,
            Normal,
            Verbose
        }
        #endregion
        #endregion
    }
}
