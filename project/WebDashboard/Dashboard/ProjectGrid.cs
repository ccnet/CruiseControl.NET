using System.Collections;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Resources;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectGrid : IProjectGrid
	{
		public ProjectGridRow[] GenerateProjectGridRows(ProjectStatusOnServer[] statusList, string forceBuildActionName,
		                                                ProjectGridSortColumn sortColumn, bool sortIsAscending, string categoryFilter,
                                                        ICruiseUrlBuilder urlBuilder, Translations translations) 
		{
			ArrayList rows = new ArrayList();
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

			rows.Sort(GetComparer(sortColumn, sortIsAscending));

			return (ProjectGridRow[]) rows.ToArray(typeof (ProjectGridRow));
		}

		private IComparer GetComparer(ProjectGridSortColumn column, bool ascending)
		{
			return new ProjectGridRowComparer(column, ascending);
		}

		private class ProjectGridRowComparer : IComparer
		{
			private readonly ProjectGridSortColumn column;
			private readonly bool ascending;

			public ProjectGridRowComparer(ProjectGridSortColumn column, bool ascending)
			{
				this.column = column;
				this.ascending = ascending;
			}

			public int Compare(object x, object y)
			{
				ProjectGridRow rowx = x as ProjectGridRow;
				ProjectGridRow rowy = y as ProjectGridRow;

				if (column == ProjectGridSortColumn.Name)
				{
					return rowx.Name.CompareTo(rowy.Name)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.LastBuildDate)
				{
					return rowx.LastBuildDate.CompareTo(rowy.LastBuildDate)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.BuildStatus)
				{
					return rowx.BuildStatus.CompareTo(rowy.BuildStatus)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.ServerName)
				{
					return rowx.ServerName.CompareTo(rowy.ServerName)*(ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.Category)
                {
                    return rowx.Category.CompareTo(rowy.Category)*(ascending ? 1 : -1);
                } 
				else
				{
					return 0;
				}
			}
		}
	}
}
