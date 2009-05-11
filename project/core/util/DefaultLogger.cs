using System;

namespace ThoughtWorks.CruiseControl.Core.Util
{
    /// <summary>
    /// Logs messages using the default logger.
    /// </summary>
    /// <remarks>
    /// Current this class just hands the messages onto <see cref="Log"/>. This should be
    /// modified to log messages directly.
    /// </remarks>
    public class DefaultLogger
        : ILogger
    {
        #region Public methods
        #region Debug()
        /// <summary>
        /// Log a debug message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        public virtual void Debug(string message, params object[] values)
        {
            if ((values != null) && (values.Length > 0))
            {
                Log.Debug(string.Format(message, values));
            }
            else
            {
                Log.Debug(message);
            }
        }
        #endregion

        #region Info()
        /// <summary>
        /// Log a informational message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        public virtual void Info(string message, params object[] values)
        {
            if ((values != null) && (values.Length > 0))
            {
                Log.Info(string.Format(message, values));
            }
            else
            {
                Log.Info(message);
            }
        }
        #endregion

        #region Warning()
        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        public virtual void Warning(string message, params object[] values)
        {
            if ((values != null) && (values.Length > 0))
            {
                Log.Warning(string.Format(message, values));
            }
            else
            {
                Log.Warning(message);
            }
        }

        /// <summary>
        /// Log an exception as a warning.
        /// </summary>
        /// <param name="error">The exception details</param>
        public virtual void Warning(Exception error)
        {
            Log.Warning(error);
        }
        #endregion

        #region Error()
        /// <summary>
        /// Log an error message.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="values">Any optional parameters.</param>
        public virtual void Error(string message, params object[] values)
        {
            if ((values != null) && (values.Length > 0))
            {
                Log.Error(string.Format(message, values));
            }
            else
            {
                Log.Error(message);
            }
        }

        /// <summary>
        /// Log an exception as an error.
        /// </summary>
        /// <param name="error">The exception details</param>
        public virtual void Error(Exception error)
        {
            Log.Error(error);
        }
        #endregion
        #endregion
    }
}
