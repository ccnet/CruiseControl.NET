using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard.Actions;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	// ToDo - test untested bits!
	public class CruiseActionFactory : IActionFactory
	{
		private readonly ObjectSource objectSource;

		public CruiseActionFactory (ObjectSource objectSource)
		{
			this.objectSource = objectSource;
		}

		public IAction Create(IRequest request)
		{
			string actionName = request.FileNameWithoutExtension;

			// Can probably do something clever with this in CruiseObjectSourceInitialiser
			if (actionName == string.Empty || actionName.ToLower() == "default")
			{
				return objectSource.GetByType(typeof(DefaultAction)) as IAction;
			}

			IAction action = objectSource.GetByName(actionName) as IAction;

			if (action == null)
			{
				return new UnknownActionAction();
			}
			return action;
		}
	}
}
