using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport
{
	public class ViewTestTimingsBuildReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewTestTimingsBuildReport";

		private readonly IRequestTransformer requestTransformer;

		public ViewTestTimingsBuildReportAction(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\timing.xsl");
		}
	}
}
