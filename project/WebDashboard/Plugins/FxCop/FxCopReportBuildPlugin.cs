using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FxCop
{
	public class FxCopReportBuildPlugin : XslReportBuildPlugin
	{
		public FxCopReportBuildPlugin(IBuildLogTransformer buildLogTransformer)
			: base (buildLogTransformer, @"xsl\FxCopReport.xsl", "View FxCop Report", "ViewFxCopBuildReport")
		{ }
	}
}
