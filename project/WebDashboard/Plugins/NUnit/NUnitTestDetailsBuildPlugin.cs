using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NUnit
{
	public class NUnitTestDetailsBuildPlugin : XslReportBuildPlugin
	{
		public NUnitTestDetailsBuildPlugin(IBuildLogTransformer buildLogTransformer)
			: base (buildLogTransformer, @"xsl\tests.xsl", "View Test Details", "ViewTestDetailsBuildReport")
		{ }
	}
}
