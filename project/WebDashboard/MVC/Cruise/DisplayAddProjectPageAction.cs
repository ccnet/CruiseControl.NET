using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class DisplayAddProjectPageAction : IAction
	{
		private readonly AddProjectViewBuilder viewBuilder;
		private readonly AddProjectModelGenerator projectModelGenerator;

		public DisplayAddProjectPageAction(AddProjectModelGenerator projectModelGenerator, AddProjectViewBuilder viewBuilder)
		{
			this.projectModelGenerator = projectModelGenerator;
			this.viewBuilder = viewBuilder;
		}

		public Control Execute(IRequest request)
		{
			return viewBuilder.BuildView(projectModelGenerator.GenerateModel(request));
		}
	}
}
