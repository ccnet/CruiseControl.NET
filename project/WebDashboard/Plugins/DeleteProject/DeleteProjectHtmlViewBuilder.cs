using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class DeleteProjectHtmlViewBuilder : HtmlBuilderViewBuilder, IDeleteProjectViewBuilder
	{
		private readonly IUrlBuilder urlBuilder;

		public DeleteProjectHtmlViewBuilder(IHtmlBuilder htmlBuilder, IUrlBuilder urlBuilder) : base(htmlBuilder)
		{
			this.urlBuilder = urlBuilder;
		}

		public Control BuildView(DeleteProjectModel model)
		{
			return Table(
				TR(TD(model.Message)),
				TR(TD("&nbsp;")),
				TR(TD(model.AllowDelete ? DoDeleteButton : LinkToDashboard))
				);
		}

		private Control DoDeleteButton
		{
			get
			{
				return Button(CruiseActionFactory.DO_DELETE_PROJECT_ACTION_NAME, "Yes - Really Delete");
			}
		}

		private Control LinkToDashboard
		{
			get
			{
				return A("Return to Dashboard", urlBuilder.BuildUrl("/"));
			}
		}
	}
}
