using System.Collections;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
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
		private DynamicMock viewGeneratorMock;
		private DynamicMock cruiseUrlBuilderMock;

		[SetUp]
		public void Setup()
		{
			requestMock = new DynamicMock(typeof(ICruiseRequest));
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			viewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			cruiseUrlBuilderMock = new DynamicMock(typeof(ICruiseUrlBuilder));

			action = new ServerLogServerPlugin((IFarmService) farmServiceMock.MockInstance, (IVelocityViewGenerator) viewGeneratorMock.MockInstance, 
				(ICruiseUrlBuilder) cruiseUrlBuilderMock.MockInstance);
		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			requestMock.Verify();
			viewGeneratorMock.Verify();
			cruiseUrlBuilderMock.Verify();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServer()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("foo");
			string serverLog = "server log";
			Hashtable expectedHashtable = new Hashtable();
			expectedHashtable["log"] = serverLog;
			expectedHashtable["projectLinks"] = new IsAnything();

			IResponse response = new HtmlFragmentResponse("foo");

			requestMock.SetupResult("ServerSpecifier", serverSpecifier);
			farmServiceMock.ExpectAndReturn("GetServerLog", serverLog, serverSpecifier, null);
			farmServiceMock.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions", new ProjectStatusListAndExceptions(new ProjectStatusOnServer[0], null), serverSpecifier, null);
			viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ServerLog.vm", new HashtableConstraint(expectedHashtable));

			// Execute
			Assert.AreEqual(response, action.Execute((ICruiseRequest) requestMock.MockInstance));

			VerifyAll();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServerForSpecificProject()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("foo");
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "foo-project");
			string serverLog = "server log";
			Hashtable expectedHashtable = new Hashtable();
			expectedHashtable["log"] = serverLog;
			expectedHashtable["projectLinks"] = new IsAnything();
			expectedHashtable["currentProject"] = projectSpecifier.ProjectName;

			IResponse response = new HtmlFragmentResponse("foo");

			requestMock.SetupResult("ServerSpecifier", serverSpecifier);			
			requestMock.SetupResult("ProjectName", projectSpecifier.ProjectName);
			requestMock.SetupResult("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetServerLog", serverLog, projectSpecifier, null);
			farmServiceMock.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions", new ProjectStatusListAndExceptions(new ProjectStatusOnServer[0], null), serverSpecifier, null);			
			viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ServerLog.vm", new HashtableConstraint(expectedHashtable));

			// Execute
			Assert.AreEqual(response, action.Execute((ICruiseRequest) requestMock.MockInstance));

			VerifyAll();
		}
	}
}
