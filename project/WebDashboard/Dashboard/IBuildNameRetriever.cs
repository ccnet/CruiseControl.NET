namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildNameRetriever
	{
		string GetLatestBuildName(string serverName, string projectName);
		string GetNextBuildName(string serverName, string projectName, string buildName);
		string GetPreviousBuildName(string serverName, string projectName, string buildName);
	}
}
