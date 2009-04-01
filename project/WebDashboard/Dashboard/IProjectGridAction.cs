using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

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
        #endregion

        IResponse Execute(string actionName, IRequest request);
		IResponse Execute(string actionName, IServerSpecifier serverSpecifer, IRequest request);
	}
}
