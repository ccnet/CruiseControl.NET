using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NAnt
{
	public class NAntReportBuildPlugin : XslReportBuildPlugin
	{
		public NAntReportBuildPlugin(IRequestTransformer requestTransformer)
			: base (requestTransformer, @"xsl\NAnt.xsl", "View NAnt Report", "ViewNAntBuildReport")
		{ }
	}
}
