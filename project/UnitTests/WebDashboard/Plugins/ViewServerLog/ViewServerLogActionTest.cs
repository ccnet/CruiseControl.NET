using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewServerLog;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ViewServerLog
{
	[TestFixture]
	public class ViewServerLogActionTest
	{
		private ViewServerLogAction action;

		private DynamicMock farmServiceMock;
		private DynamicMock requestMock;
		private string serverName;
		private string serverLog;

		[SetUp]
		public void Setup()
		{
			requestMock = new DynamicMock(typeof(ICruiseRequest));
			farmServiceMock = new DynamicMock(typeof(IFarmService));

			action = new ViewServerLogAction((IFarmService) farmServiceMock.MockInstance);

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
			requestMock.ExpectAndReturn("ServerName", serverName);
			farmServiceMock.ExpectAndReturn("GetServerLog", serverLog, serverName);

			// Execute
			HtmlGenericControl control = (HtmlGenericControl) action.Execute((ICruiseRequest) requestMock.MockInstance);
			Assert.AreEqual("<pre>" + serverLog + "</pre>", control.InnerHtml);

			VerifyAll();
		}
	}
}
