using System.Collections;
using System.Web.UI.HtmlControls;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ViewServerLog
{
	[TestFixture]
	public class ServerLogServerPluginTest
	{
		private ServerLogServerPlugin action;

		private DynamicMock farmServiceMock;
		private DynamicMock requestMock;
		private DynamicMock hashtableTransformerMock;

		[SetUp]
		public void Setup()
		{
			requestMock = new DynamicMock(typeof(ICruiseRequest));
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			hashtableTransformerMock = new DynamicMock(typeof(IHashtableTransformer));

			action = new ServerLogServerPlugin((IFarmService) farmServiceMock.MockInstance, (IHashtableTransformer) hashtableTransformerMock.MockInstance);
		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			requestMock.Verify();
			hashtableTransformerMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("foo");
			string serverLog = "server log";
			Hashtable expectedHashtable = new Hashtable();
			expectedHashtable["log"] = serverLog;

			requestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			farmServiceMock.ExpectAndReturn("GetServerLog", serverLog, serverSpecifier);
			hashtableTransformerMock.ExpectAndReturn("Transform", "some html", new HashtableConstraint(expectedHashtable), @"templates\ServerLog.vm");

			// Execute
			HtmlGenericControl control = (HtmlGenericControl) action.Execute((ICruiseRequest) requestMock.MockInstance);
			Assert.AreEqual("some html", control.InnerHtml);

			VerifyAll();
		}
	}
}
