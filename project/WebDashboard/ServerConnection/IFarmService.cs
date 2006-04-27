using System.Xml;
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
		void Start(IProjectSpecifier projectSpecifier);
		void Stop(IProjectSpecifier projectSpecifier);
		void ForceBuild(IProjectSpecifier projectSpecifier);
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions();
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier);
		ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier);
		string GetServerVersion(IServerSpecifier serverSpecifier);
		string GetArtifactDirectory(IProjectSpecifier projectSpecifier);
		string GetStatisticsDocument(IProjectSpecifier projectSpecifier);
	}
}
