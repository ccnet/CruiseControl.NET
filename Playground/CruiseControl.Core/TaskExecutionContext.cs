namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
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
        private IList<ModificationSet> modificationsSets = new List<ModificationSet>();
        private ReaderWriterLockSlim modificationsLock = new ReaderWriterLockSlim();
        private readonly IClock clock;
        private readonly IFileSystem fileSystem;
        private readonly Project project;
        private readonly XmlWriter writer;
        private readonly IntegrationRequest request;
        private readonly string buildName;
        #endregion

        #region Constructors
        /// <summary>
        /// Prevents a default instance of the <see cref="TaskExecutionContext"/> class from being created.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        public TaskExecutionContext(TaskExecutionParameters parameters)
        {
            this.writer = parameters.XmlWriter;
            this.fileSystem = parameters.FileSystem;
            this.clock = parameters.Clock;
            this.request = parameters.IntegrationRequest;
            this.project = parameters.Project;
            this.buildName = parameters.BuildName;
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
        /// <remarks>
        /// If the modifications cannot be locked for reading within 5s then <c>null</c> will be returned.
        /// </remarks>
        public virtual IEnumerable<ModificationSet> ModificationSets
        {
            get
            {
                var lockAcquired = false;
                try
                {
                    lockAcquired = this.modificationsLock.TryEnterReadLock(TimeSpan.FromSeconds(5));
                    return lockAcquired ? this.modificationsSets : null;
                }
                finally
                {
                    if (lockAcquired)
                    {
                        this.modificationsLock.ExitReadLock();
                    }
                }
            }
        }
        #endregion

        #region Project
        /// <summary>
        /// Gets the project.
        /// </summary>
        public Project Project
        {
            get { return this.project; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GenerateArtifactFileName()
        /// <summary>
        /// Generates a new log name for a project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="buildName">Name of the build.</param>
        /// <param name="filename">The filename.</param>
        /// <returns>
        /// The name of the new log for the project.
        /// </returns>
        public static string GenerateArtifactFileName(Project project, string buildName, string filename)
        {
            var baseDir = Environment.CurrentDirectory;
            var logName = Path.Combine(
                baseDir,
                project.Name,
                buildName,
                filename);
            return logName;
        }
        #endregion

        #region AddModifications()
        /// <summary>
        /// Adds a modification set.
        /// </summary>
        /// <param name="modifications">The modification set.</param>
        public void AddModifications(ModificationSet modifications)
        {
            var lockAcquired = false;
            try
            {
                lockAcquired = this.modificationsLock.TryEnterWriteLock(TimeSpan.FromMinutes(2));
                if (lockAcquired)
                {
                    logger.Debug("Adding modification set to context");
                    this.modificationsSets.Add(modifications);
                }
                else
                {
                    logger.Error("Unable to acquire lock for adding modifications");
                    throw new Exception("Unable to acquire lock for adding modifications");
                }
            }
            finally
            {
                if (lockAcquired)
                {
                    this.modificationsLock.ExitWriteLock();
                }
            }
        }
        #endregion

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
            var parameters = new TaskExecutionParameters
                                 {
                                     XmlWriter = this.writer,
                                     FileSystem = this.fileSystem,
                                     Clock = this.clock,
                                     IntegrationRequest = this.request,
                                     Project = this.project,
                                     BuildName = this.buildName
                                 };
            var child = new TaskExecutionContext(parameters)
                            {
                                Parent = this,
                                modificationsSets = this.modificationsSets,
                                modificationsLock = this.modificationsLock
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

        #region ImportFile()
        /// <summary>
        /// Imports a file.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="deleteAfterImport">If set to <c>true</c> the file will be deleted after the import.</param>
        public virtual void ImportFile(string filename, bool deleteAfterImport)
        {
            var destination = Path.GetFileName(filename);
            logger.Trace("Importing file: {0}", filename);
            this.writer.WriteStartElement("file");
            this.writer.WriteAttributeString("time", this.clock.Now.ToString("s"));
            this.writer.WriteString(destination);
            this.writer.WriteEndElement();

            var destinationPath = GenerateArtifactFileName(this.project,
                                                   this.buildName,
                                                   destination);
            if (deleteAfterImport)
            {
                logger.Debug("Moving file from '{0}' to '{1}'", filename, destinationPath);
                this.fileSystem.MoveFile(filename, destinationPath);
            }
            else
            {
                logger.Debug("Copying file from '{0}' to '{1}'", filename, destinationPath);
                this.fileSystem.CopyFile(filename, destinationPath);
            }
        }
        #endregion

        #region StartOutputStream()
        /// <summary>
        /// Starts an output stream.
        /// </summary>
        /// <param name="outputName">Name of the output stream.</param>
        /// <returns>
        /// The <see cref="Stream"/> to retrieve the output.
        /// </returns>
        public Stream StartOutputStream(string outputName)
        {
            logger.Trace("Starting output stream: {0}", outputName);
            this.writer.WriteStartElement("file");
            this.writer.WriteAttributeString("time", this.clock.Now.ToString("s"));
            this.writer.WriteString(outputName);
            this.writer.WriteEndElement();

            var destinationPath = GenerateArtifactFileName(this.project,
                                                           this.buildName,
                                                           outputName);
            var stream = this.fileSystem.OpenFileForWrite(destinationPath);
            return stream;
        }
        #endregion
        #endregion
    }
}
