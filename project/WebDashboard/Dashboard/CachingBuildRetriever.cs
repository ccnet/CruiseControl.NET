using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CachingBuildRetriever : IBuildRetriever
	{
		public static readonly string CacheDirectory = "originalBuilds";

		private readonly ICacheManager cacheManager;
		private readonly ICruiseManagerWrapper cruiseManagerWrapper;

		public CachingBuildRetriever(ICacheManager cacheManager, ICruiseManagerWrapper cruiseManagerWrapper)
		{
			this.cacheManager = cacheManager;
			this.cruiseManagerWrapper = cruiseManagerWrapper;
		}

		public Build GetBuild(string serverName, string projectName, string buildName)
		{
			PutLogInCacheIfNecessary(serverName, projectName, buildName);

			string buildLog = cacheManager.GetContent(serverName, projectName, CacheDirectory, buildName);
			string buildLogLocation = cacheManager.GetURLForFile(serverName, projectName, CacheDirectory, buildName);

			return new Build(buildName, buildLog, serverName, projectName, buildLogLocation);
		}

		private void PutLogInCacheIfNecessary(string serverName, string projectName, string buildName)
		{
			if (cacheManager.GetContent(serverName, projectName, CacheDirectory, buildName) == null)
			{
				string log = cruiseManagerWrapper.GetLog(serverName, projectName, buildName);
				cacheManager.AddContent(serverName, projectName, CacheDirectory, buildName, log);
			}
		}
	}
}
