using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewBuildReport
{
	public class ViewTestDetailsBuildReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewTestDetailsBuildReport";

		private readonly IMultiTransformer transformer;

		private readonly IBuildRetriever buildRetriever;

		public ViewTestDetailsBuildReportAction(IBuildRetriever buildRetriever, IMultiTransformer transformer)
		{
			this.buildRetriever = buildRetriever;
			this.transformer = transformer;
		}

		public Control Execute (ICruiseRequest cruiseRequest)
		{
			string log = buildRetriever.GetBuild(cruiseRequest.ServerName, cruiseRequest.ProjectName, cruiseRequest.BuildName).Log;
			HtmlGenericControl control = new HtmlGenericControl("div");
			control.InnerHtml = transformer.Transform(log, new string[] { @"xsl\tests.xsl" });
			return control;
		}
	}
}
