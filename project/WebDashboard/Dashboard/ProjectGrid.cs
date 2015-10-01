using System.Collections.Generic;
using System.Linq;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
    public class ProjectGrid : IProjectGrid
	{
		public ProjectGridRow[] GenerateProjectGridRows(ProjectStatusOnServer[] statusList, string forceBuildActionName,
		                                                ProjectGridSortColumn sortColumn, bool sortIsAscending, string categoryFilter,
                                                        ICruiseUrlBuilder urlBuilder, Translations translations) 
		{
			var rows = new List<ProjectGridRow>();
			foreach (ProjectStatusOnServer statusOnServer in statusList)
			{
				ProjectStatus status = statusOnServer.ProjectStatus;
				IServerSpecifier serverSpecifier = statusOnServer.ServerSpecifier;
				DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, status.Name);
				
				if ((categoryFilter != string.Empty) && (categoryFilter != status.Category))
					continue;

				rows.Add(
					new ProjectGridRow(status,
					                   serverSpecifier,
                                       urlBuilder.BuildProjectUrl(ProjectReportProjectPlugin.ACTION_NAME, projectSpecifier),
                                       urlBuilder.BuildProjectUrl(ProjectParametersAction.ActionName, projectSpecifier),
                                       translations));
			}

            return rows.OrderBy(a => a, GetComparer(sortColumn, sortIsAscending)).ToArray();
		}

		private IComparer<ProjectGridRow> GetComparer(ProjectGridSortColumn column, bool ascending)
		{
			return new ProjectGridRowComparer(column, ascending);
		}

		private class ProjectGridRowComparer 
            : IComparer<ProjectGridRow>
		{
			private readonly ProjectGridSortColumn column;
			private readonly bool ascending;

			public ProjectGridRowComparer(ProjectGridSortColumn column, bool ascending)
			{
				this.column = column;
				this.ascending = ascending;
			}

            public int Compare(ProjectGridRow x, ProjectGridRow y)
			{
				if (column == ProjectGridSortColumn.Name)
				{
					return x.Name.CompareTo(y.Name)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.LastBuildDate)
				{
					return x.LastBuildDate.CompareTo(y.LastBuildDate)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.BuildStatus)
				{
					return x.BuildStatus.CompareTo(y.BuildStatus)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.ServerName)
				{
					return x.ServerName.CompareTo(y.ServerName)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.Category)
                {
                    return x.Category.CompareTo(y.Category)*(ascending ? 1 : -1);
                } 
				else
				{
					return 0;
				}
			}
		}
	}
}
