using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NUnit
{
	public class NUnitTestTimingsBuildPlugin : XslReportBuildPlugin
	{
		public NUnitTestTimingsBuildPlugin(IBuildLogTransformer buildLogTransformer)
			: base (buildLogTransformer, @"xsl\timing.xsl", "View Test Timings", "ViewTestTimingsBuildReport")
		{ }
	}
}
