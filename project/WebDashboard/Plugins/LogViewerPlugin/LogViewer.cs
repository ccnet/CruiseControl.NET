using System;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewer : IBuildPlugin
	{
		public string Description
		{
			get { return "View Log"; }
		}

		public string CreateURL (string serverName, string projectName, string buildName, IBuildUrlGenerator urlGenerator)
		{
			return urlGenerator.GenerateUrl("ViewLog.aspx", serverName, projectName, buildName);
		}
	}
}
