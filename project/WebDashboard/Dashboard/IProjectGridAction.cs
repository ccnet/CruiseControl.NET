using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectGridAction
    {
        #region Properties
        #region DefaultSortColumn
        /// <summary>
        /// The default column to sort by.
        /// </summary>
        ProjectGridSortColumn DefaultSortColumn { get; set; }
        #endregion

        #region SuccessIndicatorBarLocation
        /// <summary>
        /// Gets or sets the success indicator bar location.
        /// </summary>
        /// <value>The success indicator bar location.</value>
        IndicatorBarLocation SuccessIndicatorBarLocation { get; set; }
        #endregion
        #endregion

        IResponse Execute(string actionName, ICruiseRequest request);
        IResponse Execute(string actionName, IServerSpecifier serverSpecifer, ICruiseRequest request);
	}
}
