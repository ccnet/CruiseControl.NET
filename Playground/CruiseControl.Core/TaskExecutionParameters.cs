namespace CruiseControl.Core
{
    using System.Xml;
    using CruiseControl.Core.Interfaces;

    /// <summary>
    /// The parameters for a task execution.
    /// </summary>
    public class TaskExecutionParameters
    {
        #region Public properties
        #region XmlWriter
        /// <summary>
        /// Gets or sets the XML writer.
        /// </summary>
        /// <value>
        /// The XML writer.
        /// </value>
        public XmlWriter XmlWriter { get; set; }
        #endregion

        #region IntegrationRequest
        /// <summary>
        /// Gets or sets the integration request.
        /// </summary>
        /// <value>
        /// The integration request.
        /// </value>
        public IntegrationRequest IntegrationRequest { get; set; }
        #endregion

        #region Project
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>
        /// The project.
        /// </value>
        public Project Project { get; set; }
        #endregion

        #region Clock
        /// <summary>
        /// Gets or sets the clock.
        /// </summary>
        /// <value>
        /// The clock.
        /// </value>
        public IClock Clock { get; set; }
        #endregion

        #region FileSystem
        /// <summary>
        /// Gets or sets the file system.
        /// </summary>
        /// <value>
        /// The file system.
        /// </value>
        public IFileSystem FileSystem { get; set; }
        #endregion

        #region BuildName
        /// <summary>
        /// Gets or sets the name of the build.
        /// </summary>
        /// <value>
        /// The name of the build.
        /// </value>
        public string BuildName { get; set; }
        #endregion
        #endregion
    }
}
