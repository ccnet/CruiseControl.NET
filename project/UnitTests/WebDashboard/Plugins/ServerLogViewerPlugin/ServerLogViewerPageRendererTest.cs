using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerLogViewerPlugin;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ServerLogViewerPlugin
{
	[TestFixture]
	public class ServerLogViewerPageRendererTest : Assertion
	{
		private ServerLogViewerPageRenderer serverLogViewerPageRenderer;

		private DynamicMock cruiseManagerWrapperMock;
		private DynamicMock requestWrapperMock;
		private string serverName;
		private string serverLog;

		[SetUp]
		public void Setup()
		{
			requestWrapperMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseManagerWrapperMock = new DynamicMock(typeof(ICruiseManagerWrapper));

			serverLogViewerPageRenderer = new ServerLogViewerPageRenderer((ICruiseRequest) requestWrapperMock.MockInstance, 
				(ICruiseManagerWrapper) cruiseManagerWrapperMock.MockInstance);

			serverName = "myserver";
			serverLog = "Some Stuff on the server";
		}

		private void VerifyAll()
		{
            cruiseManagerWrapperMock.Verify();
			requestWrapperMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			requestWrapperMock.ExpectAndReturn("ServerName", serverName);
			cruiseManagerWrapperMock.ExpectAndReturn("GetServerLog", serverLog, serverName);

			AssertEquals("<pre>" + serverLog + "</pre>", serverLogViewerPageRenderer.Do().LogHtml);

			VerifyAll();
		}
	}
}
