using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	//		Commented by Mike Roberts - this is in development - please contact me if you change it
//	public class StartAddProjectPageAction : ICruiseAction
//	{
//		private readonly AddProjectViewBuilder viewBuilder;
//		private readonly IUrlBuilder urlBuilder;
//		private readonly AddProjectModelGenerator projectModelGenerator;
//
//		public StartAddProjectPageAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder, IUrlBuilder urlBuilder)
//		{
//			this.projectModelGenerator = projectModelGenerator;
//			this.viewBuilder = viewBuilder;
//			this.urlBuilder = urlBuilder;
//		}
//
//		public IView Execute(ICruiseRequest request)
//		{
//			AddEditProjectModel model = projectModelGenerator.GenerateNewProjectModel();
//			model.SaveActionName = SaveNewProjectAction.ACTION_NAME;
//			model.UpdateSourceControlUrl = urlBuilder.BuildServerUrl(new ActionSpecifierWithName(ContinueAddProjectPageAction.ACTION_NAME), "updateSourceControl");
//			model.UpdateBuilderUrl = urlBuilder.BuildFormName(new ActionSpecifierWithName(ContinueAddProjectPageAction.ACTION_NAME), "updateBuilder");
//			model.IsAdd = true;
//			model.Status = "";
//			return viewBuilder.BuildView(model);
//		}
//	}
}
