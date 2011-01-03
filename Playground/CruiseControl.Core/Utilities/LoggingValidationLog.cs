namespace CruiseControl.Core.Utilities
{
    using System;
    using CruiseControl.Core.Interfaces;
    using NLog;

    /// <summary>
    /// A <see cref="IValidationLog"/> implementation that logs any messages.
    /// </summary>
    public class LoggingValidationLog
        : IValidationLog
    {
        #region Private fields
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        #endregion

        #region Public properties
        #region NumberOfWarnings
        /// <summary>
        /// Gets the number of warnings.
        /// </summary>
        /// <value>
        /// The number of warnings.
        /// </value>
        public int NumberOfWarnings { get; private set; }
        #endregion

        #region NumberOfErrors
        /// <summary>
        /// Gets the number of errors.
        /// </summary>
        /// <value>
        /// The number of errors.
        /// </value>
        public int NumberOfErrors { get; private set; }
        #endregion
        #endregion

        #region Public methods
        #region AddError()
        /// <summary>
        /// Adds an error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public void AddError(string message, params object[] args)
        {
            logger.Error(message, args);
            this.NumberOfErrors++;
        }

        /// <summary>
        /// Adds an error from an exception.
        /// </summary>
        /// <param name="error">The error.</param>
        public void AddError(Exception error)
        {
            logger.ErrorException(error.Message, error);
            this.NumberOfErrors++;
        }
        #endregion

        #region AddWarning()
        /// <summary>
        /// Adds a warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        public void AddWarning(string message, params object[] args)
        {
            logger.Warn(message, args);
            this.NumberOfWarnings++;
        }

        /// <summary>
        /// Adds a warning from an exception.
        /// </summary>
        /// <param name="error">The error.</param>
        public void AddWarning(Exception error)
        {
            logger.WarnException(error.Message, error);
            this.NumberOfWarnings++;
        }
        #endregion

        #region Reset()
        /// <summary>
        /// Resets the log.
        /// </summary>
        public void Reset()
        {
            this.NumberOfErrors = 0;
            this.NumberOfWarnings = 0;
        }
        #endregion
        #endregion
    }
}
