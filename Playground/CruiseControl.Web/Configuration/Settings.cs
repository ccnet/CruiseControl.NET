namespace CruiseControl.Web.Configuration
{
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// The settings for the web console.
    /// </summary>
    public class Settings
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class.
        /// </summary>
        public Settings()
        {
            this.ReportLevels = new List<ReportLevel>();
            this.Servers = new List<Server>();
        }
        #endregion

        #region Public properties
        #region ReportLevels
        /// <summary>
        /// Gets or sets the report levels.
        /// </summary>
        /// <value>The report levels.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IList<ReportLevel> ReportLevels { get; private set; }

        /// <summary>
        /// Check if ReportLevels should be serialised.
        /// </summary>
        /// <returns>
        /// <c>true</c> if it should be serialized; <c>false</c> otherwise.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeReportLevels()
        {
            return (this.ReportLevels != null) &&
                   (this.ReportLevels.Count > 0);
        }
        #endregion

        #region Servers
        /// <summary>
        /// Gets or sets the servers.
        /// </summary>
        /// <value>The servers.</value>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public IList<Server> Servers { get; private set; }

        /// <summary>
        /// Check if Servers should be serialised.
        /// </summary>
        /// <returns>
        /// <c>true</c> if it should be serialized; <c>false</c> otherwise.
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShouldSerializeServers()
        {
            return (this.Servers != null) &&
                   (this.Servers.Count > 0);
        }
        #endregion
        #endregion
    }
}