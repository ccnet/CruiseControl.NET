namespace CruiseControl.Core
{
    using System;

    /// <summary>
    /// The context that tasks run in.
    /// </summary>
    public class TaskExecutionContext
    {
        #region Public properties
        #region CurrentStatus
        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public IntegrationStatus CurrentStatus { get; set; }
        #endregion
        #endregion

        #region Public methods
        #region AddEntryToBuildLog()
        /// <summary>
        /// Adds an information entry to build log.
        /// </summary>
        /// <param name="message">The message of the entry.</param>
        public virtual void AddEntryToBuildLog(string message)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }
        #endregion
        #endregion
    }
}
