using System;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin
{
	public class SiteTemplateResults
	{
		private readonly string latestLogLink;
		private readonly string previousLogLink;
		private readonly string nextLogLink;
		private readonly string buildStatsClass;
		private readonly string buildStatsHtml;
		private readonly bool projectMode;
		private readonly HtmlAnchor[] buildLinkList;
		private readonly string pluginLinksHtml;

		public SiteTemplateResults(bool projectMode, HtmlAnchor[] buildLinkList, string buildStatsHtml, string buildStatsClass, 
			string pluginLinksHtml, string latestLogLink, string nextLogLink, string previousLogLink)
		{
			this.buildLinkList = buildLinkList;
			this.projectMode = projectMode;
			this.buildStatsHtml = buildStatsHtml;
			this.buildStatsClass = buildStatsClass;
			this.pluginLinksHtml = pluginLinksHtml;
			this.latestLogLink = latestLogLink;
			this.nextLogLink = nextLogLink;
			this.previousLogLink = previousLogLink;
		}

		public bool ProjectMode
		{
			get { return projectMode; }
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

		public string PluginLinksHtml
		{
			get { return pluginLinksHtml; }
		}

		public string LatestLogLink
		{
			get { return latestLogLink; }
		}

		public string PreviousLogLink
		{
			get { return previousLogLink; }
		}

		public string NextLogLink
		{
			get { return nextLogLink; }
		}
	}
}
