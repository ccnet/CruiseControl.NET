using System.Collections;
using System.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport
{
	public class ViewBuildReportAction : ICruiseAction
	{
		private readonly IMultiTransformer transformer;
		public static readonly string ACTION_NAME = "ViewBuildReport";

		private readonly IBuildRetriever buildRetriever;

		public ViewBuildReportAction(IBuildRetriever buildRetriever, IMultiTransformer transformer)
		{
			this.buildRetriever = buildRetriever;
			this.transformer = transformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			// ToDo - train wreck removal
			string log = buildRetriever.GetBuild(cruiseRequest.ServerName, cruiseRequest.ProjectName, cruiseRequest.BuildName).Log;
			HtmlGenericControl control = new HtmlGenericControl("div");
			// ToDo - config stuff still nasty. We can do better. :)
			control.InnerHtml = transformer.Transform(log, (string[]) ((ArrayList) ConfigurationSettings.GetConfig("CCNet/xslFiles")).ToArray(typeof (string)));
			return control;
		}
	}
}
