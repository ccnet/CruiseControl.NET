using System;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildNameRetriever
	{
		string GetLatestBuildName(string serverName, string projectName);
		string GetNextBuildName(Build build);
		string GetPreviousBuildName(Build build);
	}
}
