using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ViewServerLog
{
	[TestFixture]
	public class ViewServerLogActionTest
	{
		private ServerLogServerPlugin action;

		private DynamicMock farmServiceMock;
		private DynamicMock requestMock;
		private string serverName;
		private string serverLog;

		[SetUp]
		public void Setup()
		{
			requestMock = new DynamicMock(typeof(ICruiseRequest));
			farmServiceMock = new DynamicMock(typeof(IFarmService));

			action = new ServerLogServerPlugin((IFarmService) farmServiceMock.MockInstance);

			serverName = "myserver";
			serverLog = "Some Stuff on the server";
		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			requestMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier(serverName);
			requestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			farmServiceMock.ExpectAndReturn("GetServerLog", serverLog, serverSpecifier);

			// Execute
			HtmlGenericControl control = (HtmlGenericControl) action.Execute((ICruiseRequest) requestMock.MockInstance);
			Assert.AreEqual(@"<pre class=""log"">" + serverLog + "</pre>", control.InnerHtml);

			VerifyAll();
		}
	}
}
