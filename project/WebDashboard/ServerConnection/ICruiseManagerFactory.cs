using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface ICruiseManagerFactory
	{
		ICruiseManager GetCruiseManager(string url);
	}
}
