using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NCover
{
	public class NCoverReportBuildPlugin : XslReportBuildPlugin
	{
		public NCoverReportBuildPlugin(IBuildLogTransformer buildLogTransformer)
			: base (buildLogTransformer, @"xsl\NCover.xsl", "View NCover Report", "ViewNCoverBuildReport")
		{ }
	}
}
