using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NUnit
{
	public class NUnitTestDetailsBuildPlugin : XslReportBuildPlugin
	{
		public NUnitTestDetailsBuildPlugin(IRequestTransformer requestTransformer)
			: base (requestTransformer, @"xsl\tests.xsl", "View Test Details", "ViewTestDetailsBuildReport")
		{ }
	}
}
