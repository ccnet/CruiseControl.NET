using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildPlugin
	{
		string ActionName { get; }
		Type ActionType { get; }
	}
}
