using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class RecentBuildLister : HtmlBuilderViewBuilder, IRecentBuildsPanelViewBuilder
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

		public HtmlTable BuildRecentBuildsPanel(string serverName, string projectName)
		{
			HtmlTable table = Table();

			string[] names = farmService.GetMostRecentBuildNames(serverName, projectName, 10);
			foreach (string name in names)
			{
				table.Rows.Add(TR(TD(A(buildNameFormatter.GetPrettyBuildName(name), urlBuilder.BuildBuildUrl("BuildReport.aspx",serverName, projectName, name)))));	
			}

			return table;
		}
	}
}
