using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Util;
using System.Globalization;

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    /// <summary>
    /// <para>
    /// Generate a code coverage report using NCover.
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
    /// <title>NCover Report Task</title>
    /// <version>1.5</version>
    /// <example>
    /// <code title="To produce a summary report with a minimum coverage of 95%">
    /// &lt;ncoverReport&gt;
    /// &lt;executable&gt;C:\Program Files\NCover\NCover.Reporting.exe&lt;/executable&gt;
    /// &lt;outputDir&gt;ncover\reports&lt;/outputDir&gt;
    /// &lt;reports&gt;
    /// &lt;report&gt;Summary&lt;/report&gt;
    /// &lt;/reports&gt;
    /// &lt;minimumThresholds&gt;
    /// &lt;coverageThreshold metric="SymbolCoverage" value="95"/&gt;
    /// &lt;/minimumThresholds&gt;
    /// &lt;/ncoverReport&gt;
    /// </code>
    /// <code title="To generate a full report that is ordered by coverage percentage in a descending order">
    /// &lt;ncoverReport&gt;
    /// &lt;executable&gt;C:\Program Files\NCover\NCover.Reporting.exe&lt;/executable&gt;
    /// &lt;outputDir&gt;ncover\reports&lt;/outputDir&gt;
    /// &lt;reports&gt;
    /// &lt;report&gt;FullCoverageReport&lt;/report&gt;
    /// &lt;/reports&gt;
    /// &lt;sortBy&gt;CoveragePercentageDescending&lt;/sortBy&gt;
    /// &lt;/ncoverReport&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This task requires a profile to be completed before running. See the <link>NCover Profiler Task</link>.
    /// </para>
    /// <para>
    /// This task calls NCover.Reporting to generate the reports. Full details on this tool is available at
    /// http://docs.ncover.com/ref/3-0/ncover-reporting/. Additional details on the mapped arguments can be found there.
    /// </para>
    /// </remarks>
    [ReflectorType("ncoverReport")]
    public class NCoverReportTask
        : BaseExecutableTask
    {
        #region Private consts
        private const string defaultExecutable = "NCover.Reporting";
        #endregion

        #region Private fields
        private string rootPath;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialise a new <see cref="NCoverReportTask"/>.
        /// </summary>
        public NCoverReportTask()
            : this(new ProcessExecutor())
        {
        }

        /// <summary>
        /// Initialise a new <see cref="NCoverReportTask"/> with a <see cref="ProcessExecutor"/>.
        /// </summary>
        /// <param name="executor"></param>
        public NCoverReportTask(ProcessExecutor executor)
        {
            this.executor = executor;
            this.XmlReportFilter = NCoverReportFilter.Default;
            this.TimeOut = 600;
            this.NumberToReport = -1;
            this.SortBy = NCoverSortBy.None;
            this.MergeMode = NCoverMergeMode.Default;
            this.Priority = ProcessPriorityClass.Normal;
        }
        #endregion

        #region Public properties
        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>NCover.Reporting</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
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
        /// The working directory for the executable. If relative, this will be relative to baseDir.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //w</b>
        /// </remarks>
        [ReflectorProperty("workingDir", Required = false)]
        public string WorkingDirectory { get; set; }
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

        #region CoverageFile
        /// <summary>
        /// The location to read the coverage date from. If relative, this will be relative to baseDir.
        /// </summary>
        /// <version>1.5</version>
        /// <default>coverage.xml</default>
        [ReflectorProperty("coverageFile", Required = false)]
        public string CoverageFile { get; set; }
        #endregion

        #region ClearCoverageFilters
        /// <summary>
        /// Should the coverage filters be cleared.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        /// <remarks>
        /// <b>Maps to //ccf</b>
        /// </remarks>
        [ReflectorProperty("clearFilters", Required = false)]
        public bool ClearCoverageFilters { get; set; }
        #endregion

        #region CoverageFilters
        /// <summary>
        /// The filters to apply.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //cf</b>
        /// </remarks>
        [ReflectorProperty("filters", Required = false)]
        public CoverageFilter[] CoverageFilters { get; set; }
        #endregion

        #region MinimumThresholds
        /// <summary>
        /// The minimum coverage thresholds.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //mc</b>
        /// </remarks>
        [ReflectorProperty("minimumThresholds", Required = false)]
        public CoverageThreshold[] MinimumThresholds { get; set; }
        #endregion

        #region UseMinimumCoverage
        /// <summary>
        /// Whether to use minimum coverage or not.
        /// </summary>
        /// <version>1.5</version>
        /// <default>false</default>
        /// <remarks>
        /// <b>Maps to //mcsc</b>
        /// </remarks>
        [ReflectorProperty("minimumCoverage", Required = false)]
        public bool UseMinimumCoverage { get; set; }
        #endregion

        #region XmlReportFilter
        /// <summary>
        /// The type of report filtering to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Default</default>
        /// <remarks>
        /// <b>Maps to //rdf</b>
        /// </remarks>
        [ReflectorProperty("xmlReportFilter", Required = false)]
        public NCoverReportFilter XmlReportFilter { get; set; }
        #endregion

        #region SatisfactoryThresholds
        /// <summary>
        /// The satisfactory coverage thresholds.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //sct</b>
        /// </remarks>
        [ReflectorProperty("satisfactory", Required = false)]
        public CoverageThreshold[] SatisfactoryThresholds { get; set; }
        #endregion

        #region NumberToReport
        /// <summary>
        /// The maximum number of items to report.
        /// </summary>
        /// <version>1.5</version>
        /// <default>-1</default>
        /// <remarks>
        /// <b>Maps to //smf</b>
        /// </remarks>
        [ReflectorProperty("numberToReport", Required = false)]
        public int NumberToReport { get; set; }
        #endregion

        #region TrendOutputFile
        /// <summary>
        /// The file to append the trend to.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //at</b>
        /// </remarks>
        [ReflectorProperty("trendOutput", Required = false)]
        public string TrendOutputFile { get; set; }
        #endregion

        #region TrendInputFile
        /// <summary>
        /// The file to import the trend from.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //lt</b>
        /// </remarks>
        [ReflectorProperty("trendInput", Required = false)]
        public string TrendInputFile { get; set; }
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

        #region HideElements
        /// <summary>
        /// The elements to hide.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //hi</b>
        /// </remarks>
        [ReflectorProperty("hide", Required = false)]
        public string HideElements { get; set; }
        #endregion

        #region OutputDir
        /// <summary>
        /// The directory to output the reports to. If relative, this will be relative to baseDir. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("outputDir", Required = false)]
        public string OutputDir { get; set; }
        #endregion

        #region Reports
        /// <summary>
        /// The type of report to generate.
        /// </summary>
        /// <version>1.5</version>
        /// <default>FullCoverageReport</default>
        /// <remarks>
        /// <b>Maps to //or</b>
        /// </remarks>
        [ReflectorProperty("reports", Required = false)]
        public NCoverReportType[] Reports { get; set; }
        #endregion

        #region ProjectName
        /// <summary>
        /// The project name to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //p</b>
        /// </remarks>
        [ReflectorProperty("projectName", Required = false)]
        public string ProjectName { get; set; }
        #endregion

        #region SortBy
        /// <summary>
        /// The sort order to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //so</b>
        /// </remarks>
        [ReflectorProperty("sortBy", Required = false)]
        public NCoverSortBy SortBy { get; set; }
        #endregion

        #region TopUncoveredAmount
        /// <summary>
        /// The amount of uncovered items to cover.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //tu</b>
        /// </remarks>
        [ReflectorProperty("uncoveredAmount", Required = false)]
        public int TopUncoveredAmount { get; set; }
        #endregion

        #region MergeMode
        /// <summary>
        /// The merge mode to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Default</default>
        /// <remarks>
        /// <b>Maps to //mfm</b>
        /// </remarks>
        [ReflectorProperty("mergeMode", Required = false)]
        public NCoverMergeMode MergeMode { get; set; }
        #endregion

        #region MergeFile
        /// <summary>
        /// The file to store the merged data in. If relative, this will be relative to baseDir. 
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        /// <remarks>
        /// <b>Maps to //s</b>
        /// </remarks>
        [ReflectorProperty("mergeFile", Required = false)]
        public string MergeFile { get; set; }
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
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Running NCover reporting");

            // Make sure there is a root directory
            rootPath = BaseDirectory;
            if (string.IsNullOrEmpty(rootPath)) rootPath = result.WorkingDirectory;

            // Take a before snapshot of all the files
            var outputDirectory = new DirectoryInfo(RootPath(OutputDir, false));
            var oldFiles = GenerateOriginalFileList(outputDirectory);

            // Run the executable
			var processResult = TryToRun(CreateProcessInfo(result), result);
            result.AddTaskResult(new ProcessTaskResult(processResult));

            // Check for any new files and copy them to the artefact folder
            if (!processResult.Failed)
            {
                outputDirectory.Refresh();
                var newFiles = ListFileDifferences(oldFiles, outputDirectory);
                if (newFiles.Length > 0)
                {
                    // Copy all the new files over
                    var publishDir = Path.Combine(result.BaseFromArtifactsDirectory(result.Label), "NCover");
                    Log.Debug(string.Format(System.Globalization.CultureInfo.CurrentCulture,"Copying {0} files to {1}", newFiles.Length, publishDir));

                    var index = outputDirectory.FullName.Length + 1;
                    foreach (FileInfo newFile in newFiles)
                    {
                        var fileInfo = new FileInfo(Path.Combine(publishDir, newFile.FullName.Substring(index)));
                        if (!fileInfo.Directory.Exists) fileInfo.Directory.Create();
                        newFile.CopyTo(fileInfo.FullName, true);
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
            var coverageFile = string.IsNullOrEmpty(CoverageFile) ? "coverage.xml" : CoverageFile; 
            buffer.AppendArgument(RootPath(coverageFile, true));

            // Add all the NCover arguments
            buffer.AppendIf(ClearCoverageFilters, "//ccf");
            foreach (var filter in CoverageFilters ?? new CoverageFilter[0])
            {
                buffer.AppendArgument("//cf {0}", filter.ToParamString());
            }
            foreach (var threshold in MinimumThresholds ?? new CoverageThreshold[0])
            {
                buffer.AppendArgument("//mc {0}", threshold.ToParamString());
            }
            buffer.AppendIf(UseMinimumCoverage, "//mcsc");
            buffer.AppendIf(XmlReportFilter != NCoverReportFilter.Default, "//rdf {0}", XmlReportFilter.ToString());
            foreach (var threshold in SatisfactoryThresholds ?? new CoverageThreshold[0])
            {
                buffer.AppendArgument("//sct {0}", threshold.ToParamString());
            }
            buffer.AppendIf(NumberToReport > 0, "//smf {0}", NumberToReport.ToString(CultureInfo.CurrentCulture));
            buffer.AppendIf(!string.IsNullOrEmpty(TrendOutputFile), "//at \"{0}\"", RootPath(TrendOutputFile, false));
            buffer.AppendArgument("//bi \"{0}\"", string.IsNullOrEmpty(BuildId) ? result.Label : BuildId);
            buffer.AppendIf(!string.IsNullOrEmpty(HideElements), "//hi \"{0}\"", HideElements);
            buffer.AppendIf(!string.IsNullOrEmpty(TrendInputFile), "//lt \"{0}\"", RootPath(TrendInputFile, false));
            GenerateReportList(buffer);
            buffer.AppendIf(!string.IsNullOrEmpty(ProjectName), "//p \"{0}\"", ProjectName);
            buffer.AppendIf(SortBy != NCoverSortBy.None, "//so \"{0}\"", SortBy.ToString());
            buffer.AppendIf(TopUncoveredAmount > 0, "//tu \"{0}\"", TopUncoveredAmount.ToString(CultureInfo.CurrentCulture));
            buffer.AppendIf(MergeMode != NCoverMergeMode.Default, "//mfm \"{0}\"", MergeMode.ToString());
            buffer.AppendIf(!string.IsNullOrEmpty(MergeFile), "//s \"{0}\"", RootPath(MergeFile, false));
            buffer.AppendIf(!string.IsNullOrEmpty(WorkingDirectory), "//w \"{0}\"", RootPath(WorkingDirectory, false));

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

        #region ListFileDifferences()
        /// <summary>
        /// Generate a list of differences in files.
        /// </summary>
        /// <param name="originalList"></param>
        /// <param name="outputDirectory"></param>
        /// <returns></returns>
        private FileInfo[] ListFileDifferences(Dictionary<string, DateTime> originalList, DirectoryInfo outputDirectory)
        {
            FileInfo[] newList = { };
            var index = 0;
            if (outputDirectory.Exists)
            {
                index = outputDirectory.FullName.Length;
                newList = outputDirectory.GetFiles("*.*", SearchOption.AllDirectories);
            }

            List<FileInfo> differenceList = new List<FileInfo>();

            // For each new file, see if it is in the old file list
            foreach (FileInfo newFile in newList)
            {
                var filename = newFile.FullName.Substring(index);
                if (originalList.ContainsKey(filename))
                {
                    // Check if the times are different
                    if (originalList[filename] != newFile.LastWriteTime)
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
                FileInfo[] oldFiles = outputDirectory.GetFiles("*.*", SearchOption.AllDirectories);
                var index = outputDirectory.FullName.Length;
                foreach (FileInfo oldFile in oldFiles)
                {
                    var filename = oldFile.FullName.Substring(index);
                    originalFiles.Add(filename, oldFile.LastWriteTime);
                }
            }
            return originalFiles;
        }
        #endregion

        #region GenerateReportList()
        /// <summary>
        /// Generate the list of reports to generate.
        /// </summary>
        /// <param name="buffer"></param>
        private void GenerateReportList(ProcessArgumentBuilder buffer)
        {
            var reportList = new List<NCoverReportType>();
            if ((Reports != null) && (Reports.Length > 0))
            {
                reportList.AddRange(Reports);
            }
            else
            {
                reportList.Add(NCoverReportType.FullCoverageReport);
            }
            foreach (var report in reportList)
            {
                var path = OutputDir;
                if (report == NCoverReportType.FullCoverageReport)
                {
                    path = RootPath(path, false);
                }
                else
                {
                    path = RootPath(string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}.html", report), false);
                }
                buffer.AppendArgument("//or \"{0}\"", string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}:Html:{1}", report, path));
            }
        }
        #endregion
        #endregion

        #region Enumerations
        #region NCoverReportFilter
        /// <summary>
        /// The type of report filter.
        /// </summary>
        public enum NCoverReportFilter
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
            Assembly,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            Namespace
        }
        #endregion

        #region NCoverReportType
        /// <summary>
        /// The type of report to generate.
        /// </summary>
        public enum NCoverReportType
        {
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            FullCoverageReport,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            Summary,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            UncoveredCodeSections,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolSourceCode,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolSourceCodeClass,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolSourceCodeClassMethod,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            MethodSourceCode,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            MethodSourceCodeClass,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            MethodSourceCodeClassMethod,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolModule,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolModuleNamespace,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolModuleNamespaceClass,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolModuleNamespaceClassMethod,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolCCModuleClassFailedCoverageTop,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            SymbolCCModuleClassCoverageTop,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            MethodModuleNamespaceClass,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            MethodModuleNamespaceClassMethod,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            MethodCCModuleClassFailedCoverageTop,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            MethodCCModuleClassCoverageTop
        }
        #endregion

        #region NCoverSortBy
        /// <summary>
        /// The sort order to use.
        /// </summary>
        public enum NCoverSortBy
        {
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            None,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            Name,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            ClassLine,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            CoveragePercentageAscending,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            CoveragePercentageDescending,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            UnvisitedSequencePointsAscending,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            UnvisitedSequencePointsDescending,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            VisitCountAscending,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            VisitCountDescending,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            FunctionCoverageAscending,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            FunctionCoverageDescending
        }
        #endregion

        #region NCoverMergeMode
        /// <summary>
        /// The merge mode to use.
        /// </summary>
        public enum NCoverMergeMode
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
            KeepSourceFilters,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            Destructive,
            /// <summary>
            /// 	
            /// </summary>
            /// <remarks></remarks>
            AppendFilters
        }
        #endregion
        #endregion
    }
}
