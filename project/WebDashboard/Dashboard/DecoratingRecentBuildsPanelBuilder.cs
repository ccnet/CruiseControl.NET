using System;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DecoratingRecentBuildsPanelBuilder : HtmlBuilderViewBuilder, IRecentBuildsPanelViewBuilder
	{
		private readonly IUrlBuilder urlBuilder;
		private readonly IRecentBuildsPanelViewBuilder builderToDecorate;

		public DecoratingRecentBuildsPanelBuilder(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder, IRecentBuildsPanelViewBuilder builderToDecorate) : base(htmlBuilder)
		{
			this.builderToDecorate = builderToDecorate;
			this.urlBuilder = urlBuilder;
		}

		public HtmlTable BuildRecentBuildsPanel(string serverName, string projectName)
		{
			HtmlTable subTable = builderToDecorate.BuildRecentBuildsPanel(serverName, projectName);
			subTable.Rows.Insert(0, TR(TD("Recent Builds")));
			subTable.Rows.Add(TR(TD("Show All")));
			return subTable;
		}
	}
}
