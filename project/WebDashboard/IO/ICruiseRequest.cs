using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public interface ICruiseRequest
	{
		IBuildSpecifier GetBuildSpecifier();
		string ServerName { get; }
		string ProjectName { get; }
		string BuildName { get; }
	}
}
