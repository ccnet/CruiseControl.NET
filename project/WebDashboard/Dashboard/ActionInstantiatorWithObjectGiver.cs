using System;
using ObjectWizard;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ActionInstantiatorWithObjectGiver : IActionInstantiator
	{
		private readonly ObjectGiver objectGiver;

		public ActionInstantiatorWithObjectGiver(ObjectGiver objectGiver)
		{
			this.objectGiver = objectGiver;
		}

		public ICruiseAction InstantiateAction(Type actionType)
		{
			return (ICruiseAction) objectGiver.GiveObjectByType(actionType);
		}
	}
}
