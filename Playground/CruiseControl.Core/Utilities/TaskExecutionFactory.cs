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
        /// <param name="project">The project.</param>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The new <see cref="TaskExecutionContext"/>.
        /// </returns>
        public TaskExecutionContext StartNew(Project project, IntegrationRequest request)
        {
            logger.Debug("Starting execution context for project '{0}", project.Name);
            var buildName = this.Clock.Now.ToString("yyyyMMddHHmmss");
            var logFile = TaskExecutionContext.GenerateArtifactFileName(project, buildName, "build.log");
            this.FileSystem.EnsureFolderExists(Path.GetDirectoryName(logFile));
            var writer = this.FileSystem.CreateXmlWriter(logFile);
            writer.WriteStartElement("project");
            writer.WriteAttributeString("name", project.Name);
            writer.WriteElementString("start", this.Clock.Now.ToString("s"));
            var parameters = new TaskExecutionParameters
                                 {
                                     XmlWriter = writer,
                                     FileSystem = this.FileSystem,
                                     Clock = this.Clock,
                                     IntegrationRequest = request,
                                     Project = project,
                                     BuildName = buildName
                                 };
            var context = new TaskExecutionContext(parameters);
            return context;
        }
        #endregion
        #endregion
    }
}
