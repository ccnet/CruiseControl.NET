using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface IRequestWrapper
	{
		ILogSpecifier GetLogSpecifier();
		string GetServerName();
		string GetProjectName();
	}
}
