using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.EditProject
{
	public class EditProjectPlugin : IPluginLinkRenderer, IPlugin
	{
		public string LinkDescription
		{
			get { return "Edit Project"; }
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
					new TypedAction("EditProjectDisplay", typeof(DisplayEditProjectPageAction)),
					new TypedAction(SaveEditProjectAction.ACTION_NAME, typeof(SaveEditProjectAction))
				};
			}
		}
	}
}
