namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface IFarmService
	{
		string[] GetMostRecentBuildNames(string serverName, string projectName, int buildCount);
	}
}
