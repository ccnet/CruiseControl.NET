using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface IRequestWrapper
	{
		ILogSpecifier GetBuildSpecifier();
		string GetServerName();
		string GetProjectName();
	}
}
