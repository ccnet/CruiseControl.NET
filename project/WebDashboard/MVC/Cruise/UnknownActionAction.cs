using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class UnknownActionAction : IAction
	{
		public IView Execute(IRequest request)
		{
			string actionName = request.FindParameterStartingWith(DefaultUrlBuilder.ACTION_PARAMETER_PREFIX);
			if (actionName == "")
			{
				return new StringView("Internal Error - 'UnknownActionAction' called but there is no action is request!");
			}
			else
			{
				actionName = actionName.Substring(DefaultUrlBuilder.ACTION_PARAMETER_PREFIX.Length);	
				return new StringView("Unknown action requested - " + actionName);
			}
		}
	}
}
