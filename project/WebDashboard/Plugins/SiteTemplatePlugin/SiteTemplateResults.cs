using System;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.SiteTemplatePlugin
{
	public class SiteTemplateResults
	{
		private readonly HtmlAnchor[] serverPluginsList;
		private readonly HtmlAnchor[] buildPluginList;
		private readonly string buildStatsClass;
		private readonly string buildStatsHtml;
		private readonly bool projectMode;
		private readonly HtmlAnchor[] buildLinkList;
		private readonly string pluginLinksHtml;

		public SiteTemplateResults(bool projectMode, HtmlAnchor[] buildLinkList, string buildStatsHtml, string buildStatsClass, 
			string pluginLinksHtml, HtmlAnchor[] buildPluginList)
		{
			this.buildLinkList = buildLinkList;
			this.projectMode = projectMode;
			this.buildStatsHtml = buildStatsHtml;
			this.buildStatsClass = buildStatsClass;
			this.pluginLinksHtml = pluginLinksHtml;
			this.buildPluginList = buildPluginList;
			this.serverPluginsList = serverPluginsList;
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

		public HtmlAnchor[] BuildPluginsList
		{
			get { return buildPluginList; }
		}

		public HtmlAnchor[] ServerPluginsList
		{
			get { return serverPluginsList; }
		}
	}
}
