using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NAnt
{
	public class ViewNAntBuildReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewNAntBuildReport";

		private readonly IRequestTransformer requestTransformer;

		public ViewNAntBuildReportAction(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\NAnt.xsl");
		}
	}
}
