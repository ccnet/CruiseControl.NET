using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class RecentBuildLister : HtmlBuilderViewBuilder, IRecentBuildsViewBuilder, IAllBuildsViewBuilder
	{
		private readonly IBuildNameFormatter buildNameFormatter;
		private readonly IFarmService farmService;
		private readonly IUrlBuilder urlBuilder;

		public RecentBuildLister(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder, IFarmService farmService, IBuildNameFormatter buildNameFormatter) : base(htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
			this.farmService = farmService;
			this.buildNameFormatter = buildNameFormatter;
		}

		public HtmlTable BuildRecentBuildsTable(IProjectSpecifier projectSpecifier)
		{
			return BuildBuildsTable(farmService.GetMostRecentBuildSpecifiers(projectSpecifier, 10));
		}

		public HtmlTable BuildAllBuildsTable(IProjectSpecifier projectSpecifier)
		{
			HtmlTable table = BuildBuildsTable(farmService.GetBuildSpecifiers(projectSpecifier));
			table.Rows.Insert(0, TR(TD("&nbsp;")));
			table.Rows.Insert(0, TR(TD(string.Format("<b>All builds for {0}</b>", projectSpecifier.ProjectName))));
			return table;
		}

		private HtmlTable BuildBuildsTable(IBuildSpecifier[] buildSpecifiers)
		{
			HtmlTable table = Table();
			foreach (IBuildSpecifier buildSpecifier in buildSpecifiers)
			{
				table.Rows.Add(TR(TD(A(
					buildNameFormatter.GetPrettyBuildName(buildSpecifier), 
					urlBuilder.BuildBuildUrl(new ActionSpecifierWithName(ViewBuildReportAction.ACTION_NAME), buildSpecifier),
					buildNameFormatter.GetCssClassForBuildLink(buildSpecifier)
					))));	
			}
			return table;
		}
	}
}
