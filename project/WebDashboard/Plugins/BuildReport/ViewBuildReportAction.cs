using System.Collections;
using System.Configuration;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
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

		// ToDo - this shouldn't access Configuration Settings directly ... but maybe we want a new plugin impl anyway using configurable plugins
		public IView Execute (ICruiseRequest cruiseRequest)
		{
			return new DefaultView(buildLogTransformer.Transform(cruiseRequest.BuildSpecifier, (string[]) ((ArrayList) ConfigurationSettings.GetConfig("CCNet/xslFiles")).ToArray(typeof (string))));
		}
	}
}
