using ThoughtWorks.CruiseControl.WebDashboard.Cache;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CachingBuildRetriever : IBuildRetriever
	{
		private readonly IBuildRetriever buildRetriever;
		public static readonly string CacheDirectory = "originalBuilds";

		private readonly ICacheManager cacheManager;

		public CachingBuildRetriever(ICacheManager cacheManager, IBuildRetriever buildRetriever)
		{
			this.cacheManager = cacheManager;
			this.buildRetriever = buildRetriever;
		}

		public Build GetBuild(string serverName, string projectName, string buildName)
		{
			PutLogInCacheIfNecessary(serverName, projectName, buildName);

			return new Build(buildName, cacheManager.GetContent(serverName, projectName, CacheDirectory, buildName), serverName, projectName);
		}

		private void PutLogInCacheIfNecessary(string serverName, string projectName, string buildName)
		{
			if (cacheManager.GetContent(serverName, projectName, CacheDirectory, buildName) == null)
			{
				string log = buildRetriever.GetBuild(serverName, projectName, buildName).Log;
				cacheManager.AddContent(serverName, projectName, CacheDirectory, buildName, log);
			}
		}
	}
}
