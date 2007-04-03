namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationRepository
	{
		string GetBuildLog(string buildName);
		string[] GetBuildNames();
		string[] GetMostRecentBuildNames(int buildCount);
		string GetLatestBuildName();
	}
}