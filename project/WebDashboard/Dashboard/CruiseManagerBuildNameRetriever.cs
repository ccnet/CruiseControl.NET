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

		public string GetNextBuildName(Build build)
		{
			string[] buildNames = cruiseManagerWrapper.GetBuildNames(build.ServerName, build.ProjectName);

			if (build.Name == buildNames[0])
			{
				return build.Name;
			}

			for (int i = 1; i < buildNames.Length; i++)
			{
				if (build.Name == buildNames[i])
				{
					return buildNames[i-1];
				}
			}
			throw new UnknownBuildException(build);
		}

		public string GetPreviousBuildName(Build build)
		{
			string[] buildNames = cruiseManagerWrapper.GetBuildNames(build.ServerName, build.ProjectName);

			if (build.Name == buildNames[buildNames.Length - 1])
			{
				return build.Name;
			}

			for (int i = 0; i < buildNames.Length - 1; i++)
			{
				if (build.Name == buildNames[i])
				{
					return buildNames[i+1];
				}
			}

			throw new UnknownBuildException(build);
		}
	}
}
