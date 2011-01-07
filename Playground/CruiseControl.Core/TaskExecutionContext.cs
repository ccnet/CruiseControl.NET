namespace CruiseControl.Core
{
    using System;
    using System.Threading;
    using System.Xml;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// The context that tasks run in.
    /// </summary>
    public class TaskExecutionContext
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly XmlWriter writer;
        private readonly int threadId;
        private readonly IFileSystem fileSystem;
        private readonly IClock clock;
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="TaskExecutionContext"/> class from being created.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="clock">The clock.</param>
        public TaskExecutionContext(XmlWriter writer, IFileSystem fileSystem, IClock clock)
        {
            this.writer = writer;
            this.threadId = Thread.CurrentThread.ManagedThreadId;
            this.fileSystem = fileSystem;
            this.clock = clock;
        }
        #endregion

        #region Public properties
        #region CurrentStatus
        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public virtual IntegrationStatus CurrentStatus { get; set; }
        #endregion

        #region IsCompleted
        /// <summary>
        /// Gets a value indicating whether this context is completed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region StartChild()
        /// <summary>
        /// Starts a new child context.
        /// </summary>
        /// <param name="task">The task.</param>
        /// <returns>
        /// The new child <see cref="TaskExecutionContext"/>.
        /// </returns>
        public TaskExecutionContext StartChild(Task task)
        {
            var child = new TaskExecutionContext(this.writer, this.fileSystem, this.clock);
            this.writer.WriteStartElement("task");
            if (!string.IsNullOrEmpty(task.Name))
            {
                this.writer.WriteAttributeString("name", task.Name);
            }

            this.writer.WriteAttributeString("type", task.GetType().Name);
            writer.WriteElementString("start", this.clock.Now.ToString("s"));
            return child;
        }
        #endregion

        #region AddEntryToBuildLog()
        /// <summary>
        /// Adds an information entry to build log.
        /// </summary>
        /// <param name="message">The message of the entry.</param>
        public virtual void AddEntryToBuildLog(string message)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
        #endregion

        #region Complete()
        /// <summary>
        /// Completes this context.
        /// </summary>
        public virtual void Complete()
        {
            if (!this.IsCompleted)
            {
                this.IsCompleted = true;
                this.writer.WriteEndDocument();
                this.writer.Close();
            }
        }
        #endregion
        #endregion
    }
}
