using System;
using System.Collections;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.config;
using ThoughtWorks.CruiseControl.WebDashboard.Config;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class ServerAggregatingCruiseManagerWrapper : ICruiseManagerWrapper
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

		private ICruiseManager GetCruiseManager(string serverName)
		{
			foreach (ServerSpecification server in (IEnumerable) configurationGetter.GetConfigFromSection(ServersSectionHandler.SectionName))
			{
				if (server.Name == serverName)
				{
					return managerFactory.GetCruiseManager(server.Url);
				}
			}

			throw new UnknownServerException(serverName);

		}
	}
}
