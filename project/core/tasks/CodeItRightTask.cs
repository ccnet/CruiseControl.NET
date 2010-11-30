//***********************************************************************
// Author           : Craig Sutherland
// Created          : 27 Feb, 2010
// Copyright        : (c) 2010 CruiseControl.NET. All rights reserved.
//***********************************************************************
namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <title>CodeItRight Analysis Task</title>
    /// <version>1.5</version>
    /// <summary>
    /// <para>
    /// Perform a code analysis using SubMain.CodeItRight.
    /// </para>
    /// <para type="tip">
    /// SubMain.CodeItRight is a commerical application that will analyse the code for any standards violations. The tool is 
    /// available from http://submain.com/products/codeit.right.aspx.
    /// </para>
    /// <para type="info">
    /// <title>Supported Versions</title>
    /// CruiseControl.NET only supports CodeItRight 1.9 currently.
    /// </para>
    /// </summary>
    /// <example>
    /// <code>
    /// &lt;codeItRight&gt;
    /// &lt;solution&gt;myproject.*.sln&lt;/solution&gt;
    /// &lt;/codeItRight&gt;
    /// </code>
    /// </example>
    /// <remarks>
    /// <para>
    /// This task calls CodeItRight.Cmd to perform the analysis. Full details on this tool is available at
    /// http://community.submain.com/blogs/tutorials/archive/2008/03/23/codeitright-cmd-exe-command-line-parameters.aspx. 
    /// Additional details on the mapped arguments can be found there.
    /// </para>
    /// </remarks>
    [ReflectorType("codeItRight")]
    public class CodeItRightTask
        : BaseExecutableTask
    {
        #region Private consts
        /// <summary>
        /// The name of the default executable.
        /// </summary>
        private const string DefaultExecutable = "SubMain.CodeItRight.Cmd";
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CodeItRightTask"/> class.
        /// </summary>
        public CodeItRightTask()
            : this(new ProcessExecutor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CodeItRightTask"/> class.
        /// </summary>
        /// <param name="executor">The executor.</param>
        public CodeItRightTask(ProcessExecutor executor)
        {
            this.executor = executor;
            this.Executable = CodeItRightTask.DefaultExecutable;
            this.TimeOut = 600;
            this.ReportingThreshold = Severity.None;
            this.FailureThreshold = Severity.None;
            this.Priority = ProcessPriorityClass.Normal;
        }
        #endregion

        #region Public properties
        #region Executable
        /// <summary>
        /// The executable to use.
        /// </summary>
        /// <version>1.5</version>
        /// <default>SubMain.CodeItRight.Cmd</default>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
        #endregion

        #region Solution
        /// <summary>
        /// The solution to analyse.
        /// </summary>
        /// <version>1.5</version>
        /// <default>none</default>
        /// <remarks>
        /// Either the solution or the project must be specified.
        /// </remarks>
        [ReflectorProperty("solution", Required = false)]
        public string Solution { get; set; }
        #endregion

        #region Project
        /// <summary>
        /// The project to analyse.
        /// </summary>
        /// <version>1.5</version>
        /// <default>none</default>
        /// <remarks>
        /// Either the solution or the project must be specified.
        /// </remarks>
        [ReflectorProperty("project", Required = false)]
        public string Project { get; set; }
        #endregion

        #region Xsl
        /// <summary>
        /// The name of the XSL file to override the default XSL.
        /// </summary>
        /// <version>1.5</version>
        /// <default>none</default>
        [ReflectorProperty("xsl", Required = false)]
        public string Xsl { get; set; }
        #endregion

        #region CRData
        /// <summary>
        /// The name of the CodeIt.Right .crdata file. When specified, CodeItRight.Cmd will use the exclusion list (violations, rules
        /// and files) saved using the Visual Studio version of CodeIt.Right.
        /// </summary>
        /// <version>1.5</version>
        /// <default>none</default>
        [ReflectorProperty("crData", Required = false)]
        public string CRData { get; set; }
        #endregion

        #region Profile
        /// <summary>
        /// The name of the User Profile that defines active rule set for the analysis. When omitted, the built-in profile is used.
        /// </summary>
        /// <version>1.5</version>
        /// <default>none</default>
        [ReflectorProperty("profile", Required = false)]
        public string Profile { get; set; }
        #endregion

        #region ReportingThreshold
        /// <summary>
        /// Severity Threshold value to limit the output violation set. When omitted, the the lowest Severity is used - None.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("reportingThreshold", Required = false)]
        public Severity ReportingThreshold { get; set; }
        #endregion

        #region FailureThreshold
        /// <summary>
        /// Severity value to fail the build on. When omitted, the the lowest Severity is used - None.
        /// </summary>
        /// <version>1.5</version>
        /// <default>None</default>
        [ReflectorProperty("failureThreshold", Required = false)]
        public Severity FailureThreshold { get; set; }
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

        #region Priority
        /// <summary>
        /// The priority class of the spawned process.
        /// </summary>
        /// <version>1.5</version>
        /// <default>Normal</default>
        [ReflectorProperty("priority", Required = false)]
        public ProcessPriorityClass Priority { get; set; }
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
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Running CodeItRight analysis");

            // Run the executable
            var info = this.CreateProcessInfo(result);
            var processResult = this.TryToRun(info, result);

            // Need to start a new result as CodeItRight returns the number of violation
            processResult = new ProcessResult(
                processResult.StandardOutput,
                processResult.StandardError,
                processResult.ExitCode,
                processResult.TimedOut,
                processResult.ExitCode < 0);
            result.AddTaskResult(new ProcessTaskResult(processResult));
            if (processResult.TimedOut)
            {
                result.AddTaskResult(MakeTimeoutBuildResult(info));
            }

            if (processResult.Succeeded)
            {
                var xmlFile = result.BaseFromWorkingDirectory("codeitright.xml");
                result.AddTaskResult(
                    new FileTaskResult(xmlFile, true));
            }

            // Check the failure threshold
            var failed = !processResult.Succeeded;
            if (!failed && (this.FailureThreshold != Severity.None))
            {
                var xmlFile = result.BaseFromWorkingDirectory("codeitright.xml");
                var document = new XmlDocument();
                if (File.Exists(xmlFile))
                {
                    document.Load(xmlFile);
                    for (var level = (int)Severity.CriticalError; level >= (int)this.FailureThreshold; level--)
                    {
                        failed = CodeItRightTask.CheckReportForSeverity(document, (Severity)level);
                        if (failed)
                        {
                            break;
                        }
                    }
                }
            }

            return !failed;
        }
        #endregion

        #region GetProcessFilename()
        /// <summary>
        /// Retrieve the executable to use.
        /// </summary>
        /// <returns>The name of the executable.</returns>
        protected override string GetProcessFilename()
        {
            return this.Executable;
        }
        #endregion

        #region GetProcessBaseDirectory()
        /// <summary>
        /// Retrieve the base directory.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>The working directory folder.</returns>
        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            string path = result.WorkingDirectory;
            return path;
        }
        #endregion

        #region GetProcessTimeout()
        /// <summary>
        /// Get the time-out period.
        /// </summary>
        /// <returns>The timeout period in milliseconds.</returns>
        protected override int GetProcessTimeout()
        {
            return this.TimeOut * 1000;
        }
        #endregion

        #region GetProcessArguments()
        /// <summary>
        /// Retrieve the arguments
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>A <c>string</c> containing the arguments.</returns>
        protected override string GetProcessArguments(IIntegrationResult result)
        {
            ProcessArgumentBuilder buffer = new ProcessArgumentBuilder();
            buffer.AddArgument("/quiet");
            buffer.AddArgument("/severityThreshold:\"" + this.ReportingThreshold.ToString() + "\"");
            buffer.AddArgument("/out:\"" + result.BaseFromWorkingDirectory("codeitright.xml") + "\"");
            if (!string.IsNullOrEmpty(this.Solution))
            {
                buffer.AddArgument("/Solution:\"" + this.EnsurePathIsRooted(result, this.Solution) + "\"");
            }
            else if (!string.IsNullOrEmpty(this.Project))
            {
                buffer.AddArgument("/Project:\"" + this.EnsurePathIsRooted(result, this.Project) + "\"");
            }
            else
            {
                throw new CruiseControlException("Either a solution or a project must be specified for analysis.");
            }

            if (!string.IsNullOrEmpty(this.Xsl))
            {
                buffer.AddArgument("/outxsl:\"" + this.EnsurePathIsRooted(result, this.Xsl) + "\"");
            }

            if (!string.IsNullOrEmpty(this.CRData))
            {
                buffer.AddArgument("/crdata:\"" + this.EnsurePathIsRooted(result, this.CRData) + "\"");
            }

            if (!string.IsNullOrEmpty(this.Profile))
            {
                buffer.AddArgument("/profile:\"" + this.Profile + "\"");
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
        #region CheckReportForSeverity()
        /// <summary>
        /// Checks if the report has the severity.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if there is a violation with that severity, <c>false</c> otherwise.</returns>
        private static bool CheckReportForSeverity(XmlDocument document, Severity value)
        {
            var nodes = document.SelectNodes("/CodeItRightReport/Violations/Violation[Severity='" + value.ToString() + "']");
            return nodes.Count > 0;
        }
        #endregion

        #region EnsurePathIsRooted()
        /// <summary>
        /// Ensures the path is rooted.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="path">The path.</param>
        /// <returns>The rooted path.</returns>
        private string EnsurePathIsRooted(IIntegrationResult result, string path)
        {
            if (!Path.IsPathRooted(path))
            {
                return result.BaseFromWorkingDirectory(path);
            }
            else
            {
                return path;
            }
        }
        #endregion
        #endregion

        #region Enumerations
        #region SeverityThreshold
        /// <summary>
        /// The severity thresholds.
        /// </summary>
        public enum Severity
        {
            /// <summary>
            /// Display critical errors.
            /// </summary>
            CriticalError = 5,

            /// <summary>
            /// Display errors.
            /// </summary>
            Error = 4,

            /// <summary>
            /// Display critical warnings.
            /// </summary>
            CriticalWarning = 3,

            /// <summary>
            /// Display warnings.
            /// </summary>
            Warning = 2,

            /// <summary>
            /// Display information.
            /// </summary>
            Information = 1,

            /// <summary>
            /// No severity threshold.
            /// </summary>
            None = 0,
        }
        #endregion
        #endregion
    }
}
