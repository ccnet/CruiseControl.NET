using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWkorks.CruiseControl.UnitTests.WebDashboard.Plugins.ViewServerLog
{
	[TestFixture]
	public class ServerLogServerPluginTest
	{
		private ServerLogServerPlugin action;

		private DynamicMock farmServiceMock;
		private DynamicMock requestMock;
		private DynamicMock viewGeneratorMock;

		[SetUp]
		public void Setup()
		{
			requestMock = new DynamicMock(typeof(ICruiseRequest));
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			viewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));

			action = new ServerLogServerPlugin((IFarmService) farmServiceMock.MockInstance, (IVelocityViewGenerator) viewGeneratorMock.MockInstance);
		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			requestMock.Verify();
			viewGeneratorMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("foo");
			string serverLog = "server log";
			Hashtable expectedHashtable = new Hashtable();
			expectedHashtable["log"] = serverLog;

			IResponse response = new HtmlFragmentResponse("foo");

			requestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			farmServiceMock.ExpectAndReturn("GetServerLog", serverLog, serverSpecifier);
			viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ServerLog.vm", new HashtableConstraint(expectedHashtable));

			// Execute
			Assert.AreEqual(response, action.Execute((ICruiseRequest) requestMock.MockInstance));

			VerifyAll();
		}
	}
}
