using System.Web.UI;
using System.Web.UI.HtmlControls;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
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
