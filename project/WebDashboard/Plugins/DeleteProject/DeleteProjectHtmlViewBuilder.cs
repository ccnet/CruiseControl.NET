using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	//		Commented by Mike Roberts - this is in development - please contact me if you change it
//	public class DeleteProjectHtmlViewBuilder : IDeleteProjectViewBuilder
//	{
//		public IView BuildView(DeleteProjectModel model)
//		{
//			if (model.AllowDelete)
//			{
//				return NotYetDeletedView(model);
//			}
//			else
//			{
//				return HasBeenDeletedView(model);
//			}
//		}
//
//		private IView NotYetDeletedView(DeleteProjectModel model)
//		{
//			return new ControlView(
//				Table(
//				TR(TD(model.Message, 2)),
//				TR(TD("&nbsp;", 2)),
//				TR(TD("Purge Working Directory?"), TD(BooleanCheckBox("PurgeWorkingDirectory", model.PurgeWorkingDirectory))),
//				TR(TD("Purge Artifact Directory?"), TD(BooleanCheckBox("PurgeArtifactDirectory", model.PurgeArtifactDirectory))),
//				TR(TD("Purge Source Control Environment?"), TD(BooleanCheckBox("PurgeSourceControlEnvironment", model.PurgeSourceControlEnvironment))),
//				TR(TD(Button(DoDeleteProjectAction.ACTION_NAME, "Yes - Really Delete"), 2))
//				));
//		}
//
//		private IView HasBeenDeletedView(DeleteProjectModel model)
//		{
//			return new ControlView(
//				Table(
//				TR(TD(model.Message)),
//				TR(TD("&nbsp;"))
//				));
//		}
//	}
}
