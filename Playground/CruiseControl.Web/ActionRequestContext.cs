namespace CruiseControl.Web
{
    /// <summary>
    /// The context for an action request.
    /// </summary>
    public class ActionRequestContext
    {
        #region Public properties
        #region Server
        /// <summary>
        /// Gets or sets the server.
        /// </summary>
        /// <value>The server.</value>
        public string Server { get; set; }
        #endregion

        #region Project
        /// <summary>
        /// Gets or sets the project.
        /// </summary>
        /// <value>The project.</value>
        public string Project { get; set; }
        #endregion

        #region Build
        /// <summary>
        /// Gets or sets the build.
        /// </summary>
        /// <value>The build.</value>
        public string Build { get; set; }
        #endregion

        #region Report
        /// <summary>
        /// Gets or sets the report.
        /// </summary>
        /// <value>The report.</value>
        public string Report { get; set; }
        #endregion

        #region Level
        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public ActionHandlerTargets Level { get; set; }
        #endregion
        #endregion
    }
}