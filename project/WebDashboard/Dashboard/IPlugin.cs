using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IPlugin
	{
		string Description { get; }
		string Url { get; }
	}
}
