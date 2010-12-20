namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using System.Windows.Markup;

    [ContentProperty("Tasks")]
    public class Sequence
        : Task
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        public Sequence()
        {
            this.Tasks = new List<Task>();
        }
        #endregion

        #region Public properties
        #region Tasks
        /// <summary>
        /// Gets or sets the tasks.
        /// </summary>
        /// <value>The tasks.</value>
        public IList<Task> Tasks { get; private set; }
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
            // TODO: Implement this task
            return null;
        }
        #endregion
        #endregion
    }
}
