using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class DoDeleteProjectAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "DoDeleteProject";

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
			bool purgeWorkingDirectory = request.Request.GetChecked("PurgeWorkingDirectory");
			bool purgeArtifactDirectory = request.Request.GetChecked("PurgeArtifactDirectory");
			bool purgeSourceControlEnvironment = request.Request.GetChecked("PurgeSourceControlEnvironment");
			farmService.DeleteProject(serverName, projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
			return viewBuilder.BuildView(BuildModel(serverName, projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment));
		}

		private DeleteProjectModel BuildModel(string serverName, string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			return new DeleteProjectModel(serverName, projectName, string.Format("Project Deleted"), false, 
				purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}
	}
}
