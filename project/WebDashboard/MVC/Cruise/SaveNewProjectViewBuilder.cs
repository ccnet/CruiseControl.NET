using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class SaveNewProjectViewBuilder : HtmlBuilderViewBuilder
	{
		private readonly AddProjectViewBuilder projectViewBuilder;

		public SaveNewProjectViewBuilder(IHtmlBuilder htmlBuilder, AddProjectViewBuilder projectViewBuilder) : base(htmlBuilder)
		{
			this.projectViewBuilder = projectViewBuilder;
		}

		public Control BuildView(AddProjectModel model)
		{
			return Table(
				TR(TD(model.Status)),
				TR(TD(projectViewBuilder.BuildView(model))));
		}
	}
}
