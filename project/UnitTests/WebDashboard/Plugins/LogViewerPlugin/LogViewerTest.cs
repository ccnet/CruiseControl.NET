using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.BuildLog;
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
		private DynamicMock logInspectorMock;

		private LogViewer logViewer;
		private DynamicMock cacheManagerMock;

		[SetUp]
		public void Setup()
		{
			requestWrapperMock = new DynamicMock(typeof(IRequestWrapper));
			cruiseManagerMock = new DynamicMock(typeof(ICruiseManagerWrapper));
			logInspectorMock = new DynamicMock(typeof(ILogInspector));
			cacheManagerMock = new DynamicMock(typeof(ICacheManager));

			logViewer = new LogViewer((IRequestWrapper) requestWrapperMock.MockInstance, (ICruiseManagerWrapper) cruiseManagerMock.MockInstance,
				(ILogInspector) logInspectorMock.MockInstance, (ICacheManager) cacheManagerMock.MockInstance);
		}

		[Test]
		public void GetsLatestLogPutsItInCacheAndReturnsURLOfFile()
		{
			ILogSpecifier logSpecifier = new NoLogSpecified();
			string serverName = "my Server";
			string projectName = "my Project";
			string logContent = "log Content";
			string logFileName = "myLogfile.xml";

			requestWrapperMock.SetupResult("GetLogSpecifier", logSpecifier);
			requestWrapperMock.SetupResult("GetServerName", serverName);
			requestWrapperMock.SetupResult("GetProjectName", projectName);

			cruiseManagerMock.ExpectAndReturn("GetLog", logContent, serverName, projectName, logSpecifier);
			logInspectorMock.ExpectAndReturn("GetLogFileName", logFileName, logContent);
			cacheManagerMock.Expect("AddContent", serverName, projectName, LogViewer.CacheDirectory, logFileName, logContent);
			cacheManagerMock.ExpectAndReturn("GetURLForFile", "http://foo.bar/baz.xml" , serverName, projectName, LogViewer.CacheDirectory, logFileName);

			LogViewerResults results = logViewer.Do();
			AssertEquals("http://foo.bar/baz.xml", results.RedirectURL);

			cruiseManagerMock.Verify();
			logInspectorMock.Verify();
			cacheManagerMock.Verify();
		}
	}
}
