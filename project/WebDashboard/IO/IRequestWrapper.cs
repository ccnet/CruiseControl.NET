using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface IRequestWrapper
	{
		IBuildSpecifier GetBuildSpecifier();
		string GetServerName();
		string GetProjectName();
	}
}
