namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface IFarmService
	{
		string[] GetMostRecentBuildNames(string serverName, string projectName, int buildCount);
		string[] GetBuildNames(string serverName, string projectName);
		void DeleteProject(string serverName, string projectName);
		string GetServerLog(string serverName);
	}
}
