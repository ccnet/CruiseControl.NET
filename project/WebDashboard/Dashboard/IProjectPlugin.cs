using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectPlugin : IPlugin
	{
		string CreateURL(string serverName, string projectName, IProjectUrlGenerator urlGenerator);
	}
}
