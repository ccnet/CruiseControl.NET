using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.NCover
{
	public class ViewNCoverBuildReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewNCoverBuildReport";

		private readonly IRequestTransformer requestTransformer;

		public ViewNCoverBuildReportAction(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, @"xsl\NCover.xsl");
		}
	}
}
