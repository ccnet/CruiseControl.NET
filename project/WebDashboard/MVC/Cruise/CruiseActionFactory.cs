using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;

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

		public static readonly string ACTION_PARAMETER_PREFIX = "_action_";
		public static readonly char ACTION_ARG_SEPARATOR = '_';

		public IAction Create(IRequest request)
		{
			string actionParam = request.FindParameterStartingWith(ACTION_PARAMETER_PREFIX);
			string actionName = getActionName(actionParam);

			if (actionName == "")
			{
				// euwww - something better here!
				return giver.GiveObjectById(FarmReportFarmPlugin.ACTION_NAME) as IAction;
			}

			IAction action = giver.GiveObjectById(actionName) as IAction;

			if (action == null)
			{
				return new UnknownActionAction();
			}
			return action;
		}

		public string[] ActionArguments(IRequest request)
		{
			string actionParam = request.FindParameterStartingWith(ACTION_PARAMETER_PREFIX);
			string actionName = getActionName(actionParam);
			if (actionParam == "" || actionParam.Length == (actionName.Length + ACTION_PARAMETER_PREFIX.Length))
			{
				return new string[0];
			}
			else
			{
				return actionParam.Substring(actionName.Length + ACTION_PARAMETER_PREFIX.Length + 1).Split(ACTION_ARG_SEPARATOR);
			}
		}

		private string getActionName(string actionParam)
		{
			if (actionParam == "")
			{
				return "";
			}
			else
			{
				string actionNameAndArgs = actionParam.Substring(ACTION_PARAMETER_PREFIX.Length);
				if (actionNameAndArgs.IndexOf(ACTION_ARG_SEPARATOR) < 0)
				{
					return actionNameAndArgs;
				}
				else
				{
					return actionNameAndArgs.Substring(0, actionNameAndArgs.IndexOf(ACTION_ARG_SEPARATOR));
				}
			}
		}
	}
}
