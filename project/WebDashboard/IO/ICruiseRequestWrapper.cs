using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface ICruiseRequestWrapper
	{
		IBuildSpecifier GetBuildSpecifier();
		string GetServerName();
		string GetProjectName();
		string GetBuildName();
	}
}
