using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds
{
	public class ViewAllBuildsAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ViewAllBuilds";

		private readonly IAllBuildsViewBuilder viewBuilder;

		public ViewAllBuildsAction (IAllBuildsViewBuilder viewBuilder)
		{
			this.viewBuilder = viewBuilder;
		}

		public Control Execute(ICruiseRequest cruiseRequest)
		{
			return viewBuilder.BuildAllBuildsTable(cruiseRequest.ProjectSpecifier);
		}
	}
}
