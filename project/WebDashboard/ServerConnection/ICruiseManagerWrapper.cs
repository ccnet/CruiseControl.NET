using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface ICruiseManagerWrapper
	{
		string GetLatestBuildName(string serverName, string projectName);
		string GetLog(string serverName, string projectName, string buildName);
		string[] GetBuildNames(string serverName, string projectName);
		string GetServerLog(string serverName);
		string[] GetServerNames();
		void AddProject(string serverName, string serializedProject);
	}
}
