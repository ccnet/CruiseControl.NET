using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class ShowDeleteProjectAction : IAction
	{
		private readonly IDeleteProjectViewBuilder viewBuilder;
		private readonly ICruiseRequestFactory cruiseRequestFactory;

		public ShowDeleteProjectAction(ICruiseRequestFactory cruiseRequestFactory, IDeleteProjectViewBuilder viewBuilder)
		{
			this.cruiseRequestFactory = cruiseRequestFactory;
			this.viewBuilder = viewBuilder;
		}

		public Control Execute(IRequest request)
		{
			return viewBuilder.BuildView(BuildModel(cruiseRequestFactory.CreateCruiseRequest(request)));
		}

		private DeleteProjectModel BuildModel(ICruiseRequest cruiseRequest)
		{
			return BuildModel(cruiseRequest.ServerName, cruiseRequest.ProjectName);
		}

		private DeleteProjectModel BuildModel(string serverName, string projectName)
		{
			return new DeleteProjectModel(serverName, projectName, string.Format("Are you sure you want to delete {0} on {1}?", projectName, serverName), true);
		}
	}
}
