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
			if (model.AllowDelete)
			{
				return NotYetDeletedView(model);
			}
			else
			{
				return HasBeenDeletedView(model);
			}
		}

		private Control NotYetDeletedView(DeleteProjectModel model)
		{
			return Table(
				TR(TD(model.Message, 2)),
				TR(TD("&nbsp;", 2)),
				TR(TD("Purge Working Directory?"), TD(BooleanCheckBox("PurgeWorkingDirectory", model.PurgeWorkingDirectory))),
				TR(TD("Purge Artifact Directory?"), TD(BooleanCheckBox("PurgeArtifactDirectory", model.PurgeArtifactDirectory))),
				TR(TD("Purge Source Control Environment?"), TD(BooleanCheckBox("PurgeSourceControlEnvironment", model.PurgeSourceControlEnvironment))),
				TR(TD(Button(CruiseActionFactory.DO_DELETE_PROJECT_ACTION_NAME, "Yes - Really Delete"), 2))
				);
		}

		private Control HasBeenDeletedView(DeleteProjectModel model)
		{
			return Table(
				TR(TD(model.Message)),
				TR(TD("&nbsp;")),
				TR(TD(A("Return to Dashboard", urlBuilder.BuildUrl("default.aspx"))))
				);
		}
	}
}
