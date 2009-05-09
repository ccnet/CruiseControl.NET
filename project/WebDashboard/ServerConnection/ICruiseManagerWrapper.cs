using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface ICruiseManagerWrapper
	{
        IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier, string sessionToken);
        string GetLog(IBuildSpecifier buildSpecifier, string sessionToken);
        IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier projectSpecifier, string sessionToken);
        string GetServerLog(IServerSpecifier serverSpecifier, string sessionToken);
		IServerSpecifier[] GetServerSpecifiers();
        void AddProject(IServerSpecifier serverSpecifier, string serializedProject, string sessionToken);
        string GetProject(IProjectSpecifier projectSpecifier, string sessionToken);
        void UpdateProject(IProjectSpecifier projectSpecifier, string serializedProject, string sessionToken);
        IServerSpecifier GetServerConfiguration(string serverName);
	}
}
