using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ImmutableNamedAction : INamedAction
	{
		private readonly string actionName;
		private readonly ICruiseAction action;

		public ImmutableNamedAction(string actionName, ICruiseAction action)
		{
			this.actionName = actionName;
			this.action = action;
		}

		public string ActionName
		{
			get { return actionName; }
		}

		public ICruiseAction Action
		{
			get { return action; }
		}
	}
}
