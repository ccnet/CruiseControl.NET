namespace CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;

    public class BuildRake
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
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
