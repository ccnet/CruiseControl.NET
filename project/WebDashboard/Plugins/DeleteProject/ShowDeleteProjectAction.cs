using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class ShowDeleteProjectAction : ICruiseAction
	{
		public static readonly string ACTION_NAME = "ShowDeleteProject";

		private readonly IDeleteProjectViewBuilder viewBuilder;

		public ShowDeleteProjectAction(IDeleteProjectViewBuilder viewBuilder)
		{
			this.viewBuilder = viewBuilder;
		}

		public IView Execute(ICruiseRequest request)
		{
			return viewBuilder.BuildView(BuildModel(request.ProjectSpecifier));
		}

		private DeleteProjectModel BuildModel(IProjectSpecifier projectSpecifier)
		{
			return new DeleteProjectModel(projectSpecifier, string.Format("Please confirm you want to delete {0}, and choose which extra delete actions you want to perform", projectSpecifier.ProjectName), true, true, true, true);
		}
	}
}
