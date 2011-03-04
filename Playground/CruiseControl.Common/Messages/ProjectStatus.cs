namespace CruiseControl.Common.Messages
{
    using System;

    /// <summary>
    /// The current status of the project.
    /// </summary>
    public class ProjectStatus
    {
        #region Public properties
        #region Status
        /// <summary>
        /// Gets or sets the current status.
        /// </summary>
        /// <value>
        /// The current status.
        /// </value>
        public string Status { get; set; }
        #endregion

        #region LastBuildDate
        /// <summary>
        /// Gets or sets the last build date.
        /// </summary>
        /// <value>
        /// The last build date.
        /// </value>
        public DateTime? LastBuildDate { get; set; }
        #endregion
        #endregion
    }
}
