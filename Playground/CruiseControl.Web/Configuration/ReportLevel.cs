namespace CruiseControl.Web.Configuration
{
    using System.Collections.Generic;
    using System.Windows.Markup;

    /// <summary>
    /// Defines the reports in a level.
    /// </summary>
    [ContentProperty("Reports")]
    public class ReportLevel
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportLevel"/> class.
        /// </summary>
        public ReportLevel()
        {
            this.Reports = new List<Report>();
        }
        #endregion

        #region Public properties
        #region Level
        /// <summary>
        /// Gets or sets the target level.
        /// </summary>
        /// <value>The target level.</value>
        public ActionHandlerTargets Target { get; set; }
        #endregion

        #region Reports
        /// <summary>
        /// Gets or sets the reports.
        /// </summary>
        /// <value>The reports.</value>
        public IList<Report> Reports { get; private set; }
        #endregion
        #endregion
    }
}