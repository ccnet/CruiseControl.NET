using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IServerPlugin : IPlugin
	{
		string CreateURL(string serverName, IServerUrlGenerator urlGenerator);
	}
}
