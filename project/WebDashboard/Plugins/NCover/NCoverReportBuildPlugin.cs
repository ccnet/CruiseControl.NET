using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NCover
{
	public class NCoverReportBuildPlugin : XslReportBuildPlugin
	{
		public NCoverReportBuildPlugin(IRequestTransformer requestTransformer)
			: base (requestTransformer, @"xsl\NCover.xsl", "View NCover Report", "ViewNCoverBuildReport")
		{ }
	}
}
