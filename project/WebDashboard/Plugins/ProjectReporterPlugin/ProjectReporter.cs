using System;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin
{
	public class ProjectReporter : IPlugin
	{
		public string Description
		{
			get { return "Build Report"; }
		}
		public string Url
		{
			get { return "ProjectReport.aspx"; }
		}
	}
}
