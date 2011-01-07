namespace CruiseControl.Core.Utilities
{
    using System;
    using System.IO;
    using CruiseControl.Core.Interfaces;
    using Ninject;
    using NLog;

    /// <summary>
    /// Factory methods for working with task executions.
    /// </summary>
    public class TaskExecutionFactory
        : ITaskExecutionFactory
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public properties
        #region Clock
        /// <summary>
        /// Gets or sets the clock.
        /// </summary>
        /// <value>
        /// The clock.
        /// </value>
        [Inject]
        public IClock Clock { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
        [Inject]
        public IFileSystem FileSystem { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region StartNew()
        /// <summary>
        /// Starts a new <see cref="TaskExecutionContext"/>.
        /// </summary>
        /// <param name="logFile">The path to the log file.</param>
        /// <param name="project">The project.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The new <see cref="TaskExecutionContext"/>.
        /// </returns>
        public TaskExecutionContext StartNew(string logFile, Project project, IntegrationRequest request)
        {
            logger.Debug("Starting execution context for project '{0}", project.Name);
            this.FileSystem.EnsureFolderExists(Path.GetDirectoryName(logFile));
            var writer = this.FileSystem.CreateXmlWriter(logFile);
            writer.WriteStartElement("project");
            writer.WriteAttributeString("name", project.Name);
            writer.WriteElementString("start", this.Clock.Now.ToString("s"));
            var context = new TaskExecutionContext(writer, this.FileSystem, this.Clock, request);
            return context;
        }
        #endregion

        #region GenerateLogName()
        /// <summary>
        /// Generates a new log name for a project.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <returns>
        /// The name of the new log for the project.
        /// </returns>
        public string GenerateLogName(Project project)
        {
            var baseDir = Environment.CurrentDirectory;
            var logName = Path.Combine(
                baseDir,
                project.Name,
                this.Clock.Now.ToString("yyyyMMddHHmmss"),
                "build.log");
            return logName;
        }
        #endregion
        #endregion
    }
}
