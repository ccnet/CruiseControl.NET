using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	public class DisplayAddProjectPageAction : IAction
	{
		public static readonly string ACTION_NAME = "AddProjectDisplay";

		private readonly AddProjectViewBuilder viewBuilder;
		private readonly AddProjectModelGenerator projectModelGenerator;

		public DisplayAddProjectPageAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder)
		{
			this.projectModelGenerator = projectModelGenerator;
			this.viewBuilder = viewBuilder;
		}

		public Control Execute(IRequest request)
		{
			AddEditProjectModel model = projectModelGenerator.GenerateModel(request);
			model.SaveActionName = SaveNewProjectAction.ACTION_NAME;
			model.IsAdd = true;
			model.Status = "";
			return viewBuilder.BuildView(model);
		}
	}
}
