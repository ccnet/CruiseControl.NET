namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class UnknownActionAction : IAction
	{
		public IView Execute(IRequest request)
		{
			string actionName = request.FindParameterStartingWith(CruiseActionFactory.ACTION_PARAMETER_PREFIX);
			if (actionName == "")
			{
				return new StringView("Internal Error - 'UnknownActionAction' called but there is no action is request!");
			}
			else
			{
				actionName = actionName.Substring(CruiseActionFactory.ACTION_PARAMETER_PREFIX.Length);	
				return new StringView("Unknown action requested - " + actionName);
			}
		}
	}
}
