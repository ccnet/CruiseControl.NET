using System;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin
{
	public class ProjectReporter : IBuildPlugin
	{
		public string Description
		{
			get { return "Build Report"; }
		}

		public string CreateURL (string serverName, string projectName, string buildName, IBuildUrlGenerator urlGenerator)
		{
			return urlGenerator.GenerateUrl("ProjectReport.aspx", serverName, projectName, buildName);
		}
	}
}
