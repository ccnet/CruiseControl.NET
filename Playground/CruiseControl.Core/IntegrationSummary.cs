namespace CruiseControl.Core
{
    using System;

    /// <summary>
    /// The summary of an integration.
    /// </summary>
    public class IntegrationSummary
    {
        #region Public properties
        #region StartTime
        /// <summary>
        /// Gets or sets the time the integration started.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public DateTime StartTime { get; set; }
        #endregion

        #region FinishTime
        /// <summary>
        /// Gets or sets the time the integration finished.
        /// </summary>
        /// <value>
        /// The finish time.
        /// </value>
        public DateTime FinishTime { get; set; }
        #endregion

        #region Status
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public IntegrationStatus Status { get; set; }
        #endregion
        #endregion
    }
}
