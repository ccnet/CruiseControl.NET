using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport
{
	public class ViewFxCopBuildReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewFxCopBuildReport";

		private readonly IRequestTransformer requestTransformer;

		public ViewFxCopBuildReportAction(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\FxCopReport.xsl");
		}
	}
}
