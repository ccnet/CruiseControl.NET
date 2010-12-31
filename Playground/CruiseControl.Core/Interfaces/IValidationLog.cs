namespace CruiseControl.Core.Interfaces
{
    using System;

    /// <summary>
    /// The log to add validation messages to.
    /// </summary>
    public interface IValidationLog
    {
        #region Public methods
        #region AddError()
        /// <summary>
        /// Adds an error.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        void AddError(string message, params object[] args);

        /// <summary>
        /// Adds an error from an exception.
        /// </summary>
        /// <param name="error">The error.</param>
        void AddError(Exception error);
        #endregion

        #region AddWarning()
        /// <summary>
        /// Adds a warning.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        void AddWarning(string message, params object[] args);

        /// <summary>
        /// Adds a warning from an exception.
        /// </summary>
        /// <param name="error">The error.</param>
        void AddWarning(Exception error);
        #endregion
        #endregion
    }
}
