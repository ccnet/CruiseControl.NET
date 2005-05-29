using System.Collections;
using Commons.Collections;
using ObjectWizard;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	// ToDo - test untested bits!
	public class CruiseActionFactory : IActionFactory
	{
		private readonly ObjectGiver giver;

		public CruiseActionFactory (ObjectGiver giver)
		{
			this.giver = giver;
		}

		public IAction Create(IRequest request)
		{
			string actionParam = request.FindParameterStartingWith(DefaultUrlBuilder.ACTION_PARAMETER_PREFIX);
			string actionName = actionParam == "" ? "" : actionParam.Substring(DefaultUrlBuilder.ACTION_PARAMETER_PREFIX.Length);;

			if (actionName == "")
			{
				return giver.GiveObjectByType(typeof(DefaultAction)) as IAction;
			}

			IAction action = giver.GiveObjectById(actionName) as IAction;

			if (action == null)
			{
				return new UnknownActionAction();
			}
			return action;
		}
	}
}
