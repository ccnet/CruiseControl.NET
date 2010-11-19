namespace CruiseControl.Web.Configuration
{
    /// <summary>
    /// A report that can be displayed.
    /// </summary>
    public class Report
    {
        #region Public properties
        #region DisplayName
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
        #endregion

        #region ActionName
        /// <summary>
        /// Gets or sets the action name.
        /// </summary>
        /// <value>The action name.</value>
        public string ActionName { get; set; }
        #endregion

        #region IsDefault
        /// <summary>
        /// Gets or sets a value indicating whether this report is the default for its level.
        /// </summary>
        /// <value>
        /// <c>true</c> if this report is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }
        #endregion
        #endregion
    }
}