using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.AddProject
{
	public class AddProjectPlugin : IPluginLinkRenderer, IPlugin
	{
		public string LinkDescription
		{
			get { return "Add Project"; }
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
					new TypedAction("AddProjectDisplay", typeof(DisplayAddProjectPageAction)),
					new TypedAction(SaveNewProjectAction.ACTION_NAME, typeof(SaveNewProjectAction))
				};
			}
		}
	}
}
