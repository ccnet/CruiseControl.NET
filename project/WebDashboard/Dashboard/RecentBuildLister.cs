using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
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

		public HtmlTable BuildRecentBuildsTable(string serverName, string projectName)
		{
			return BuildBuildsTable(serverName, projectName, farmService.GetMostRecentBuildNames(serverName, projectName, 10));
		}

		public HtmlTable BuildAllBuildsTable(string serverName, string projectName)
		{
			return BuildBuildsTable(serverName, projectName, farmService.GetBuildNames(serverName, projectName));
		}

		private HtmlTable BuildBuildsTable(string serverName, string projectName, string[] buildNames)
		{
			HtmlTable table = Table();
			foreach (string name in buildNames)
			{
				table.Rows.Add(TR(TD(A(
					buildNameFormatter.GetPrettyBuildName(name), 
					urlBuilder.BuildBuildUrl("BuildReport.aspx",serverName, projectName, name),
					buildNameFormatter.GetCssClassForBuildLink(name)
					))));	
			}
			return table;
		}
	}
}
