using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.config;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

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

		public string GetLatestBuildName(string serverName, string projectName)
		{
			return GetCruiseManager(serverName).GetLatestBuildName(projectName);
		}

		public string GetLog(string serverName, string projectName, string buildName)
		{
			return GetCruiseManager(serverName).GetLog(projectName, buildName);
		}

		public string[] GetBuildNames(string serverName, string projectName)
		{
			return GetCruiseManager(serverName).GetBuildNames(projectName);
		}

		public string[] GetMostRecentBuildNames(string serverName, string projectName, int buildCount)
		{
			return GetCruiseManager(serverName).GetMostRecentBuildNames(projectName,buildCount);
		}

		public void DeleteProject(string serverName, string projectName)
		{
			GetCruiseManager(serverName).DeleteProject(projectName);
		}

		public string GetServerLog (string serverName)
		{
			return GetCruiseManager(serverName).GetServerLog();
		}

		public string[] GetServerNames()
		{
			ArrayList names = new ArrayList();
			foreach (ServerSpecification server in ServerSpecifcations)
			{
				names.Add(server.Name);
			}
			return (string[]) names.ToArray(typeof (string));
		}

		public void AddProject(string serverName, string serializedProject)
		{
			GetCruiseManager(serverName).AddProject(serializedProject);
		}

		private ICruiseManager GetCruiseManager(string serverName)
		{
			foreach (ServerSpecification server in ServerSpecifcations)
			{
				if (server.Name == serverName)
				{
					return managerFactory.GetCruiseManager(server.Url);
				}
			}

			throw new UnknownServerException(serverName);
		}

		private IEnumerable ServerSpecifcations
		{
			get { return (IEnumerable) configurationGetter.GetConfigFromSection(ServersSectionHandler.SectionName); }
		}


	}
}
