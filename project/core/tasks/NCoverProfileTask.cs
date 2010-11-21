using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// Perform a code coverage profile using NCover.
    /// </para>
    /// <para type="tip">
    /// NCover is a commerical application that will profile code while unit tests are running. The tool is available from
    /// http://www.ncover.com/.
    /// </para>
    /// <para type="info">
    /// <title>Supported Versions</title>
    /// CruiseControl.NET only supports NCover 3.x currently.
    /// </para>
    /// </summary>
    /// <title>NCover Profiler Task</title>
    /// <version>1.5</version>
    /// <example>
    /// <code>
    /// &lt;ncoverProfile&gt;
    /// &lt;executable&gt;C:\Program Files\NCover\NCover.Console.exe&lt;/executable&gt;
    /// &lt;program&gt;tools\nunit\nunit-console.exe&lt;/program&gt;
    /// &lt;testProject&gt;myproject.test.dll&lt;/testProject&gt;
    /// &lt;workingDir&gt;build\unittests&lt;/workingDir&gt;
    /// &lt;includedAssemblies&gt;myproject.*.dll&lt;/includedAssemblies&gt;
    /// &lt;/ncoverProfile&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This task calls NCover.Console to perform the profiling. Full details on this tool is available at
    /// http://docs.ncover.com/ref/3-0/ncover-console/. Additional details on the mapped arguments can be found there.
    /// </para>
    /// </remarks>
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
            this.Priority = ProcessPriorityClass.Normal;
        }
        #endregion

        #region Public properties
        #region ProgramToCover
        /// <summary>
        /// The program to execute and collect coverage stats from.
        /// </summary>
        /// <version>1.5</version>
        /// <default>n/a</default>
        [ReflectorProperty("program")]
        public string ProgramToCover { get; set; }
        #endregion

        #region TestProject
        /// <summary>
        /// The project that contains the tests. If relative, this will be relative to baseDir. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("testProject", Required = false)]
        public string TestProject { get; set; }
        #endregion

        #region ProgramParameters
        /// <summary>
        /// The parameters to pass to the program.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("programParameters", Required = false)]
        public string ProgramParameters { get; set; }
        #endregion

        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Ncover.Console</default>
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
        /// The time-out period in seconds. If the task does no finish running in this time it will be terminated. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>600</default>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region BaseDirectory
        /// <summary>
        /// The base directory to use. All relative parameters will be relative to this parameter. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>Project Working Directory</default>
        [ReflectorProperty("baseDir", Required = false)]
        public string BaseDirectory { get; set; }
        #endregion

        #region WorkingDirectory
        /// <summary>
        /// The working directory to use. If relative, this will be relative to baseDir.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //w</b>
        /// </remarks>
        [ReflectorProperty("workingDir", Required = false)]
        public string WorkingDirectory { get; set; }
        #endregion

        #region Publish
        /// <summary>
        /// Whether to publish the output files or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>true</default>
        [ReflectorProperty("publish", Required = false)]
        public bool Publish { get; set; }
        #endregion

        #region LogFile
        /// <summary>
        /// The location of the NCover log file. If relative, this will be relative to baseDir.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //l</b>
        /// </remarks>
        [ReflectorProperty("logFile", Required = false)]
        public string LogFile { get; set; }
        #endregion

        #region LogLevel
        /// <summary>
        /// The profiler log level.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Default</default>
        /// <remarks>
        /// <b>Maps to //ll</b>
        /// </remarks>
        [ReflectorProperty("logLevel", Required = false)]
        public NCoverLogLevel LogLevel { get; set; }
        #endregion

        #region ProjectName
        /// <summary>
        /// The name of the project (used in the HTML report).
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //p</b>
        /// </remarks>
        [ReflectorProperty("projectName", Required = false)]
        public string ProjectName { get; set; }
        #endregion

        #region CoverageFile
        /// <summary>
        /// The location to write the coverage file to. If relative, this will be relative to baseDir.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Coverage.xml</default>
        /// <remarks>
        /// <b>Maps to //x</b>
        /// </remarks>
        [ReflectorProperty("coverageFile", Required = false)]
        public string CoverageFile { get; set; }
        #endregion

        #region CoverageMetric
        /// <summary>
        /// The coverage metric to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //ct</b>
        /// </remarks>
        [ReflectorProperty("coverageMetric", Required = false)]
        public string CoverageMetric { get; set; }
        #endregion

        #region ExcludedAttributes
        /// <summary>
        /// The attributes to exclude.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //ea</b>
        /// </remarks>
        [ReflectorProperty("excludedAttributes", Required = false)]
        public string ExcludedAttributes { get; set; }
        #endregion

        #region ExcludedAssemblies
        /// <summary>
        /// The assemblies to exclude.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //eas</b>
        /// </remarks>
        [ReflectorProperty("excludedAssemblies", Required = false)]
        public string ExcludedAssemblies { get; set; }
        #endregion

        #region ExcludedFiles
        /// <summary>
        /// The files to exclude.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //ef</b>
        /// </remarks>
        [ReflectorProperty("excludedFiles", Required = false)]
        public string ExcludedFiles { get; set; }
        #endregion

        #region ExcludedMethods
        /// <summary>
        /// The methods to exclude.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //em</b>
        /// </remarks>
        [ReflectorProperty("excludedMethods", Required = false)]
        public string ExcludedMethods { get; set; }
        #endregion

        #region ExcludedTypes
        /// <summary>
        /// The types to exclude.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //et</b>
        /// </remarks>
        [ReflectorProperty("excludedTypes", Required = false)]
        public string ExcludedTypes { get; set; }
        #endregion

        #region IncludedAttributes
        /// <summary>
        /// The attributes to include.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //ia</b>
        /// </remarks>
        [ReflectorProperty("includedAttributes", Required = false)]
        public string IncludedAttributes { get; set; }
        #endregion

        #region IncludedAssemblies
        /// <summary>
        /// The assemblies to include.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //ias</b>
        /// </remarks>
        [ReflectorProperty("includedAssemblies", Required = false)]
        public string IncludedAssemblies { get; set; }
        #endregion

        #region IncludedFiles
        /// <summary>
        /// The files to include.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //if</b>
        /// </remarks>
        [ReflectorProperty("includedFiles", Required = false)]
        public string IncludedFiles { get; set; }
        #endregion

        #region IncludedTypes
        /// <summary>
        /// The types to include.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //it</b>
        /// </remarks>
        [ReflectorProperty("includedTypes", Required = false)]
        public string IncludedTypes { get; set; }
        #endregion

        #region DisableAutoexclusion
        /// <summary>
        /// Whether to turn off autoexclusion or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        /// <remarks>
        /// <b>Maps to //na</b>
        /// </remarks>
        [ReflectorProperty("disableAutoexclusion", Required = false)]
        public bool DisableAutoexclusion { get; set; }
        #endregion

        #region ProcessModule
        /// <summary>
        /// The module to process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //pm</b>
        /// </remarks>
        [ReflectorProperty("processModule", Required = false)]
        public string ProcessModule { get; set; }
        #endregion

        #region SymbolSearch
        /// <summary>
        /// The symbol search policy to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //ssp</b>
        /// </remarks>
        [ReflectorProperty("symbolSearch", Required = false)]
        public string SymbolSearch { get; set; }
        #endregion

        #region TrendFile
        /// <summary>
        /// The location to write the trend file to.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //at</b>
        /// </remarks>
        [ReflectorProperty("trendFile", Required = false)]
        public string TrendFile { get; set; }
        #endregion

        #region BuildId
        /// <summary>
        /// A custom build id to attach.
        /// </summary>
        /// <version>1.5</version>
        /// <default>The build label</default>
        /// <remarks>
        /// <b>Maps to //bi</b>
        /// </remarks>
        [ReflectorProperty("buildId", Required = false)]
        public string BuildId { get; set; }
        #endregion

        #region SettingsFile
        /// <summary>
        /// The location to read the settings from. If relative, this will be relative to baseDir.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //cr</b>
        /// </remarks>
        [ReflectorProperty("settingsFile", Required = false)]
        public string SettingsFile { get; set; }
        #endregion

        #region Register
        /// <summary>
        /// Temporarily enable NCover.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        /// <remarks>
        /// <b>Maps to //reg</b>
        /// </remarks>
        [ReflectorProperty("register", Required = false)]
        public bool Register { get; set; }
        #endregion

        #region ApplicationLoadWait
        /// <summary>
        /// The amount of time that NCover will wait for the application to start up.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //wal</b>
        /// </remarks>
        [ReflectorProperty("applicationLoadWait", Required = false)]
        public int ApplicationLoadWait { get; set; }
        #endregion

        #region CoverIis
        /// <summary>
        /// Whether to cover IIS or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        /// <remarks>
        /// <b>Maps to //iis</b>
        /// </remarks>
        [ReflectorProperty("iis", Required = false)]
        public bool CoverIis { get; set; }
        #endregion

        #region ServiceTimeout
        /// <summary>
        /// The timeout period for covering a service.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //st</b>
        /// </remarks>
        [ReflectorProperty("serviceTimeout", Required = false)]
        public int ServiceTimeout { get; set; }
        #endregion

        #region WindowsService
        /// <summary>
        /// The windows service to cover.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //svc</b>
        /// </remarks>
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
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            Default,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            None,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            Normal,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            Verbose
        }
        #endregion
        #endregion
    }
}
