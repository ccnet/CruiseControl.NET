namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface ICruiseManagerWrapper
	{
		string GetLog(string serverName, string projectName, ILogSpecifier logSpecifier);
	}
}
