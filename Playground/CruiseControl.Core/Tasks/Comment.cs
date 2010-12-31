namespace CruiseControl.Core.Tasks
{
    using System.Collections.Generic;
    using System.Windows.Markup;
    using NLog;

    /// <summary>
    /// Outputs a comment to the logs.
    /// </summary>
    [ContentProperty("Text")]
    public class Comment
        : Task
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        public Comment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public Comment(string name)
            : base(name)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Comment"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="text">The text.</param>
        public Comment(string name, string text)
            : base(name)
        {
            this.Text = text;
        }
        #endregion

        #region Public properties
        #region Text
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; set; }
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
            logger.Info("Adding comment to the build log");
            context.AddEntryToBuildLog(this.Text);
            return null;
        }
        #endregion
        #endregion
    }
}
