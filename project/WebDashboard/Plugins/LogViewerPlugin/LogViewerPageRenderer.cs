using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewerPageRenderer
	{
		private readonly ICacheManager cacheManager;
		private readonly ICruiseRequest request;
		private readonly IBuildRetrieverForRequest buildRetrieverForRequest;

		public LogViewerPageRenderer(ICruiseRequest request, IBuildRetrieverForRequest buildRetrieverForRequest, ICacheManager cacheManager)
		{
			this.buildRetrieverForRequest = buildRetrieverForRequest;
			this.request = request;
			this.cacheManager = cacheManager;
		}

		public LogViewerResults Do()
		{
			Build build = buildRetrieverForRequest.GetBuild(request);
			// ToDo - better way of specifiying the Cache Directory
			string Url = cacheManager.GetURLForFile(build.ServerName, build.ProjectName, CachingBuildRetriever.CacheDirectory, build.Name);
			return new LogViewerResults(Url);
		}
	}
}
