using ThoughtWorks.CruiseControl.Core.BuildLog;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewer
	{
		public static readonly string CacheDirectory = "originalLogs";

		private readonly ICacheManager cacheManager;
		private readonly ILogInspector logInspector;
		private readonly ICruiseManagerWrapper manager;
		private readonly IRequestWrapper requestWrapper;

		public LogViewer(IRequestWrapper requestWrapper, ICruiseManagerWrapper manager, ILogInspector logInspector, ICacheManager cacheManager)
		{
			this.requestWrapper = requestWrapper;
			this.manager = manager;
			this.logInspector = logInspector;
			this.cacheManager = cacheManager;
		}

		public LogViewerResults Do()
		{
			string serverName = requestWrapper.GetServerName();
			string projectName = requestWrapper.GetProjectName();
			string log = manager.GetLog(serverName, projectName, requestWrapper.GetLogSpecifier());
			string logFilename = logInspector.GetLogFileName(log);
			cacheManager.AddContent(serverName, projectName, CacheDirectory, logFilename, log);

			return new LogViewerResults(cacheManager.GetURLForFile(serverName, projectName, CacheDirectory, logFilename));
		}
	}
}
