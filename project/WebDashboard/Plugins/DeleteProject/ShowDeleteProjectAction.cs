using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class ShowDeleteProjectAction : ICruiseAction
	{
		private readonly IDeleteProjectViewBuilder viewBuilder;

		public ShowDeleteProjectAction(IDeleteProjectViewBuilder viewBuilder)
		{
			this.viewBuilder = viewBuilder;
		}

		public Control Execute(ICruiseRequest request)
		{
			return viewBuilder.BuildView(BuildModel(request.ServerName, request.ProjectName));
		}

		private DeleteProjectModel BuildModel(string serverName, string projectName)
		{
			return new DeleteProjectModel(serverName, projectName, string.Format("Please confirm you want to delete {0}, and choose which extra delete actions you want to perform", projectName), true, true, true, true);
		}
	}
}
