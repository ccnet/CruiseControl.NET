using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class CachingBuildRetrieverTest : Assertion
	{
		private DynamicMock cacheManagerMock;
		private DynamicMock slaveBuildRetrieverMock;
		private CachingBuildRetriever cachingBuildRetriever;
		private string serverName;
		private string projectName;
		private string logContent;
		private string buildName;
		private Build build;

		[SetUp]
		public void Setup()
		{
			cacheManagerMock = new DynamicMock(typeof(ICacheManager));
			slaveBuildRetrieverMock = new DynamicMock(typeof(IBuildRetriever));

			cachingBuildRetriever = new CachingBuildRetriever((ICacheManager) cacheManagerMock.MockInstance,
				(IBuildRetriever) slaveBuildRetrieverMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			logContent = "log Content";
			buildName = "myLogfile.xml";
			build = new Build(buildName, logContent, serverName, projectName);
		}

		[Test]
		public void ReturnsBuildAndPutsItInCacheIfNotAlreadyThere()
		{
			slaveBuildRetrieverMock.ExpectAndReturn("GetBuild", build, serverName, projectName, buildName);

			cacheManagerMock.ExpectAndReturn("GetContent", null, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.Expect("AddContent", serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName, logContent);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);

			Build returnedBuild = cachingBuildRetriever.GetBuild(serverName, projectName, buildName);
			AssertEquals(buildName, returnedBuild.Name);
			AssertEquals(logContent, returnedBuild.Log);
			AssertEquals(serverName, returnedBuild.ServerName);
			AssertEquals(projectName, returnedBuild.ProjectName);

			VerifyAll();
		}
		
		[Test]
		public void IfBuildInCacheThenDoesntGetLogContentFromCruiseServer()
		{
			slaveBuildRetrieverMock.ExpectNoCall("GetBuild", typeof(string), typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, CachingBuildRetriever.CacheDirectory, buildName);
			cacheManagerMock.ExpectNoCall("AddContent",typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

			Build returnedBuild = cachingBuildRetriever.GetBuild(serverName, projectName, buildName);
			AssertEquals(buildName, returnedBuild.Name);
			AssertEquals(logContent, returnedBuild.Log);
			AssertEquals(serverName, returnedBuild.ServerName);
			AssertEquals(projectName, returnedBuild.ProjectName);

			VerifyAll();
		}

		private void VerifyAll()
		{
			cacheManagerMock.Verify();
			slaveBuildRetrieverMock.Verify();
		}
	}
}
