using System;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin
{
	public class LatestProjectReporter : IProjectPlugin
	{
		public string CreateURL (string serverName, string projectName, IProjectUrlGenerator urlGenerator)
		{
			return urlGenerator.GenerateUrl("ProjectReport.aspx", serverName, projectName);
		}

		public string Description
		{
			get { return "Latest"; }
		}
	}
}
