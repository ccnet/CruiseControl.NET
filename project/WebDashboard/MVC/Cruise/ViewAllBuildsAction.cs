using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ViewAllBuildsAction : IAction
	{
		private readonly ICruiseRequestFactory cruiseRequestFactory;
		private readonly IAllBuildsViewBuilder viewBuilder;

		public ViewAllBuildsAction (IAllBuildsViewBuilder viewBuilder, ICruiseRequestFactory cruiseRequestFactory)
		{
			this.viewBuilder = viewBuilder;
			this.cruiseRequestFactory = cruiseRequestFactory;
		}

		public Control Execute(IRequest request)
		{
			ICruiseRequest cruiseRequest = cruiseRequestFactory.CreateCruiseRequest(request);
			return viewBuilder.BuildAllBuildsTable(cruiseRequest.ServerName, cruiseRequest.ProjectName);
		}
	}
}
