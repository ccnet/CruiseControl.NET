using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IPlugin
	{
		TypedAction[] Actions { get; }
	}
}
