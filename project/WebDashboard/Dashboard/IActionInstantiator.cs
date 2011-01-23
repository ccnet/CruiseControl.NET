using System;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IActionInstantiator
	{
		ICruiseAction InstantiateAction(Type actionType);
	}
}