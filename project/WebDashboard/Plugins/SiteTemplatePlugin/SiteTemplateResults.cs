using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin
{
	public class SiteTemplateResults
	{
		private readonly string buildStatsClass;
		private readonly string buildStatsHtml;
		private readonly HtmlAnchor[] buildLinkList;

		public SiteTemplateResults(HtmlAnchor[] buildLinkList, string buildStatsHtml, string buildStatsClass)
		{
			this.buildLinkList = buildLinkList;
			this.buildStatsHtml = buildStatsHtml;
			this.buildStatsClass = buildStatsClass;
		}

		public string BuildStatsClass
		{
			get { return buildStatsClass; }
		}

		public string BuildStatsHtml
		{
			get { return buildStatsHtml; }
		}

		public HtmlAnchor[] BuildLinkList
		{
			get { return buildLinkList; }
		}
	}
}
