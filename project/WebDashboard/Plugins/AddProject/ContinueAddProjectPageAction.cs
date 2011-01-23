using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
//	public class ContinueAddProjectPageAction : ICruiseAction
//	{
//		Commented by Mike Roberts - this is in development - please contact me if you change it
//		public static readonly string ACTION_NAME = "ContinueAddProject";
//
//		private readonly AddProjectViewBuilder viewBuilder;
//		private readonly AddProjectModelGenerator projectModelGenerator;
//
//		public ContinueAddProjectPageAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder)
//		{
//			this.projectModelGenerator = projectModelGenerator;
//			this.viewBuilder = viewBuilder;
//		}
//
//		public IView Execute(ICruiseRequest request)
//		{
//			AddEditProjectModel model = projectModelGenerator.GenerateNewProjectModel(request);
//			model.SaveActionName = SaveNewProjectAction.ACTION_NAME;
//			model.IsAdd = true;
//			model.Status = "";
//			return viewBuilder.BuildView(model);
//		}
//	}
}
