using System.Collections;
using System.Configuration;
using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport
{
	public class ViewBuildReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewBuildReport";

		private readonly IBuildLogTransformer buildLogTransformer;

		public ViewBuildReportAction(IBuildLogTransformer buildLogTransformer)
		{
			this.buildLogTransformer = buildLogTransformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			return buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, (string[]) ((ArrayList) ConfigurationSettings.GetConfig("CCNet/xslFiles")).ToArray(typeof (string)));
		}
	}
}
