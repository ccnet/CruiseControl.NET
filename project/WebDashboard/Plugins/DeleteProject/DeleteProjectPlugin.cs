using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.DeleteProject
{
	// ToDo - Test!
	public class DeleteProjectPlugin : IPlugin
	{
		private readonly IActionInstantiator actionInstantiator;

		public DeleteProjectPlugin(IActionInstantiator actionInstantiator)
		{
			this.actionInstantiator = actionInstantiator;
		}

		public string LinkDescription
		{
			get { return "Delete Project"; }
		}

		public string LinkActionName
		{
			get { return NamedActions[0].ActionName; }
		}

		public INamedAction[] NamedActions
		{
			get
			{
				return new INamedAction[]
				{
					new ImmutableNamedAction("ShowDeleteProject", actionInstantiator.InstantiateAction(typeof(ShowDeleteProjectAction))),
					new ImmutableNamedAction(DoDeleteProjectAction.ACTION_NAME, actionInstantiator.InstantiateAction(typeof(ShowDeleteProjectAction)))
				};
			}
		}
	}
}
