using System;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReporterPlugin
{
	// ToDo - see all notes in NextProjectReporter
	public class PreviousProjectReporter  : IBuildPlugin, IBuildNameRetrieverSettable
	{
		public IBuildNameRetriever buildNameRetriever;

		public string CreateURL (string serverName, string projectName, string buildName, IBuildUrlGenerator urlGenerator)
		{
			// ToDo - test
			// ToDo - sort out Build Name Specs (this is also mentioned elsewhere)
			return urlGenerator.GenerateUrl("ProjectReport.aspx", serverName, projectName, buildNameRetriever.GetPreviousBuildName(new Build(buildName, "", serverName, projectName)));
		}

		public string Description
		{
			get { return "Previous"; }
		}

		// ToDo - something else - see note in SiteTemplate
		public IBuildNameRetriever BuildNameRetriever
		{
			set { this.buildNameRetriever = value; }
		}
	}
}
