using System.Collections;
using System.Drawing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ProjectGrid : IProjectGrid
	{
		private readonly IUrlBuilder urlBuilder;
		private readonly ILinkFactory linkFactory;

		public ProjectGrid(IUrlBuilder urlBuilder, ILinkFactory linkFactory)
		{
			this.urlBuilder = urlBuilder;
			this.linkFactory = linkFactory;
		}

		public ProjectGridRow[] GenerateProjectGridRows(ProjectStatusOnServer[] statusList, string forceBuildActionName,
			ProjectGridSortColumn sortColumn, bool sortIsAscending)
		{
			ArrayList rows = new ArrayList();
			foreach (ProjectStatusOnServer statusOnServer in statusList)
			{
				ProjectStatus status = statusOnServer.ProjectStatus;
				IServerSpecifier serverSpecifier = statusOnServer.ServerSpecifier;
				string projectName = status.Name;
				rows.Add(
					new ProjectGridRow(
						projectName, 
						status.BuildStatus.ToString(), 
						CalculateHtmlColor(status.BuildStatus), 
						status.LastBuildDate, 
						(status.LastBuildLabel != null ? status.LastBuildLabel : "no build available") , 
						status.Status.ToString(), 
						status.Activity.ToString(), 
						urlBuilder.BuildFormName(
							new ActionSpecifierWithName(forceBuildActionName), 
							serverSpecifier.ServerName, 
							projectName),
						linkFactory.CreateProjectLink(
							new DefaultProjectSpecifier(serverSpecifier, projectName), new ActionSpecifierWithName(ProjectReportProjectPlugin.ACTION_NAME)
						).Url
					));
			}

			rows.Sort(GetComparer(sortColumn, sortIsAscending));

			return (ProjectGridRow[]) rows.ToArray(typeof (ProjectGridRow));
		}

		private string CalculateHtmlColor(IntegrationStatus status)
		{
			if (status == IntegrationStatus.Success)
			{
				return Color.Green.Name;
			}
			else if (status == IntegrationStatus.Unknown)
			{
				return Color.Yellow.Name;
			}
			else
			{
				return Color.Red.Name;
			}
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
					return rowx.Name.CompareTo(rowy.Name) * (ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.LastBuildDate)
				{
					return rowx.LastBuildDate.CompareTo(rowy.LastBuildDate) * (ascending ? 1 : -1);
				}
				else if (column == ProjectGridSortColumn.BuildStatus)
				{
					return rowx.BuildStatus.CompareTo(rowy.BuildStatus) * (ascending ? 1 : -1);
				}
				else
				{
					return 0;
				}
			}
		}
	}
}
