using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CachingBuildRetrieverTest : Assertion
	{
		private DynamicMock cruiseManagerMock;
		private DynamicMock cacheManagerMock;
		private DynamicMock requestWrapperMock;
		private CachingBuildRetriever cachingBuildRetriever;
		private string serverName;
		private string projectName;
		private string logContent;
		private string buildName;

		[SetUp]
		public void Setup()
		{
			cruiseManagerMock = new DynamicMock(typeof(ICruiseManagerWrapper));
			cacheManagerMock = new DynamicMock(typeof(ICacheManager));
			requestWrapperMock = new DynamicMock(typeof(IRequestWrapper));

			cachingBuildRetriever = new CachingBuildRetriever((ICruiseManagerWrapper) cruiseManagerMock.MockInstance,
				(ICacheManager) cacheManagerMock.MockInstance, (IRequestWrapper) requestWrapperMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			logContent = "log Content";
			buildName = "myLogfile.xml";
		}

		[Test]
		public void ReturnsLatestLogAndPutsItInCacheIfNotAlreadyThere()
		{
			requestWrapperMock.ExpectAndReturn("GetServerName", serverName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", projectName);
			requestWrapperMock.ExpectAndReturn("GetBuildSpecifier", new NoLogSpecified());

			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", buildName, serverName, projectName);
			cruiseManagerMock.ExpectAndReturn("GetLog", logContent, serverName, projectName, buildName);

			cacheManagerMock.ExpectAndReturn("GetContent", null, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.Expect("AddContent", serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName, logContent);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);

			Build build = cachingBuildRetriever.GetBuild();
			AssertEquals(buildName, build.Name);
			AssertEquals(logContent, build.Log);
			AssertEquals("http://foo.bar/baz.xml", build.Url);

			VerifyAll();
		}

		[Test]
		public void GetsSpecifiedLogAndPutsItInCacheAndReturnsURLOfFileInCache()
		{
			requestWrapperMock.SetupResult("GetServerName", serverName);
			requestWrapperMock.SetupResult("GetProjectName", projectName);
			requestWrapperMock.SetupResult("GetBuildSpecifier", new FileNameLogSpecifier(buildName));

			cruiseManagerMock.ExpectAndReturn("GetLog", logContent, serverName, projectName, buildName);
			cruiseManagerMock.ExpectNoCall("GetLatestBuildName", typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", null, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.Expect("AddContent", serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName, logContent);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			
			Build build = cachingBuildRetriever.GetBuild();
			AssertEquals(buildName, build.Name);
			AssertEquals(logContent, build.Log);
			AssertEquals("http://foo.bar/baz.xml", build.Url);

			VerifyAll();
		}
		
		[Test]
		public void IfLogNotSpecifiedButLatestIsInCacheThenDoesntGetLogContentFromCruiseServer()
		{
			requestWrapperMock.ExpectAndReturn("GetServerName", serverName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", projectName);
			requestWrapperMock.ExpectAndReturn("GetBuildSpecifier", new NoLogSpecified());

			cruiseManagerMock.ExpectAndReturn("GetLatestBuildName", buildName, serverName, projectName);
			cruiseManagerMock.ExpectNoCall("GetLog", typeof(string), typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectNoCall("AddContent",typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

			Build build = cachingBuildRetriever.GetBuild();
			AssertEquals(buildName, build.Name);
			AssertEquals(logContent, build.Log);
			AssertEquals("http://foo.bar/baz.xml", build.Url);

			VerifyAll();
		}

		[Test]
		public void IfLogSpecifiedAndInCacheThenDoesntCallCruiseServer()
		{
			requestWrapperMock.SetupResult("GetServerName", serverName);
			requestWrapperMock.SetupResult("GetProjectName", projectName);
			requestWrapperMock.SetupResult("GetBuildSpecifier", new FileNameLogSpecifier(buildName));

			cruiseManagerMock.ExpectNoCall("GetLatestBuildName", typeof(string), typeof(string));
			cruiseManagerMock.ExpectNoCall("GetLog", typeof(string), typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectNoCall("AddContent",typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
			
			Build build = cachingBuildRetriever.GetBuild();
			AssertEquals(buildName, build.Name);
			AssertEquals(logContent, build.Log);
			AssertEquals("http://foo.bar/baz.xml", build.Url);

			VerifyAll();
		}
		
		private void VerifyAll()
		{
			cruiseManagerMock.Verify();
			cacheManagerMock.Verify();
			requestWrapperMock.Verify();
		}
	}
}
