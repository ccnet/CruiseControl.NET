using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultUserRequestSpecificSideBarViewBuilder : HtmlBuilderViewBuilder, IUserRequestSpecificSideBarViewBuilder
	{
		private readonly IBuildPluginLinkCalculator buildPluginLinkCalculator;
		private readonly IRecentBuildsViewBuilder recentBuildsViewBuilder;
		private readonly IBuildNameRetriever buildNameRetriever;
		private readonly IUrlBuilder urlBuilder;

		public DefaultUserRequestSpecificSideBarViewBuilder(
			IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder, IBuildNameRetriever buildNameRetriever, IRecentBuildsViewBuilder recentBuildsViewBuilder, IBuildPluginLinkCalculator buildPluginLinkCalculator) 
			: base (htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
			this.buildNameRetriever = buildNameRetriever;
			this.recentBuildsViewBuilder = recentBuildsViewBuilder;
			this.buildPluginLinkCalculator = buildPluginLinkCalculator;
		}

		public HtmlTable GetFarmSideBar()
		{
			return Table(
				TR( TD( A("Add Project", urlBuilder.BuildUrl(new ActionSpecifierWithName(DisplayAddProjectPageAction.ACTION_NAME))))));
		}

		public HtmlTable GetServerSideBar(IServerSpecifier serverSpecifier)
		{
			return Table(
				TR( TD( A("View Server Log", urlBuilder.BuildServerUrl(new ActionSpecifierWithName(ViewServerLogAction.ACTION_NAME), serverSpecifier)))),
				TR( TD( A("Add Project", urlBuilder.BuildServerUrl(new ActionSpecifierWithName(DisplayAddProjectPageAction.ACTION_NAME), serverSpecifier)))));
		}

		public HtmlTable GetProjectSideBar(IProjectSpecifier projectSpecifier)
		{
			return Table(
				TR( TD( A("Edit Project", urlBuilder.BuildProjectUrl(new ActionSpecifierWithName(DisplayEditProjectPageAction.ACTION_NAME), projectSpecifier)))),
				TR( TD( A("Delete Project", urlBuilder.BuildProjectUrl(new ActionSpecifierWithName(ShowDeleteProjectAction.ACTION_NAME), projectSpecifier)))),
				TR( TD( "&nbsp;")),
				TR( TD( recentBuildsViewBuilder.BuildRecentBuildsTable(projectSpecifier)))
				);
		}

		public HtmlTable GetBuildSideBar(IBuildSpecifier buildSpecifier)
		{
			IAbsoluteLink[] pluginLinks = buildPluginLinkCalculator.GetBuildPluginLinks(buildSpecifier);

			HtmlTable table = Table(
				TR( TD( A("Latest", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), buildNameRetriever.GetLatestBuildSpecifier(buildSpecifier.ProjectSpecifier))))),
				TR( TD( A("Next", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), buildNameRetriever.GetNextBuildSpecifier(buildSpecifier))))),
				TR( TD( A("Previous", urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), buildNameRetriever.GetPreviousBuildSpecifier(buildSpecifier))))),
				TR( TD( "&nbsp;")));

			foreach (IAbsoluteLink link in pluginLinks)
			{
				table.Rows.Add(TR( TD( A(link.Description, link.AbsoluteURL))));
			}

			table.Rows.Add(TR( TD( "&nbsp;")));
			table.Rows.Add(TR( TD( recentBuildsViewBuilder.BuildRecentBuildsTable(buildSpecifier.ProjectSpecifier))));

			return table;
		}
	}
}
