namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using CruiseControl.Common;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// Forces a build on another project.
    /// </summary>
    public class ForceBuild
        : Task
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public properties
        #region ProjectName
        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        /// <value>
        /// The name of the project to force.
        /// </value>
        public string ProjectName { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region Validate()
        /// <summary>
        /// Called when this task is validated.
        /// </summary>
        /// <param name="validationLog">The validation log.</param>
        protected override void OnValidate(IValidationLog validationLog)
        {
            base.OnValidate(validationLog);
            if (string.IsNullOrEmpty(this.ProjectName))
            {
                validationLog.AddError("ProjectName has not been set");
            }
        }
        #endregion
        #endregion

        #region Protected methods
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
            logger.Info("Sending force build to '{0}'", this.ProjectName);
            var arguments = new InvokeArguments
                                {
                                    Action = "ForceBuild"
                                };
            var result = this.Project.Server.ActionInvoker.Invoke(this.ProjectName, arguments);

            // TODO: Validate the result
            return null;
        }
        #endregion
        #endregion
    }
}
