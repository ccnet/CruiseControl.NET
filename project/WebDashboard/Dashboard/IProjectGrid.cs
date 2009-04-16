using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectGrid
	{
        ProjectGridRow[] GenerateProjectGridRows(ProjectStatusOnServer[] statusList, string forceBuildActionName, ProjectGridSortColumn sortColumn, bool sortIsAscending, string categoryFilter, ICruiseUrlBuilder urlBuilder);
	}
}
