using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewProjectReport
{
	public class ViewProjectReportAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewProjectReport";

		public ViewProjectReportAction()
		{
		}

		public IView Execute(ICruiseRequest cruiseRequest)
		{
			return new DefaultView("Project report in development. For now, click one of the builds in the side bar to see a build report");
		}
	}
}
