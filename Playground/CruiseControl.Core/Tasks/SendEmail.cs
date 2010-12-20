namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;

    public class SendEmail
        : Task
    {
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
            // TODO: Implement this task
            return null;
        }
        #endregion
        #endregion
    }
}
