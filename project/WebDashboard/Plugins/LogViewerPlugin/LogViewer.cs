using System;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewer
	{
		public static readonly string CacheDirectory = "originalLogs";

		private readonly ICacheManager cacheManager;
		private readonly ICruiseManagerWrapper manager;
		private readonly IRequestWrapper requestWrapper;

		public LogViewer(IRequestWrapper requestWrapper, ICruiseManagerWrapper manager, ICacheManager cacheManager)
		{
			this.requestWrapper = requestWrapper;
			this.manager = manager;
			this.cacheManager = cacheManager;
		}

		public LogViewerResults Do()
		{
			string serverName = requestWrapper.GetServerName();
			string projectName = requestWrapper.GetProjectName();
			string logName = GetLogName(serverName, projectName);

			PutLogInCacheIfNecessary(serverName, projectName, logName);

			return new LogViewerResults(cacheManager.GetURLForFile(serverName, projectName, CacheDirectory, logName));
		}

		private string GetLogName(string serverName, string projectName)
		{
			ILogSpecifier logSpecifier = requestWrapper.GetLogSpecifier();

			if (logSpecifier is NoLogSpecified)
			{
				return manager.GetLatestLogName(serverName, projectName);
			}
			else
			{
				return ((FileNameLogSpecifier) logSpecifier).Filename;
			}
		}

		private void PutLogInCacheIfNecessary(string serverName, string projectName, string logName)
		{
			if (cacheManager.GetContent(serverName, projectName, CacheDirectory, logName) == null)
			{
				string log = manager.GetLog(serverName, projectName, logName);
				cacheManager.AddContent(serverName, projectName, CacheDirectory, logName, log);
			}
		}
	}
}
