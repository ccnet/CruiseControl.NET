using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Config;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class ServerAggregatingCruiseManagerWrapper : ICruiseManagerWrapper, IFarmService
	{
		private readonly ICruiseManagerFactory managerFactory;
		private readonly IConfigurationGetter configurationGetter;

		public ServerAggregatingCruiseManagerWrapper(IConfigurationGetter configurationGetter, ICruiseManagerFactory managerFactory)
		{
			this.configurationGetter = configurationGetter;
			this.managerFactory = managerFactory;
		}

		public IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier)
		{
			return new DefaultBuildSpecifier(projectSpecifier, GetCruiseManager(projectSpecifier.ServerSpecifier).GetLatestBuildName(projectSpecifier.ProjectName));
		}

		public string GetLog(IBuildSpecifier buildSpecifier)
		{
			return GetCruiseManager(buildSpecifier).GetLog(buildSpecifier.ProjectSpecifier.ProjectName, buildSpecifier.BuildName);
		}

		public IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier projectSpecifier)
		{
			return CreateBuildSpecifiers(projectSpecifier, GetCruiseManager(projectSpecifier).GetBuildNames(projectSpecifier.ProjectName));
		}

		public IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount)
		{
			return CreateBuildSpecifiers(projectSpecifier, GetCruiseManager(projectSpecifier).GetMostRecentBuildNames(projectSpecifier.ProjectName, buildCount));
		}

		private IBuildSpecifier[] CreateBuildSpecifiers(IProjectSpecifier projectSpecifier, string[] buildNames)
		{
			ArrayList buildSpecifiers = new ArrayList();
			foreach (string buildName in buildNames)
			{
				buildSpecifiers.Add(new DefaultBuildSpecifier(projectSpecifier, buildName));
			}
			return (IBuildSpecifier[]) buildSpecifiers.ToArray(typeof (IBuildSpecifier));
		}

		public void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			GetCruiseManager(projectSpecifier).DeleteProject(projectSpecifier.ProjectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

		public ProjectStatus[] GetProjectStatusList(IServerSpecifier serverSpecifier)
		{
			return GetCruiseManager(serverSpecifier).GetProjectStatus();
		}

		public void ForceBuild(IProjectSpecifier projectSpecifier)
		{
			GetCruiseManager(projectSpecifier.ServerSpecifier).ForceBuild(projectSpecifier.ProjectName);
		}

		public string GetServerLog(IServerSpecifier serverSpecifier)
		{
			return GetCruiseManager(serverSpecifier).GetServerLog();
		}

		public IServerSpecifier[] GetServerSpecifiers()
		{
			ArrayList serverSpecifiers = new ArrayList();
			foreach (ServerLocation serverLocation in ServerLocations)
			{
				serverSpecifiers.Add(new DefaultServerSpecifier(serverLocation.Name));
			}
			return (IServerSpecifier[]) serverSpecifiers.ToArray(typeof (IServerSpecifier));
		}

		public void AddProject(IServerSpecifier serverSpecifier, string serializedProject)
		{
			GetCruiseManager(serverSpecifier).AddProject(serializedProject);
		}

		public string GetProject(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier.ServerSpecifier).GetProject(projectSpecifier.ProjectName);
		}

		public void UpdateProject(IProjectSpecifier projectSpecifier, string serializedProject)
		{
			GetCruiseManager(projectSpecifier.ServerSpecifier).UpdateProject(projectSpecifier.ProjectName, serializedProject);
		}

		private ICruiseManager GetCruiseManager(IBuildSpecifier buildSpecifier)
		{
			return GetCruiseManager(buildSpecifier.ProjectSpecifier);
		}

		private ICruiseManager GetCruiseManager(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier.ServerSpecifier);
		}

		private ICruiseManager GetCruiseManager(IServerSpecifier serverSpecifier)
		{
			foreach (ServerLocation serverLocation in ServerLocations)
			{
				if (serverLocation.Name == serverSpecifier.ServerName)
				{
					return managerFactory.GetCruiseManager(serverLocation.Url);
				}
			}

			throw new UnknownServerException(serverSpecifier.ServerName);
		}

		private IEnumerable ServerLocations
		{
			get { return (IEnumerable) configurationGetter.GetConfigFromSection(ServersSectionHandler.SectionName); }
		}
	}
}
