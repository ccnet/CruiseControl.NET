using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FxCop
{
	public class FxCopReportBuildPlugin : XslReportBuildPlugin
	{
		public FxCopReportBuildPlugin(IRequestTransformer requestTransformer)
			: base (requestTransformer, @"xsl\FxCopReport.xsl", "View FxCop Report", "ViewFxCopBuildReport")
		{ }
	}
}
