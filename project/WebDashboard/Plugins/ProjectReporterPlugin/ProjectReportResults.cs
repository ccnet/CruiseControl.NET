using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin
{
	public class ProjectReportResults
	{
		private readonly string pluginLinksHtml;
		private readonly string headerCellHtml;
		private readonly string detailsCellHtml;

		public string HeaderCellHtml
		{
			get { return headerCellHtml; }
		}

		public string DetailsCellHtml
		{
			get { return detailsCellHtml; }
		}

		public string PluginLinksHtml
		{
			get { return pluginLinksHtml; }
		}

		public ProjectReportResults(string headerCellHtml, string detailsCellHtml, string pluginLinksHtml)
		{
			this.detailsCellHtml = detailsCellHtml;
			this.headerCellHtml = headerCellHtml;
			this.pluginLinksHtml = pluginLinksHtml;
		}
	}
}
