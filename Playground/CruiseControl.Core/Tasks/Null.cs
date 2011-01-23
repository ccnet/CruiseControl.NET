namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using NLog;

    /// <summary>
    /// A task that does nothing.
    /// </summary>
    public class Null
        : Task
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
            logger.Info("Doing nothing");
            return null;
        }
        #endregion
        #endregion
    }
}
