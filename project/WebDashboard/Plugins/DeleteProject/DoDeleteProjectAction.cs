using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class DoDeleteProjectAction : ICruiseAction
	{
		private readonly IFarmService farmService;
		private readonly IDeleteProjectViewBuilder viewBuilder;

		public DoDeleteProjectAction(IDeleteProjectViewBuilder viewBuilder, IFarmService farmService)
		{
			this.viewBuilder = viewBuilder;
			this.farmService = farmService;
		}

		public Control Execute(ICruiseRequest request)
		{
			string serverName = request.ServerName;
			string projectName = request.ProjectName;
			farmService.DeleteProject(serverName, projectName);
			return viewBuilder.BuildView(BuildModel(serverName, projectName));
		}

		private DeleteProjectModel BuildModel(string serverName, string projectName)
		{
			return new DeleteProjectModel(serverName, projectName, string.Format("Project Deleted"), false);
		}
	}
}
