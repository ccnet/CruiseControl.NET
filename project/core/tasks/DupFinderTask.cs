//-----------------------------------------------------------------------
// <copyright file="DupFinderTask.cs" company="Craig Sutherland">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Tasks
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Util;

    /// <summary>
    /// Check for duplicates using dupfinder (http://duplicatefinder.codeplex.com/).
    /// </summary>
    [ReflectorType("dupfinder")]
    public class DupFinderTask
        : BaseExecutableTask
    {
        #region Private consts
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private constant")]
        private const string DefaultExecutable = "dupfinder";
        #endregion

        #region Private fields
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Private field")]
        private string executable;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="DupFinderTask"/> class.
        /// </summary>
        public DupFinderTask()
            : this(new ProcessExecutor())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DupFinderTask"/> class.
        /// </summary>
        /// <param name="executor">The executor to use.</param>
        public DupFinderTask(ProcessExecutor executor)
        {
            this.executor = executor;
            this.TimeOut = 600;
            this.Threshold = 5;
            this.Width = 2;
        }
        #endregion

        #region Public properties
        #region Executable
        /// <summary>
        /// Gets or sets the executable to use.
        /// </summary>
        /// <value>The executable.</value>
        [ReflectorProperty("executable", Required = false)]
        public string Executable { get; set; }
        #endregion

        #region InputDir
        /// <summary>
        /// Gets or sets the input directory to scan.
        /// </summary>
        [ReflectorProperty("inputDir", Required = true)]
        public string InputDir { get; set; }
        #endregion

        #region FileMask
        /// <summary>
        /// Gets or sets the file mask to use.
        /// </summary>
        [ReflectorProperty("fileMask", Required = true)]
        public string FileMask { get; set; }
        #endregion

        #region Focus
        /// <summary>
        /// Gets or sets the name of the file to focus on.
        /// </summary>
        [ReflectorProperty("focus", Required = false)]
        public string Focus { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// Gets or sets the time-out period in seconds.
        /// </summary>
        [ReflectorProperty("timeout", Required = false)]
        public int TimeOut { get; set; }
        #endregion

        #region Threshold
        /// <summary>
        /// Gets or sets the threshold is the number of consecutive lines that have to be the same before it is considered a duplicate.
        /// </summary>
        [ReflectorProperty("threshold", Required = false)]
        public int Threshold { get; set; }
        #endregion

        #region Width
        /// <summary>
        /// Gets or sets the first line of a duplicate must contain at least this many non-white-space characters.
        /// </summary>
        [ReflectorProperty("width", Required = false)]
        public int Width { get; set; }
        #endregion

        #region Recurse
        /// <summary>
        /// Gets or sets a value indicating whether to find files that match the filemask in current directory and subdirectories.
        /// </summary>
        /// <value><c>true</c> if recurse; otherwise, <c>false</c>.</value>
        [ReflectorProperty("recurse", Required = false)]
        public bool Recurse { get; set; }
        #endregion

        #region LinesToExclude
        /// <summary>
        /// Gets or sets the lines to exclude.
        /// </summary>
        [ReflectorProperty("excludeLines", Required = false)]
        public string[] LinesToExclude { get; set; }
        #endregion

        #region FilesToExclude
        /// <summary>
        /// Gets or sets the files to exclude.
        /// </summary>
        [ReflectorProperty("excludeFiles", Required = false)]
        public string[] FilesToExclude { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Run the task.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>
        /// True if the task was successful, false otherwise.
        /// </returns>
        protected override bool Execute(IIntegrationResult result)
        {
            result.BuildProgressInformation.SignalStartRunTask(!string.IsNullOrEmpty(Description) ? Description : "Executing DupFinder");

            this.executable = string.IsNullOrEmpty(this.Executable) ? DefaultExecutable : this.Executable;
            if (!Path.IsPathRooted(this.executable))
            {
                this.executable = result.BaseFromWorkingDirectory(this.executable);
            }

            // Run the executable
            var processResult = TryToRun(CreateProcessInfo(result), result);
            result.AddTaskResult(new ProcessTaskResult(processResult, false));

            return !processResult.Failed;
        }
        #endregion

        #region GetProcessFilename()
        /// <summary>
        /// Retrieve the executable to use.
        /// </summary>
        /// <returns>The filename of the process to execute.</returns>
        protected override string GetProcessFilename()
        {
            var path = StringUtil.AutoDoubleQuoteString(this.executable);
            return path;
        }
        #endregion

        #region GetProcessBaseDirectory()
        /// <summary>
        /// Retrieve the base directory.
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>The base directory to use.</returns>
        protected override string GetProcessBaseDirectory(IIntegrationResult result)
        {
            var path = StringUtil.AutoDoubleQuoteString(this.InputDir);
            return path;
        }
        #endregion

        #region GetProcessTimeout()
        /// <summary>
        /// Get the time-out period.
        /// </summary>
        /// <returns>The time-out period in milliseconds.</returns>
        protected override int GetProcessTimeout()
        {
            return this.TimeOut * 1000;
        }
        #endregion

        #region GetProcessArguments()
        /// <summary>
        /// Retrieve the arguments
        /// </summary>
        /// <param name="result">The result to use.</param>
        /// <returns>The arguments to pass to the process.</returns>
        protected override string GetProcessArguments(IIntegrationResult result)
        {
            var buffer = new ProcessArgumentBuilder();
            buffer.AppendIf(this.Recurse, "-r");
            buffer.AppendArgument("-t" + this.Threshold.ToString());
            buffer.AppendArgument("-w" + this.Width.ToString());
            buffer.AppendArgument("-oConsole");

            // Add the focus
            if (!string.IsNullOrEmpty(this.Focus))
            {
                buffer.AppendArgument("-f" + StringUtil.AutoDoubleQuoteString(this.Focus));
            }

            // Add the lines to exclude
            foreach (var line in this.LinesToExclude ?? new string[0])
            {
                buffer.AppendArgument("-x" + StringUtil.AutoDoubleQuoteString(line));
            }

            // Add the lines to exclude
            foreach (var line in this.FilesToExclude ?? new string[0])
            {
                buffer.AppendArgument("-e" + StringUtil.AutoDoubleQuoteString(line));
            }

            buffer.AppendArgument(this.FileMask);
            return buffer.ToString();
        }
        #endregion
        #endregion
    }
}
