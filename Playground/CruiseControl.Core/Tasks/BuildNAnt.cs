namespace CruiseControl.Core.Tasks
{
    using System;
    using System.Collections.Generic;

    public class BuildNAnt
        : Task
    {
        #region Public properties
        #region BuildFile
        /// <summary>
        /// Gets or sets the build file.
        /// </summary>
        /// <value>
        /// The build file.
        /// </value>
        public string BuildFile { get; set; }
        #endregion

        #region TimeOut
        /// <summary>
        /// Gets or sets the time out.
        /// </summary>
        /// <value>
        /// The time out.
        /// </value>
        public TimeSpan TimeOut { get; set; }
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
