using System;
using Objection;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class ActionInstantiatorWithObjectSource : IActionInstantiator
	{
		private readonly ObjectSource objectSource;

		public ActionInstantiatorWithObjectSource(ObjectSource objectSource)
		{
			this.objectSource = objectSource;
		}

		public ICruiseAction InstantiateAction(Type actionType)
		{
			return (ICruiseAction) objectSource.GetByType(actionType);
		}
	}
}