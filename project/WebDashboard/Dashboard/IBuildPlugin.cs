using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildPlugin : IPlugin
	{
		// ToDo - sort out Build Specifiers. See also IBuildUrlGenerator
		string CreateURL(string serverName, string projectName, string buildName, IBuildUrlGenerator urlGenerator);
	}
}
