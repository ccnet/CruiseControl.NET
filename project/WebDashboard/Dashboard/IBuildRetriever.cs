namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildRetriever
	{
		Build GetBuild(string serverName, string projectName, string buildName);
	}
}
