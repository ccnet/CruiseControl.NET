using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Cache;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.LogViewerPlugin
{
	[TestFixture]
	public class LogViewerTest : Assertion
	{
		private DynamicMock requestWrapperMock;
		private DynamicMock cruiseManagerMock;

		private LogViewer logViewer;
		private DynamicMock cacheManagerMock;
		private string serverName;
		private string projectName;
		private string logContent;
		private string logFileName;

		[SetUp]
		public void Setup()
		{
			requestWrapperMock = new DynamicMock(typeof(IRequestWrapper));
			cruiseManagerMock = new DynamicMock(typeof(ICruiseManagerWrapper));
			cacheManagerMock = new DynamicMock(typeof(ICacheManager));

			logViewer = new LogViewer((IRequestWrapper) requestWrapperMock.MockInstance, (ICruiseManagerWrapper) cruiseManagerMock.MockInstance,
				(ICacheManager) cacheManagerMock.MockInstance);

			serverName = "my Server";
			projectName = "my Project";
			logContent = "log Content";
			logFileName = "myLogfile.xml";
		}

		[Test]
		public void GetsLatestLogAndPutsItInCacheAndReturnsURLOfFileInCache()
		{
			requestWrapperMock.ExpectAndReturn("GetServerName", serverName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", projectName);
			requestWrapperMock.ExpectAndReturn("GetBuildSpecifier", new NoLogSpecified());

			cruiseManagerMock.ExpectAndReturn("GetLatestLogName", logFileName, serverName, projectName);
			cruiseManagerMock.ExpectAndReturn("GetLog", logContent, serverName, projectName, logFileName);

			cacheManagerMock.ExpectAndReturn("GetContent", null, serverName, projectName, LogViewer.CacheDirectory, logFileName);
			cacheManagerMock.Expect("AddContent", serverName, projectName, LogViewer.CacheDirectory, logFileName, logContent);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, LogViewer.CacheDirectory, logFileName);

			LogViewerResults results = logViewer.Do();
			AssertEquals("http://foo.bar/baz.xml", results.RedirectURL);

			requestWrapperMock.Verify();
			cruiseManagerMock.Verify();
			cacheManagerMock.Verify();
		}

		[Test]
		public void GetsSpecifiedLogAndPutsItInCacheAndReturnsURLOfFileInCache()
		{
			requestWrapperMock.SetupResult("GetServerName", serverName);
			requestWrapperMock.SetupResult("GetProjectName", projectName);
			requestWrapperMock.SetupResult("GetBuildSpecifier", new FileNameLogSpecifier(logFileName));

			cruiseManagerMock.ExpectAndReturn("GetLog", logContent, serverName, projectName, logFileName);
			cruiseManagerMock.ExpectNoCall("GetLatestLogName", typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", null, serverName, projectName, LogViewer.CacheDirectory, logFileName);
			cacheManagerMock.Expect("AddContent", serverName, projectName, LogViewer.CacheDirectory, logFileName, logContent);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, LogViewer.CacheDirectory, logFileName);
			
			LogViewerResults results = logViewer.Do();
			AssertEquals("http://foo.bar/baz.xml", results.RedirectURL);

			requestWrapperMock.Verify();
			cruiseManagerMock.Verify();
			cacheManagerMock.Verify();
		}

		[Test]
		public void IfLogNotSpecifiedButLatestIsInCacheThenDoesntGetLogContentFromCruiseServer()
		{
			requestWrapperMock.ExpectAndReturn("GetServerName", serverName);
			requestWrapperMock.ExpectAndReturn("GetProjectName", projectName);
			requestWrapperMock.ExpectAndReturn("GetBuildSpecifier", new NoLogSpecified());
			
			cruiseManagerMock.ExpectAndReturn("GetLatestLogName", logFileName, serverName, projectName);
			cruiseManagerMock.ExpectNoCall("GetLog", typeof(string), typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, LogViewer.CacheDirectory, logFileName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, LogViewer.CacheDirectory, logFileName);
			cacheManagerMock.ExpectNoCall("AddContent",typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));

			LogViewerResults results = logViewer.Do();
			AssertEquals("http://foo.bar/baz.xml", results.RedirectURL);

			requestWrapperMock.Verify();
			cruiseManagerMock.Verify();
			cacheManagerMock.Verify();
		}

		[Test]
		public void IfLogSpecifiedAndInCacheThenDoesntCallCruiseServer()
		{
			requestWrapperMock.SetupResult("GetServerName", serverName);
			requestWrapperMock.SetupResult("GetProjectName", projectName);
			requestWrapperMock.SetupResult("GetBuildSpecifier", new FileNameLogSpecifier(logFileName));

			cruiseManagerMock.ExpectNoCall("GetLatestLogName", typeof(string), typeof(string));
			cruiseManagerMock.ExpectNoCall("GetLog", typeof(string), typeof(string), typeof(string));

			cacheManagerMock.ExpectAndReturn("GetContent", logContent, serverName, projectName, LogViewer.CacheDirectory, logFileName);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, LogViewer.CacheDirectory, logFileName);
			cacheManagerMock.ExpectNoCall("AddContent",typeof(string), typeof(string), typeof(string), typeof(string), typeof(string));
			
			LogViewerResults results = logViewer.Do();
			AssertEquals("http://foo.bar/baz.xml", results.RedirectURL);

			requestWrapperMock.Verify();
			cruiseManagerMock.Verify();
			cacheManagerMock.Verify();
		}
	}
}
