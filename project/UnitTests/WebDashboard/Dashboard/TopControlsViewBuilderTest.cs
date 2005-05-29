using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	// ToDo - actually look at html
	[TestFixture]
	public class TopControlsViewBuilderTest
	{
		private TopControlsViewBuilder viewBuilder;
		private DynamicMock cruiseRequestMock;
		private DynamicMock linkFactoryMock;
		private DynamicMock velocityViewGeneratorMock;
		private DefaultServerSpecifier serverSpecifier;
		private DefaultProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;
		private Hashtable expectedVelocityContext;
		private IView view;
		private IAbsoluteLink link1;
		private IAbsoluteLink link2;
		private IAbsoluteLink link3;
		private IAbsoluteLink link4;

		[SetUp]
		public void Setup()
		{
			cruiseRequestMock = new DynamicMock(typeof(ICruiseRequest));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			velocityViewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));

			viewBuilder = new TopControlsViewBuilder((ICruiseRequest) cruiseRequestMock.MockInstance, (ILinkFactory) linkFactoryMock.MockInstance,
				(IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance);

			serverSpecifier = new DefaultServerSpecifier("myServer");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myProject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
			expectedVelocityContext = new Hashtable();
			view = new StringView("foo");
			link1 = new GeneralAbsoluteLink("1");
			link2 = new GeneralAbsoluteLink("2");
			link3 = new GeneralAbsoluteLink("3");
			link4 = new GeneralAbsoluteLink("4");
		}

		private void VerifyAll()
		{
			cruiseRequestMock.Verify();
			linkFactoryMock.Verify();
			velocityViewGeneratorMock.Verify();
		}

		[Test]
		public void ShouldGenerateFarmLinkIfNothingSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "");
			cruiseRequestMock.ExpectAndReturn("BuildName", "");

			expectedVelocityContext["serverName"] = "";
			expectedVelocityContext["projectName"] = "";
			expectedVelocityContext["buildName"] = "";

			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link1, "Dashboard", FarmReportFarmPlugin.ACTION_NAME);
			expectedVelocityContext["farmLink"] = link1;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", view, "TopMenu.vm", new HashtableConstraint(expectedVelocityContext));

			// Execute & Verify
			Assert.AreEqual(view, viewBuilder.Execute());
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateFarmAndServerLinksIfServerButNoProjectSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "");
			cruiseRequestMock.ExpectAndReturn("BuildName", "");
			cruiseRequestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);

			expectedVelocityContext["serverName"] = "myServer";
			expectedVelocityContext["projectName"] = "";
			expectedVelocityContext["buildName"] = "";

			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link1, "Dashboard", FarmReportFarmPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link2, serverSpecifier, ServerReportServerPlugin.ACTION_NAME);
			expectedVelocityContext["farmLink"] = link1;
			expectedVelocityContext["serverLink"] = link2;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", view, "TopMenu.vm", new HashtableConstraint(expectedVelocityContext));

			// Execute & Verify
			Assert.AreEqual(view, viewBuilder.Execute());
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateFarmServerAndProjectLinksIfServerAndProjectButNoBuildSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestMock.ExpectAndReturn("BuildName", "");
			cruiseRequestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);

			expectedVelocityContext["serverName"] = "myServer";
			expectedVelocityContext["projectName"] = "myProject";
			expectedVelocityContext["buildName"] = "";

			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link1, "Dashboard", FarmReportFarmPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link2, serverSpecifier, ServerReportServerPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", link3, projectSpecifier, ProjectReportProjectPlugin.ACTION_NAME);
			expectedVelocityContext["farmLink"] = link1;
			expectedVelocityContext["serverLink"] = link2;
			expectedVelocityContext["projectLink"] = link3;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", view, "TopMenu.vm", new HashtableConstraint(expectedVelocityContext));

			// Execute & Verify
			Assert.AreEqual(view, viewBuilder.Execute());
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateFarmServerProjectAndBuildLinksIfServerProjectAndBuildSpecified()
		{
			// Setup
			cruiseRequestMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestMock.ExpectAndReturn("BuildName", "myBuild");
			cruiseRequestMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);
			cruiseRequestMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);
			cruiseRequestMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);

			expectedVelocityContext["serverName"] = "myServer";
			expectedVelocityContext["projectName"] = "myProject";
			expectedVelocityContext["buildName"] = "myBuild";

			linkFactoryMock.ExpectAndReturn("CreateFarmLink", link1, "Dashboard", FarmReportFarmPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link2, serverSpecifier, ServerReportServerPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateProjectLink", link3, projectSpecifier, ProjectReportProjectPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", link4, buildSpecifier, BuildReportBuildPlugin.ACTION_NAME);
			expectedVelocityContext["farmLink"] = link1;
			expectedVelocityContext["serverLink"] = link2;
			expectedVelocityContext["projectLink"] = link3;
			expectedVelocityContext["buildLink"] = link4;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", view, "TopMenu.vm", new HashtableConstraint(expectedVelocityContext));

			// Execute & Verify
			Assert.AreEqual(view, viewBuilder.Execute());
			VerifyAll();
		}
	}
}
