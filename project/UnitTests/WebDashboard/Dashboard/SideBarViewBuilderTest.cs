using System.Collections;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	[TestFixture]
	public class SideBarViewBuilderTest
	{
		private DynamicMock cruiseRequestWrapperMock;
		private DynamicMock buildNameRetrieverMock;
		private DynamicMock recentBuildsViewBuilderMock;
		private DynamicMock pluginLinkCalculatorMock;
		private DynamicMock velocityViewGeneratorMock;
		private DynamicMock linkFactoryMock;

		private SideBarViewBuilder sideBarViewBuilder;

		private IView velocityView;
		private Hashtable velocityContext;
		private IAbsoluteLink[] links;

		[SetUp]
		public void Setup()
		{
			cruiseRequestWrapperMock = new DynamicMock(typeof(ICruiseRequest));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));
			recentBuildsViewBuilderMock = new DynamicMock(typeof(IRecentBuildsViewBuilder));
			pluginLinkCalculatorMock = new DynamicMock(typeof(IPluginLinkCalculator));
			velocityViewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));

			sideBarViewBuilder = new SideBarViewBuilder(
				(ICruiseRequest) cruiseRequestWrapperMock.MockInstance,
				(IBuildNameRetriever) buildNameRetrieverMock.MockInstance,
				(IRecentBuildsViewBuilder) recentBuildsViewBuilderMock.MockInstance,
				(IPluginLinkCalculator) pluginLinkCalculatorMock.MockInstance,
				(IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance);

			velocityView = new DefaultView("velocity view");
			velocityContext = new Hashtable();
			links = new IAbsoluteLink[] { new GeneralAbsoluteLink("link")};
		}
		
		private void VerifyAll()
		{
			cruiseRequestWrapperMock.Verify();
			buildNameRetrieverMock.Verify();
			recentBuildsViewBuilderMock.Verify();
			pluginLinkCalculatorMock.Verify();
			velocityViewGeneratorMock.Verify();
			linkFactoryMock.Verify();
		}

		[Test]
		public void ShouldGenerateFarmViewIfNoServerSpecified()
		{
			// Setup
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "");
			pluginLinkCalculatorMock.ExpectAndReturn("GetFarmPluginLinks", links);

			velocityContext["links"] = links;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityView, @"FarmSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
			IView returnedView = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(velocityView, returnedView);
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateServerViewIfServerButNoProjectSpecified()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerSpecifier", serverSpecifier);

			pluginLinkCalculatorMock.ExpectAndReturn("GetServerPluginLinks", links, serverSpecifier);

			velocityContext["links"] = links;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityView, @"ServerSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
			IView returnedView = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(velocityView, returnedView);
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateProjectViewIfServerAndProjectButNoBuildSpecified()
		{
			// Setup
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildName", "");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);

			pluginLinkCalculatorMock.ExpectAndReturn("GetProjectPluginLinks", links, projectSpecifier);
			string recentBuildsView = "";
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", recentBuildsView, projectSpecifier);

			velocityContext["links"] = links;
			velocityContext["recentBuildsTable"] = recentBuildsView;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityView, @"ProjectSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
			IView returnedView = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(velocityView, returnedView);
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateBuildViewIfServerAndProjectAndBuildSpecified()
		{
			// Setup
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			IBuildSpecifier buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "myServer");
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectName", "myProject");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildName", "myBuild");
			cruiseRequestWrapperMock.ExpectAndReturn("BuildSpecifier", buildSpecifier);

			pluginLinkCalculatorMock.ExpectAndReturn("GetBuildPluginLinks", links, buildSpecifier);
			string recentBuildsView = "";
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", recentBuildsView, projectSpecifier);

			IBuildSpecifier latestBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "latest");
			IBuildSpecifier nextBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "next");
			IBuildSpecifier previousBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "previous");
			IAbsoluteLink latestLink = new GeneralAbsoluteLink("test latest");
			IAbsoluteLink nextLink = new GeneralAbsoluteLink("test next");
			IAbsoluteLink previousLink = new GeneralAbsoluteLink("test previous");

			buildNameRetrieverMock.ExpectAndReturn("GetLatestBuildSpecifier", latestBuildSpecifier, projectSpecifier);
			buildNameRetrieverMock.ExpectAndReturn("GetNextBuildSpecifier", nextBuildSpecifier, buildSpecifier);
			buildNameRetrieverMock.ExpectAndReturn("GetPreviousBuildSpecifier", previousBuildSpecifier, buildSpecifier);

			ActionSpecifierWithName actionSpecifier = new ActionSpecifierWithName(BuildReportBuildPlugin.ACTION_NAME);

			linkFactoryMock.ExpectAndReturn("CreateBuildLink", latestLink, latestBuildSpecifier, "", actionSpecifier);
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", nextLink, nextBuildSpecifier, "", actionSpecifier);
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", previousLink, previousBuildSpecifier, "", actionSpecifier);

			velocityContext["links"] = links;
			velocityContext["recentBuildsTable"] = recentBuildsView;
			velocityContext["latestLink"] = latestLink;
			velocityContext["nextLink"] = nextLink;
			velocityContext["previousLink"] = previousLink;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityView, @"BuildSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
			IView returnedView = sideBarViewBuilder.Execute();

			// Verify
			Assert.AreEqual(velocityView, returnedView);
			VerifyAll();
		}
	}
}
