using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class DisplayAddProjectPageViewBuilder : HtmlBuilderViewBuilder
	{
		private readonly AddProjectViewBuilder projectViewBuilder;

		public DisplayAddProjectPageViewBuilder(IHtmlBuilder htmlBuilder, AddProjectViewBuilder projectViewBuilder) : base(htmlBuilder)
		{
			this.projectViewBuilder = projectViewBuilder;
		}

		public Control BuildView(AddProjectModel model)
		{
			return Table(
				TR(TD(projectViewBuilder.BuildView(model))),
				TR(TD(Button("AddProjectSave", "Save")), TD()));
		}
	}
}
