using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IServerUrlGenerator
	{
		string GenerateUrl(string urlBase, string serverName);
	}
}
