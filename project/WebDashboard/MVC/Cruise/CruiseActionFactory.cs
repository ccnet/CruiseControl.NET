using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class CruiseActionFactory : IActionFactory
	{
		private readonly ObjectGiver giver;

		public CruiseActionFactory (ObjectGiver giver)
		{
			this.giver = giver;
		}

		public static readonly string ACTION_PARAMETER_PREFIX = "_action_";

		public IAction Create(IRequest request)
		{
			return new ExceptionCatchingActionProxy(CreateSpecificAction(request));
		}

		private IAction CreateSpecificAction(IRequest request)
		{
			string actionName = request.FindParameterStartingWith(ACTION_PARAMETER_PREFIX);
			if (actionName == "")
			{
				return new DefaultCruiseAction();
			}
			IAction action = giver.GiveObjectById(actionName.Substring(ACTION_PARAMETER_PREFIX.Length)) as IAction;
			if (action == null)
			{
				return new UnknownActionAction();
			}
			return action;
		}
	}
}
