using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	public class DisplayAddProjectPageAction : ICruiseAction
	{
		private readonly AddProjectViewBuilder viewBuilder;
		private readonly AddProjectModelGenerator projectModelGenerator;

		public DisplayAddProjectPageAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder)
		{
			this.projectModelGenerator = projectModelGenerator;
			this.viewBuilder = viewBuilder;
		}

		public IView Execute(ICruiseRequest request)
		{
			AddEditProjectModel model = projectModelGenerator.GenerateModel(request.Request);
			model.SaveActionName = SaveNewProjectAction.ACTION_NAME;
			model.IsAdd = true;
			model.Status = "";
			return viewBuilder.BuildView(model);
		}
	}
}
