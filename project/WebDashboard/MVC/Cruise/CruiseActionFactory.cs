using ObjectWizard;
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
			string actionName = request.FileNameWithoutExtension;

			// Can probably do something clever with this in CruiseObjectGiverInitialiser
			if (actionName == string.Empty || actionName.ToLower() == "default")
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
