using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.NCover;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildLog;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultUserRequestSpecificSideBarViewBuilder : HtmlBuilderViewBuilder, IUserRequestSpecificSideBarViewBuilder
	{
		private readonly IRecentBuildsViewBuilder recentBuildsViewBuilder;
		private readonly IBuildNameRetriever buildNameRetriever;
		private readonly IUrlBuilder urlBuilder;

		public DefaultUserRequestSpecificSideBarViewBuilder(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder, IBuildNameRetriever buildNameRetriever, IRecentBuildsViewBuilder recentBuildsViewBuilder) 
			: base (htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
			this.buildNameRetriever = buildNameRetriever;
			this.recentBuildsViewBuilder = recentBuildsViewBuilder;
		}

		public HtmlTable GetFarmSideBar()
		{
			return Table(
				TR( TD( A("Add Project", urlBuilder.BuildUrl(new ActionSpecifierWithName(DisplayAddProjectPageAction.ACTION_NAME))))));
		}

		public HtmlTable GetServerSideBar(string serverName)
		{
			return Table(
				TR( TD( A("View Server Log", urlBuilder.BuildServerUrl(new ActionSpecifierWithName(ViewServerLogAction.ACTION_NAME), serverName)))),
				TR( TD( A("Add Project", urlBuilder.BuildServerUrl(new ActionSpecifierWithName(DisplayAddProjectPageAction.ACTION_NAME), serverName)))));
		}

		public HtmlTable GetProjectSideBar(string serverName, string projectName)
		{
			return Table(
				TR( TD( A("Edit Project", urlBuilder.BuildProjectUrl(new ActionSpecifierWithName(DisplayEditProjectPageAction.ACTION_NAME), serverName, projectName)))),
				TR( TD( A("Delete Project", urlBuilder.BuildProjectUrl(new ActionSpecifierWithName(ShowDeleteProjectAction.ACTION_NAME), serverName, projectName)))),
				TR( TD( "&nbsp;")),
				TR( TD( recentBuildsViewBuilder.BuildRecentBuildsTable(serverName, projectName)))
				);
		}

		public HtmlTable GetBuildSideBar(string serverName, string projectName, string buildName)
		{
			return Table(
				TR( TD( A("Latest", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), serverName, projectName, buildNameRetriever.GetLatestBuildName(serverName, projectName))))),
				TR( TD( A("Next", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), serverName, projectName, buildNameRetriever.GetNextBuildName(serverName, projectName, buildName))))),
				TR( TD( A("Previous", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), serverName, projectName, buildNameRetriever.GetPreviousBuildName(serverName, projectName, buildName))))),
				TR( TD( "&nbsp;")),
				TR( TD( A("View Build Log", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildLogAction.ACTION_NAME), serverName, projectName, buildName)))),
				TR( TD( A("View Test Details", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewTestDetailsBuildReportAction.ACTION_NAME), serverName, projectName, buildName)))),
				TR( TD( A("View Test Timings", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewTestTimingsBuildReportAction.ACTION_NAME), serverName, projectName, buildName)))),
				TR( TD( A("View FxCop Report", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewFxCopBuildReportAction.ACTION_NAME), serverName, projectName, buildName)))),
				TR( TD( A("View NCover Report", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewNCoverBuildReportAction.ACTION_NAME), serverName, projectName, buildName)))),
				TR( TD( "&nbsp;")),
				TR( TD( recentBuildsViewBuilder.BuildRecentBuildsTable(serverName, projectName)))
					);
		}
	}
}
