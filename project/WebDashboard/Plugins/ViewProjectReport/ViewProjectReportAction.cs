using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewProjectReport
{
	public class ViewProjectReportAction : ICruiseAction
	{
		public ViewProjectReportAction()
		{
		}

		public Control Execute(ICruiseRequest cruiseRequest)
		{
			HtmlGenericControl control = new HtmlGenericControl("p");
			control.InnerHtml = "Project report in development. For now, click one of the builds in the side bar to see a build report";
			return control;
		}
	}
}
