namespace CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using CruiseControl.Core.Interfaces;
    using CruiseControl.Core.Utilities;
    using Ninject;
    using NLog;

    /// <summary>
    /// Performs a build using MSBuild.
    /// </summary>
    public class BuildMSBuild
        : Task
    {
        #region Constants
        public const string DefaultExecutable = "msbuild";
        #endregion

        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public properties
        #region Executable
        /// <summary>
        /// Gets or sets the MSBuild executable location.
        /// </summary>
        /// <value>
        /// The MSBuild executable location.
        /// </value>
        public string Executable { get; set; }
        #endregion

        #region ProjectFile
        /// <summary>
        /// Gets or sets the project file.
        /// </summary>
        /// <value>
        /// The project file.
        /// </value>
        public string ProjectFile { get; set; }
        #endregion

        #region ProcessExecutor
        /// <summary>
        /// Gets or sets the process executor.
        /// </summary>
        /// <value>
        /// The process executor.
        /// </value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Inject]
        public IProcessExecutor ProcessExecutor { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region OnRun()
        /// <summary>
        /// Executes this task.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        /// The child tasks to execute.
        /// </returns>
        protected override IEnumerable<Task> OnRun(TaskExecutionContext context)
        {
            logger.Info("Building MSBuild project '{0}'", this.ProjectFile);
            var info = InitialiseProcess();
            var result = this.ProcessExecutor.Execute(info, this, context);
            if (!result.Succeeded)
            {
                context.CurrentStatus = IntegrationStatus.Failure;
            }
            else
            {
                context.CurrentStatus = IntegrationStatus.Success;
            }

            return null;
        }
        #endregion
        #endregion

        #region Private methods
        #region InitialiseProcess()
        /// <summary>
        /// Initialises the process.
        /// </summary>
        /// <returns>
        /// The information for the process.
        /// </returns>
        private ProcessInfo InitialiseProcess()
        {
            var args = new SecureArguments();
            args.Add(this.ProjectFile);
            return new ProcessInfo(
                this.Executable ?? DefaultExecutable,
                args,
                Environment.CurrentDirectory);
        }
        #endregion
        #endregion
    }
}
