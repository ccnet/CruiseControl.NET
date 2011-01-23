namespace CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;

    public class ApplyLastChangeLabel
        : SourceControlTask
    {
        #region Public properties
        #region Prefix
        /// <summary>
        /// Gets or sets the prefix.
        /// </summary>
        /// <value>
        /// The prefix.
        /// </value>
        public string Prefix { get; set; }
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
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
