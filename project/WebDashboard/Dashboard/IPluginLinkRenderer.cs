using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IPluginLinkRenderer
	{
		string Description { get; }
		string ActionName { get; }
	}
}
