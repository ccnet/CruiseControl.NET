using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectUrlGenerator
	{
		string GenerateUrl(string urlBase, string serverName, string projectName);
	}
}
