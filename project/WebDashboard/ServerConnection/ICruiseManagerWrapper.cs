namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface ICruiseManagerWrapper
	{
		string GetLatestLogName (string serverName, string projectName);
		string GetLog(string serverName, string projectName, string logName);
	}
}
