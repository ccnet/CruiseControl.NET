using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NAnt
{
	public class NAntReportBuildPlugin : XslReportBuildPlugin
	{
		public NAntReportBuildPlugin(IBuildLogTransformer buildLogTransformer)
			: base (buildLogTransformer, @"xsl\NAnt.xsl", "View NAnt Report", "ViewNAntBuildReport")
		{ }
	}
}
