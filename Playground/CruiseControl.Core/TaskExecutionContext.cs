namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
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
        private readonly IntegrationRequest request;
        private readonly IFileSystem fileSystem;
        private readonly IClock clock;
        private IList<ModificationSet> modificationsSets = new List<ModificationSet>();
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="TaskExecutionContext"/> class from being created.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="fileSystem">The file system.</param>
        /// <param name="clock">The clock.</param>
        /// <param name="request">The request.</param>
        public TaskExecutionContext(XmlWriter writer, IFileSystem fileSystem, IClock clock, IntegrationRequest request)
        {
            this.writer = writer;
            this.threadId = Thread.CurrentThread.ManagedThreadId;
            this.fileSystem = fileSystem;
            this.clock = clock;
            this.request = request;
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

        #region Parent
        /// <summary>
        /// Gets the parent context.
        /// </summary>
        public TaskExecutionContext Parent { get; private set; }
        #endregion

        #region Request
        /// <summary>
        /// Gets the request.
        /// </summary>
        public IntegrationRequest Request
        {
            get { return this.request; }
        }
        #endregion

        #region ModificationSets
        /// <summary>
        /// Gets the modification sets.
        /// </summary>
        public virtual IEnumerable<ModificationSet> ModificationSets
        {
            get { return this.modificationsSets; }
        }
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
        public virtual TaskExecutionContext StartChild(Task task)
        {
            logger.Debug("Starting new task execution context for '{0}'", task.NameOrType);
            var child = new TaskExecutionContext(this.writer, this.fileSystem, this.clock, this.request)
                            {
                                Parent = this,
                                modificationsSets = this.modificationsSets
                            };
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
            logger.Trace("Adding trace entry to log: {0}", message);
            this.writer.WriteStartElement("entry");
            this.writer.WriteAttributeString("time", this.clock.Now.ToString("s"));
            this.writer.WriteString(message);
            this.writer.WriteEndElement();
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
                if (this.CurrentStatus == IntegrationStatus.Unknown)
                {
                    this.CurrentStatus = IntegrationStatus.Success;
                }

                writer.WriteElementString("finish", this.clock.Now.ToString("s"));
                writer.WriteElementString("status", this.CurrentStatus.ToString());
                if (this.Parent == null)
                {
                    logger.Debug("Completing task execution context");
                    this.writer.WriteEndDocument();
                    this.writer.Close();
                }
                else
                {
                    logger.Debug("Completing child task execution context");
                    this.writer.WriteEndElement();
                }
            }
        }
        #endregion
        #endregion
    }
}
