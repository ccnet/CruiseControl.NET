using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CachingBuildRetrieverTest
	{
		private DynamicMock cacheManagerMock;
		private DynamicMock cruiseManagerWrapperMock;
		private CachingBuildRetriever cachingBuildRetriever;
		private string serverName;
		private string projectName;
		private string logContent;
		private string buildName;
		private string logLocation;

		[SetUp]
		public void Setup()
		{
			cacheManagerMock = new DynamicMock(typeof(ICacheManager));
			cruiseManagerWrapperMock = new DynamicMock(typeof(ICruiseManagerWrapper));

			cachingBuildRetriever = new CachingBuildRetriever((ICacheManager) cacheManagerMock.MockInstance,
				(ICruiseManagerWrapper) cruiseManagerWrapperMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			buildName = "myBuild";
			logContent = "log Content";
			logLocation = "http://somewhere/mylog";
		}

		[Test]
		public void ReturnsBuildAndPutsItInCacheIfNotAlreadyThere()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetLog", logContent, serverName, projectName, buildName);

			cacheManagerMock.ExpectAndReturn("GetContent", null, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.Expect("AddContent", serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName, logContent);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", logLocation, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);

			Build returnedBuild = cachingBuildRetriever.GetBuild(serverName, projectName, buildName);
			Assert.AreEqual(buildName, returnedBuild.Name);
			Assert.AreEqual(logContent, returnedBuild.Log);
			Assert.AreEqual(serverName, returnedBuild.ServerName);
			Assert.AreEqual(projectName, returnedBuild.ProjectName);
			Assert.AreEqual(logLocation, returnedBuild.BuildLogLocation);

			VerifyAll();
		}
		
		[Test]
		public void IfBuildInCacheThenDoesntGetLogContentFromCruiseServer()
		{
			cruiseManagerWrapperMock.ExpectNoCall("GetLog", typeof(string), typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectNoCall("AddContent",typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
			cacheManagerMock.ExpectAndReturn("GetURLForFile", logLocation, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);

			Build returnedBuild = cachingBuildRetriever.GetBuild(serverName, projectName, buildName);
			Assert.AreEqual(buildName, returnedBuild.Name);
			Assert.AreEqual(logContent, returnedBuild.Log);
			Assert.AreEqual(serverName, returnedBuild.ServerName);
			Assert.AreEqual(projectName, returnedBuild.ProjectName);
			Assert.AreEqual(logLocation, returnedBuild.BuildLogLocation);

			VerifyAll();
		}

		private void VerifyAll()
		{
			cacheManagerMock.Verify();
			cruiseManagerWrapperMock.Verify();
		}
	}
}
