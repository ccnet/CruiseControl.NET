using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface ICruiseManagerWrapper
	{
		IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier);
		string GetLog(IBuildSpecifier buildSpecifier);
		IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier projectSpecifier);
		string GetServerLog(IServerSpecifier serverSpecifier);
		IServerSpecifier[] GetServerSpecifiers();
		void AddProject(IServerSpecifier serverSpecifier, string serializedProject);
		string GetProject(IProjectSpecifier projectSpecifier);
		void UpdateProject(IProjectSpecifier projectSpecifier, string serializedProject);
	}
}
