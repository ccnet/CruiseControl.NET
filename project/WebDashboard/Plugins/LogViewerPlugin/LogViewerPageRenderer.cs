using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewerPageRenderer
	{
		private readonly ICacheManager cacheManager;
		private readonly ICruiseRequestWrapper requestWrapper;
		private readonly IBuildRetrieverForRequest buildRetrieverForRequest;

		public LogViewerPageRenderer(ICruiseRequestWrapper requestWrapper, IBuildRetrieverForRequest buildRetrieverForRequest, ICacheManager cacheManager)
		{
			this.buildRetrieverForRequest = buildRetrieverForRequest;
			this.requestWrapper = requestWrapper;
			this.cacheManager = cacheManager;
		}

		public LogViewerResults Do()
		{
			Build build = buildRetrieverForRequest.GetBuild(requestWrapper);
			// ToDo - better way of specifiying the Cache Directory
			string Url = cacheManager.GetURLForFile(build.ServerName, build.ProjectName, CachingBuildRetriever.CacheDirectory, build.Name);
			return new LogViewerResults(Url);
		}
	}
}
