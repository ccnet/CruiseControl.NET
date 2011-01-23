namespace CruiseControl.Core
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using CruiseControl.Core.Exceptions;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// Application for running the server.
    /// </summary>
    public class Application
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private IEnumerable<Project> projects;
        #endregion

        #region Public properties
        #region Configuration
        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public Server Configuration { get; set; }
        #endregion

        #region ConfigurationService
        /// <summary>
        /// Gets or sets the configuration service.
        /// </summary>
        /// <value>
        /// The configuration service.
        /// </value>
        public IConfigurationService ConfigurationService { get; set; }
        #endregion

        #region ValidationLog
        /// <summary>
        /// Gets or sets the validation log.
        /// </summary>
        /// <value>
        /// The validation log.
        /// </value>
        public IValidationLog ValidationLog { get; set; }
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
        #endregion

        #region Public methods
        #region LoadConfiguration()
        /// <summary>
        /// Loads the configuration.
        /// </summary>
        public void LoadConfiguration()
        {
            ValidateField(this.ConfigurationService, "ConfigurationService");
            ValidateField(this.FileSystem, "FileSystem");
            ValidateField(this.ValidationLog, "ValidationLog");
            var configPath = Path.Combine(
                Environment.CurrentDirectory,
                "ccnet.config");

            this.LoadFromFile(configPath);
            this.ValidateConfiguration();
        }
        #endregion

        #region Start()
        /// <summary>
        /// Starts the application.
        /// </summary>
        public void Start()
        {
            ValidateField(this.Configuration, "Configuration");

            logger.Info("Starting application");
            try
            {
                logger.Debug("Starting projects");
                this.projects = this.Configuration
                    .Children
                    .SelectMany(c => c.ListProjects());
                foreach (var project in projects
                    .Where(project => project.CanStart()))
                {
                    project.Start();
                }

                logger.Debug("Starting communications");
                this.Configuration.OpenCommunications();
            }
            catch (Exception error)
            {
                logger.FatalException("A fatal error occurred while starting the application", error);
                this.Stop();
                throw;
            }
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            ValidateField(this.Configuration, "Configuration");

            logger.Debug("Stopping communications");
            this.Configuration.CloseCommunications();
            logger.Info("Stopping application");
            if (this.projects != null)
            {
                foreach (var project in this.projects
                    .Where(project => project.CanStop()))
                {
                    project.Stop();
                }
            }
        }
        #endregion
        #endregion

        #region Private methods
        #region ValidateField()
        /// <summary>
        /// Validates a field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="fieldName">Name of the field.</param>
        private static void ValidateField(object field, string fieldName)
        {
            if (field == null)
            {
                throw new CruiseControlException(fieldName + " has not been initialised");
            }
        }
        #endregion

        #region LoadFromFile()
        /// <summary>
        /// Loads the configuration from a file.
        /// </summary>
        /// <param name="configPath">The path to the configuration file.</param>
        private void LoadFromFile(string configPath)
        {
            logger.Info("Loading configuration from '{0}'", configPath);
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                using (var stream = this.FileSystem.OpenFileForRead(configPath))
                {
                    this.Configuration = this.ConfigurationService.Load(stream);
                }

                timer.Stop();
                logger.Info("Configuration loaded in {0:#,##0}ms", timer.ElapsedMilliseconds);
            }
            catch (Exception error)
            {
                logger.FatalException("Unable to load configuration", error);
                throw;
            }
        }
        #endregion

        #region ValidateConfiguration()
        /// <summary>
        /// Validates the configuration.
        /// </summary>
        private void ValidateConfiguration()
        {
            logger.Info("Validating configuration");
            var timer = new Stopwatch();
            timer.Start();
            try
            {
                this.ValidationLog.Reset();
                this.Configuration.Validate(this.ValidationLog);
                timer.Stop();
                logger.Info("Configuration validated in {0:#,##0}ms", timer.ElapsedMilliseconds);
            }
            catch (Exception error)
            {
                logger.FatalException("Unable to validate configuration", error);
                throw;
            }

            if (this.ValidationLog.NumberOfErrors > 0)
            {
                logger.Fatal(
                    "{0} error(s) were found in the configuration, unable to start server",
                    this.ValidationLog.NumberOfErrors);
                throw new ConfigurationException("Fatal errors found in configuration");
            }

            if (this.ValidationLog.NumberOfWarnings > 0)
            {
                logger.Warn(
                    "{0} warnings were found in the configuration, the server may not perform as expected",
                    this.ValidationLog.NumberOfWarnings);
            }
        }
        #endregion
        #endregion
    }
}
