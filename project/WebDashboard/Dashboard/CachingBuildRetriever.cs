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

		public Build GetBuild(IBuildSpecifier buildSpecifier)
		{
			PutLogInCacheIfNecessary(buildSpecifier);

			string buildLog = cacheManager.GetContent(buildSpecifier.ProjectSpecifier, CacheDirectory, buildSpecifier.BuildName);
			string buildLogLocation = cacheManager.GetURLForFile(buildSpecifier.ProjectSpecifier, CacheDirectory, buildSpecifier.BuildName);

			return new Build(buildSpecifier, buildLog, buildLogLocation);
		}

		private void PutLogInCacheIfNecessary(IBuildSpecifier buildSpecifier)
		{
			if (cacheManager.GetContent(buildSpecifier.ProjectSpecifier, CacheDirectory, buildSpecifier.BuildName) == null)
			{
				string log = cruiseManagerWrapper.GetLog(buildSpecifier);
				cacheManager.AddContent(buildSpecifier.ProjectSpecifier, CacheDirectory, buildSpecifier.BuildName, log);
			}
		}
	}
}
