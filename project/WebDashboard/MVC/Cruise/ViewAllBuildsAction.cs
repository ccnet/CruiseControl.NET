using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ViewAllBuildsAction : ICruiseAction
	{
		private readonly IAllBuildsViewBuilder viewBuilder;

		public ViewAllBuildsAction (IAllBuildsViewBuilder viewBuilder)
		{
			this.viewBuilder = viewBuilder;
		}

		public Control Execute(ICruiseRequest cruiseRequest)
		{
			return viewBuilder.BuildAllBuildsTable(cruiseRequest.ServerName, cruiseRequest.ProjectName);
		}
	}
}
