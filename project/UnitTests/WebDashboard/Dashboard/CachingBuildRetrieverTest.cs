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
		private DefaultBuildSpecifier buildSpecifier;
		private IProjectSpecifier projectSpecifier;

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
			projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier(serverName), projectName);
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, buildName);
			logContent = "log Content";
			logLocation = "http://somewhere/mylog";
		}

		[Test]
		public void ReturnsBuildAndPutsItInCacheIfNotAlreadyThere()
		{
			cruiseManagerWrapperMock.ExpectAndReturn("GetLog", logContent, buildSpecifier);

			cacheManagerMock.ExpectAndReturn("GetContent", null, projectSpecifier, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.Expect("AddContent", projectSpecifier, CachingBuildRetriever.CacheDirectory, buildName, logContent);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, projectSpecifier, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", logLocation, projectSpecifier, CachingBuildRetriever.CacheDirectory, buildName);

			Build returnedBuild = cachingBuildRetriever.GetBuild(buildSpecifier);
			Assert.AreEqual(buildSpecifier, returnedBuild.BuildSpecifier);
			Assert.AreEqual(logContent, returnedBuild.Log);
			Assert.AreEqual(logLocation, returnedBuild.BuildLogLocation);

			VerifyAll();
		}
		
		[Test]
		public void IfBuildInCacheThenDoesntGetLogContentFromCruiseServer()
		{
			cruiseManagerWrapperMock.ExpectNoCall("GetLog", typeof(IBuildSpecifier));

			cacheManagerMock.ExpectAndReturn("GetContent", logContent, projectSpecifier, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, projectSpecifier, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectNoCall("AddContent", typeof(IProjectSpecifier), typeof(string), typeof(string), typeof(string));
			cacheManagerMock.ExpectAndReturn("GetURLForFile", logLocation, projectSpecifier, CachingBuildRetriever.CacheDirectory, buildName);

			Build returnedBuild = cachingBuildRetriever.GetBuild(buildSpecifier);
			Assert.AreEqual(buildSpecifier, returnedBuild.BuildSpecifier);
			Assert.AreEqual(logContent, returnedBuild.Log);
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
