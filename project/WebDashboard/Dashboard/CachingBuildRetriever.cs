using System;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class CachingBuildRetriever : IBuildRetriever
	{
		private readonly IRequestWrapper requestWrapper;
		public static readonly string CacheDirectory = "originalBuilds";

		private readonly ICruiseManagerWrapper cruiseManagerWrapper;
		private readonly ICacheManager cacheManager;

		public CachingBuildRetriever(ICruiseManagerWrapper cruiseManagerWrapper, ICacheManager cacheManager, IRequestWrapper requestWrapper)
		{
			this.cacheManager = cacheManager;
			this.cruiseManagerWrapper = cruiseManagerWrapper;
			this.requestWrapper = requestWrapper;
		}

		public Build GetBuild()
		{
			string serverName = requestWrapper.GetServerName();
			string projectName = requestWrapper.GetProjectName();

			ILogSpecifier buildSpecifier = requestWrapper.GetBuildSpecifier();

			if (buildSpecifier is NoLogSpecified)
			{
				return GetBuild(serverName, projectName, cruiseManagerWrapper.GetLatestBuildName(serverName, projectName));
			}
			else
			{
				return GetBuild(serverName, projectName, ((FileNameLogSpecifier) buildSpecifier).Filename);
			}
		}

		public Build GetPreviousBuild(Build build)
		{
			// ToDo - implement properly
			return build;
		}

		public Build GetNextBuild(Build build)
		{
			// ToDo - implement properly
			return build;
		}

		private Build GetBuild(string serverName, string projectName, string buildName)
		{
			PutLogInCacheIfNecessary(serverName, projectName, buildName);

			return new Build(buildName, 
				cacheManager.GetContent(serverName, projectName, CacheDirectory, buildName), 
				cacheManager.GetURLForFile(serverName, projectName, CacheDirectory, buildName));
		}

		private void PutLogInCacheIfNecessary(string serverName, string projectName, string logName)
		{
			if (cacheManager.GetContent(serverName, projectName, CacheDirectory, logName) == null)
			{
				string log = cruiseManagerWrapper.GetLog(serverName, projectName, logName);
				cacheManager.AddContent(serverName, projectName, CacheDirectory, logName, log);
			}
		}
	}
}
