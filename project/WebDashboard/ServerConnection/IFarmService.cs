using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface IFarmService
	{
		IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount);
		IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier serverSpecifier);
		void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);
		string GetServerLog(IServerSpecifier serverSpecifier);
		string GetServerLog(IProjectSpecifier specifier);
		void Start(IProjectSpecifier projectSpecifier);
		void Stop(IProjectSpecifier projectSpecifier);
		void ForceBuild(IProjectSpecifier projectSpecifier, string enforcerName);
		void AbortBuild(IProjectSpecifier projectSpecifier, string enforcerName);
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions();
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier);
		ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier);
		IServerSpecifier[] GetServerSpecifiers();
		IServerSpecifier GetServerConfiguration(string serverName);
		string GetServerVersion(IServerSpecifier serverSpecifier);
		string GetArtifactDirectory(IProjectSpecifier projectSpecifier);
		string GetStatisticsDocument(IProjectSpecifier projectSpecifier);
        CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions();
        CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier serverSpecifier);
        string GetModificationHistoryDocument(IProjectSpecifier projectSpecifier);
        string GetRSSFeed(IProjectSpecifier projectSpecifier);

        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        long GetFreeDiskSpace(IServerSpecifier serverSpecifier);
    }
}
