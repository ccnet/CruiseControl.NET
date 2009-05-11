using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Log status messages.
    /// </summary>
    public interface ILogger
    {
        #region Public methods
        #region Debug()
        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        void Debug(string message, params object[] values);
        #endregion

        #region Info()
        /// <summary>
        /// Log a informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        void Info(string message, params object[] values);
        #endregion

        #region Warning()
        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        void Warning(string message, params object[] values);

        /// <summary>
        /// Log an exception as a warning.
        /// </summary>
        /// <param name="error">The exception details</param>
        void Warning(Exception error);
        #endregion

        #region Error()
        /// <summary>
        /// Log an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        void Error(string message, params object[] values);

        /// <summary>
        /// Log an exception as an error.
        /// </summary>
        /// <param name="error">The exception details</param>
        void Error(Exception error);
        #endregion
        #endregion
    }
}
