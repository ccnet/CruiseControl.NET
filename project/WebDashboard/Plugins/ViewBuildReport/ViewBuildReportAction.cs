using System.Collections;
using System.Configuration;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport
{
	public class ViewBuildReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewBuildReport";

		private readonly IRequestTransformer requestTransformer;

		public ViewBuildReportAction(IRequestTransformer requestTransformer)
		{
			this.requestTransformer = requestTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return requestTransformer.Transform(cruiseRequest, (string[]) ((ArrayList) ConfigurationSettings.GetConfig("CCNet/xslFiles")).ToArray(typeof (string)));
		}
	}
}
