using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NUnit
{
	public class NUnitTestTimingsBuildPlugin : XslReportBuildPlugin
	{
		public NUnitTestTimingsBuildPlugin(IRequestTransformer requestTransformer)
			: base (requestTransformer, @"xsl\timing.xsl", "View Test Timings", "ViewTestTimingsBuildReport")
		{ }
	}
}
