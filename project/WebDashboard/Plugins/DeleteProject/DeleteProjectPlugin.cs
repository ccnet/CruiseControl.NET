using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	public class DeleteProjectPlugin : IPluginLinkRenderer, IPlugin
	{
		public string LinkDescription
		{
			get { return "Delete Project"; }
		}

		public string LinkActionName
		{
			get { return Actions[0].ActionName; }
		}

		public TypedAction[] Actions
		{
			get
			{
				return new TypedAction[]
				{
					new TypedAction("ShowDeleteProject", typeof(ShowDeleteProjectAction)),
					new TypedAction(DoDeleteProjectAction.ACTION_NAME, typeof(DoDeleteProjectAction))
				};
			}
		}
	}
}
