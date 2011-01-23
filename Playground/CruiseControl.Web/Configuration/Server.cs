namespace CruiseControl.Web.Configuration
{
    /// <summary>
    /// Defines a server.
    /// </summary>
    public class Server
    {
        #region Public properties
        #region DisplayName
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; set; }
        #endregion

        #region Uri
        /// <summary>
        /// Gets or sets the URI for the server.
        /// </summary>
        /// <value>The server URI.</value>
        public string Uri { get; set; }
        #endregion
        #endregion
    }
}