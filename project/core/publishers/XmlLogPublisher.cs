//-----------------------------------------------------------------------
// <copyright file="XmlLogPublisher.cs" company="CruiseControl.NET">
//     Copyright (c) 2009 CruiseControl.NET. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace ThoughtWorks.CruiseControl.Core.Publishers
{
    using System.IO;
    using Exortech.NetReflector;
    using ThoughtWorks.CruiseControl.Core.Tasks;
    using ThoughtWorks.CruiseControl.Core.Util;
    using ThoughtWorks.CruiseControl.Remote;

    /// <summary>
    /// Publishes the current XML log.
    /// </summary>
    /// <remarks>
    /// This will only publish the current log details, the full log will be published automatically by the project when it
    /// has finished running.
    /// </remarks>
    /// <example>
    /// <para>
    /// This publisher is configured by the following:
    /// </para>
    /// <code>
    /// &lt;xmllogger /&gt;
    /// </code>
    /// <para>
    /// It is also possible to change the location where the log is published to:
    /// </para>
    /// <code>
    /// &lt;xmllogger logDir="newLogLocation" /&gt;
    /// </code>
    /// </example>
    [ReflectorType("xmllogger")]
    public class XmlLogPublisher 
        : TaskBase
    {
        #region Private fields
        #region fileSystem
        /// <summary>
        /// The <see cref="IFileSystem"/> to use when manipulating the file system.
        /// </summary>
        private readonly IFileSystem fileSystem;
        #endregion
        #endregion

        #region Public properties
        #region ConfiguredLogDirectory
        /// <summary>
        /// Gets or sets the configured log directory.
        /// </summary>
        /// <value>The configured log directory.</value>
        [ReflectorProperty("logDir", Required = false)]
        public string ConfiguredLogDirectory { get; set; }
        #endregion
        #endregion

        #region Protected methods
        #region Execute()
        /// <summary>
        /// Execute the actual task functionality.
        /// </summary>
        /// <param name="result">The result to use while executing this publisher.</param>
        /// <returns>
        /// True if the task was successful, false otherwise.
        /// </returns>
        protected override bool Execute(IIntegrationResult result)
        {
            var fileSystem = this.fileSystem ?? new SystemIoFileSystem();

            // only deal with known integration status
            if (result.Status == IntegrationStatus.Unknown)
            {
                return true;
            }

            // Generate the log folder
            var logLocation = this.Context.GenerateLogFolder(this.ConfiguredLogDirectory ?? this.Context.Project.LogFolder);

            // Start the log writer
            var logName = this.Context.GenerateLogFilename();
            fileSystem.DeleteFile(logName);
            using (var writer = new StreamWriter(fileSystem.OpenOutputStream(logName)))
            {
                this.Context.WriteCurrentLog(writer);
            }

            return true;
        }
        #endregion
        #endregion
    }
}