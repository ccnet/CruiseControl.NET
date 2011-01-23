namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using System.Windows.Markup;
    using NLog;

    /// <summary>
    /// A sequence of tasks.
    /// </summary>
    [ContentProperty("Tasks")]
    public class Sequence
        : Task
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        public Sequence()
        {
            this.Tasks = new List<Task>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Sequence"/> class.
        /// </summary>
        /// <param name="tasks">The tasks.</param>
        public Sequence(params Task[] tasks)
        {
            this.Tasks = new List<Task>(tasks);
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
            logger.Info("Running sequence '{0}' with {1} task(s)", this.NameOrType, this.Tasks.Count);

            var count = 0;
            foreach (var task in this.Tasks)
            {
                logger.Info("Running child task {0} in '{1}'", count++, this.NameOrType);
                yield return task;
            }

            yield break;
        }
        #endregion
        #endregion
    }
}
