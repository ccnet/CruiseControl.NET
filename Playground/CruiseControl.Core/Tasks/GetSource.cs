namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using CruiseControl.Core.Interfaces;
    using NLog;

    public class GetSource
        : SourceControlTask
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
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
            var block = this.GetSourceControlBlock();
            var parameters = new GetSourceParameters();
            logger.Debug("Getting source for '{0}'", this.NameOrType);
            block.GetSource(parameters);
            return null;
        }
        #endregion
        #endregion
    }
}
