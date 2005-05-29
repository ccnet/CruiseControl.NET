using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
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

		public IView Execute(ICruiseRequest request)
		{
			IProjectSpecifier projectSpecifier = request.ProjectSpecifier;
			bool purgeWorkingDirectory = request.Request.GetChecked("PurgeWorkingDirectory");
			bool purgeArtifactDirectory = request.Request.GetChecked("PurgeArtifactDirectory");
			bool purgeSourceControlEnvironment = request.Request.GetChecked("PurgeSourceControlEnvironment");
			farmService.DeleteProject(projectSpecifier, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
			return viewBuilder.BuildView(BuildModel(projectSpecifier, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment));
		}

		private DeleteProjectModel BuildModel(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			return new DeleteProjectModel(projectSpecifier, string.Format("Project Deleted"), false, 
				purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}
	}
}
