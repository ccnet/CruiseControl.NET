using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildUrlGenerator
	{
		// Todo - sort out our build specifiers and use it here
		string GenerateUrl(string urlBase, string serverName, string projectName, string buildName);
	}
}
