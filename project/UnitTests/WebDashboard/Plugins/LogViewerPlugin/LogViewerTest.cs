using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.LogViewerPlugin
{
	[TestFixture]
	public class LogViewerTest
	{
		private LogViewerPageRenderer logViewerPageRenderer;

		private string url;
		private Build build;
		private DynamicMock buildRetrieverforRequestMock;
		private DynamicMock requestWrapperMock;
		private DynamicMock cacheManagerMock;
		private string serverName;
		private string projectName;

		[SetUp]
		public void Setup()
		{
			buildRetrieverforRequestMock = new DynamicMock(typeof(IBuildRetrieverForRequest));
			requestWrapperMock = new DynamicMock(typeof(ICruiseRequest));
			cacheManagerMock = new DynamicMock(typeof(ICacheManager));

			logViewerPageRenderer = new LogViewerPageRenderer((ICruiseRequest) requestWrapperMock.MockInstance, (IBuildRetrieverForRequest) buildRetrieverforRequestMock.MockInstance,
				(ICacheManager) cacheManagerMock.MockInstance);

			url = "http://foo.bar";
			serverName = "myserver";
			projectName = "myproject";
			build  = new Build("mybuild", "", serverName, projectName);
		}

		private void VerifyAll()
		{
			buildRetrieverforRequestMock.Verify();
			requestWrapperMock.Verify();
			cacheManagerMock.Verify();
		}

		[Test]
		public void ReturnsURLOfRelevantBuild()
		{
			ICruiseRequest request = (ICruiseRequest) requestWrapperMock.MockInstance;

			buildRetrieverforRequestMock.ExpectAndReturn("GetBuild", build, request);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", url, serverName, projectName, CachingBuildRetriever.CacheDirectory, "mybuild");

			Assert.AreEqual(url, logViewerPageRenderer.Do().RedirectURL);
		}
	}
}
