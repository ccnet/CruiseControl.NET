using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CruiseManagerBuildNameRetriever : IBuildNameRetriever
	{
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public CruiseManagerBuildNameRetriever(ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cruiseManagerWrapper = cruiseManagerWrapper;
		}

		public string GetLatestBuildName(string serverName, string projectName)
		{
			return cruiseManagerWrapper.GetLatestBuildName(serverName, projectName);	
		}

		public string GetNextBuildName(string serverName, string projectName, string buildName)
		{
			string[] buildNames = cruiseManagerWrapper.GetBuildNames(serverName, projectName);

			if (buildName == buildNames[0])
			{
				return buildName;
			}

			for (int i = 1; i < buildNames.Length; i++)
			{
				if (buildName == buildNames[i])
				{
					return buildNames[i-1];
				}
			}
			throw new UnknownBuildException(new Build(buildName, "", serverName, projectName));
		}

		public string GetPreviousBuildName(string serverName, string projectName, string buildName)
		{
			string[] buildNames = cruiseManagerWrapper.GetBuildNames(serverName, projectName);

			if (buildName == buildNames[buildNames.Length - 1])
			{
				return buildName;
			}

			for (int i = 0; i < buildNames.Length - 1; i++)
			{
				if (buildName == buildNames[i])
				{
					return buildNames[i+1];
				}
			}

			throw new UnknownBuildException(new Build(buildName, "", serverName, projectName));
		}
	}
}
