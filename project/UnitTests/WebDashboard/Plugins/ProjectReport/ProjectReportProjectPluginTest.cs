using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
	[TestFixture]
	public class ProjectReportProjectPluginTest
	{
		private DynamicMock farmServiceMock;
		private DynamicMock viewGeneratorMock;
		private DynamicMock linkFactoryMock;
		private ProjectReportProjectPlugin plugin;
		private DynamicMock cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void Setup()
		{
			farmServiceMock = new DynamicMock(typeof(IFarmService));
			viewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			plugin = new ProjectReportProjectPlugin((IFarmService) farmServiceMock.MockInstance,
				(IVelocityViewGenerator) viewGeneratorMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance);

			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.MockInstance;
		}

		private void VerifyAll()
		{
			farmServiceMock.Verify();
			viewGeneratorMock.Verify();
			linkFactoryMock.Verify();
		}

		[Test]
		public void ShouldGetProjectDetailsAndUseCorrectTemplate()
		{
			// Setup
			ExternalLink[] links = new ExternalLink[] { new ExternalLink("foo", "bar") };
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
			Hashtable expectedContext = new Hashtable();
			expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = false;
			expectedContext["mostRecentBuildUrl"] = "buildUrl";
			IResponse response = new HtmlFragmentResponse("myView");

			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[] { buildSpecifier }, projectSpecifier, 1);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier);
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", new GeneralAbsoluteLink("foo", "buildUrl"), buildSpecifier, BuildReportBuildPlugin.ACTION_NAME);
			viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));

			// Execute
			IResponse returnedResponse = plugin.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}

		[Test]
		public void ShouldMarkNoBuildsAvailableIfNoBuildSpecifiersReturnedByRemoteServer()
		{
			// Setup
			ExternalLink[] links = new ExternalLink[] { new ExternalLink("foo", "bar") };
			Hashtable expectedContext = new Hashtable();
			expectedContext["projectName"] = "myProject";
			expectedContext["externalLinks"] = links;
			expectedContext["noLogsAvailable"] = true;
			IResponse response = new HtmlFragmentResponse("myView");

			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			farmServiceMock.ExpectAndReturn("GetMostRecentBuildSpecifiers", new IBuildSpecifier[0], projectSpecifier, 1);
			farmServiceMock.ExpectAndReturn("GetExternalLinks", links, projectSpecifier);
			viewGeneratorMock.ExpectAndReturn("GenerateView", response, @"ProjectReport.vm", new HashtableConstraint(expectedContext));

			// Execute
			IResponse returnedResponse = plugin.Execute(cruiseRequest);

			// Verify
			Assert.AreEqual(response, returnedResponse);
			VerifyAll();
		}
	}
}
