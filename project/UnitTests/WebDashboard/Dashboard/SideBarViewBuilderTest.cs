using System;
using System.Collections;
using System.Collections.Generic;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;

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
		private DynamicMock linkListFactoryMock;
		private DynamicMock farmServiceMock;

		private SideBarViewBuilder sideBarViewBuilder;

		private IResponse velocityResponse;
		private Hashtable velocityContext;
		private IAbsoluteLink[] links;
		private IAbsoluteLink[] serverLinks;
		private IServerSpecifier[] serverSpecifiers;


		[SetUp]
		public void Setup()
		{
			cruiseRequestWrapperMock = new DynamicMock(typeof(ICruiseRequest));
			buildNameRetrieverMock = new DynamicMock(typeof(IBuildNameRetriever));
			recentBuildsViewBuilderMock = new DynamicMock(typeof(IRecentBuildsViewBuilder));
			pluginLinkCalculatorMock = new DynamicMock(typeof(IPluginLinkCalculator));
			velocityViewGeneratorMock = new DynamicMock(typeof(IVelocityViewGenerator));
			linkFactoryMock = new DynamicMock(typeof(ILinkFactory));
			linkListFactoryMock = new DynamicMock(typeof(ILinkListFactory));
			farmServiceMock = new DynamicMock(typeof(IFarmService));


			sideBarViewBuilder = new SideBarViewBuilder(
				(ICruiseRequest) cruiseRequestWrapperMock.MockInstance,
				(IBuildNameRetriever) buildNameRetrieverMock.MockInstance,
				(IRecentBuildsViewBuilder) recentBuildsViewBuilderMock.MockInstance,
				(IPluginLinkCalculator) pluginLinkCalculatorMock.MockInstance,
				(IVelocityViewGenerator) velocityViewGeneratorMock.MockInstance,
				(ILinkFactory) linkFactoryMock.MockInstance,
				(ILinkListFactory)linkListFactoryMock.MockInstance,
				(IFarmService) farmServiceMock.MockInstance,
                null);

			velocityResponse = new HtmlFragmentResponse("velocity view");
			velocityContext = new Hashtable();
			links = new IAbsoluteLink[] { new GeneralAbsoluteLink("link")};
			serverLinks = new IAbsoluteLink[] { new GeneralAbsoluteLink("link")};
			serverSpecifiers = new IServerSpecifier[] {new DefaultServerSpecifier("test")};
		}
		
		private void VerifyAll()
		{
			cruiseRequestWrapperMock.Verify();
			buildNameRetrieverMock.Verify();
			recentBuildsViewBuilderMock.Verify();
			pluginLinkCalculatorMock.Verify();
			velocityViewGeneratorMock.Verify();
			linkFactoryMock.Verify();
			linkListFactoryMock.Verify();
			farmServiceMock.Verify();
		}

		[Test]
		public void ShouldGenerateFarmViewIfNoServerSpecified()
		{
			// Setup
			cruiseRequestWrapperMock.ExpectAndReturn("ServerName", "");
			pluginLinkCalculatorMock.ExpectAndReturn("GetFarmPluginLinks", links);
			farmServiceMock.ExpectAndReturn("GetServerSpecifiers", serverSpecifiers);            
			linkListFactoryMock.ExpectAndReturn("CreateServerLinkList", serverLinks, serverSpecifiers, "ViewServerReport");

            ProjectStatus ps = new ProjectStatus("", "", null, 0, 0, null, DateTime.Now, null, null, DateTime.Now, null, "", 0, new List<ParameterBase>());
            ProjectStatusOnServer[] psosa = new ProjectStatusOnServer[] { new ProjectStatusOnServer(ps, serverSpecifiers[0]) };
            ProjectStatusListAndExceptions pslae = new ProjectStatusListAndExceptions(psosa, new CruiseServerException[0]);
            farmServiceMock.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions", pslae, serverSpecifiers[0], (string)null);

			velocityContext["links"] = links;
			velocityContext["serverlinks"] = serverLinks;

            velocityContext["showCategories"] = false;
            velocityContext["categorylinks"] = null;
            CruiseControl.WebDashboard.Dashboard.DefaultLinkFactory x = new DefaultLinkFactory(new DefaultUrlBuilder(),null,null);

            IAbsoluteLink farmLink = x.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME);
            linkFactoryMock.ExpectAndReturn("CreateFarmLink", farmLink, "Dashboard", FarmReportFarmPlugin.ACTION_NAME);
            velocityContext["farmLink"] = farmLink;

            System.Diagnostics.Debug.WriteLine("starting");

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityResponse, @"FarmSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
            HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.MockInstance as ICruiseRequest);

			// Verify
			Assert.AreEqual(velocityResponse, returnedResponse);
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

            ProjectStatus ps = new ProjectStatus("", "myCategory", null, 0, 0, null, DateTime.Now, null, null, DateTime.Now, null, "", 0, new List<ParameterBase>());
			ProjectStatusOnServer[] psosa = new ProjectStatusOnServer[] { new ProjectStatusOnServer(ps, serverSpecifier) };
			ProjectStatusListAndExceptions pslae = new ProjectStatusListAndExceptions(psosa, new CruiseServerException[0]);
			farmServiceMock.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions", pslae, serverSpecifier, (string)null);

			IAbsoluteLink link = new GeneralAbsoluteLink("link");
			IAbsoluteLink[] categoryLinks = new GeneralAbsoluteLink[] { new GeneralAbsoluteLink("myCategory", "?Category=myCategory") };
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link, serverSpecifier, ServerReportServerPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateServerLink", link, serverSpecifier, ServerReportServerPlugin.ACTION_NAME);

			velocityContext["links"] = links;
			velocityContext["serverlink"] = link;
			velocityContext["showCategories"] = true;
			velocityContext["categorylinks"] = categoryLinks;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityResponse, @"ServerSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
            HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.MockInstance as ICruiseRequest);

			// Verify
			Assert.AreEqual(velocityResponse, returnedResponse);
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
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", recentBuildsView, projectSpecifier, null);

			velocityContext["links"] = links;
			velocityContext["recentBuildsTable"] = recentBuildsView;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityResponse, @"ProjectSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
            HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.MockInstance as ICruiseRequest);

			// Verify
			Assert.AreEqual(velocityResponse, returnedResponse);
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
			cruiseRequestWrapperMock.ExpectAndReturn("ProjectSpecifier", projectSpecifier);

			pluginLinkCalculatorMock.ExpectAndReturn("GetBuildPluginLinks", links, buildSpecifier);
			string recentBuildsView = "";
			recentBuildsViewBuilderMock.ExpectAndReturn("BuildRecentBuildsTable", recentBuildsView, buildSpecifier, null);

			IBuildSpecifier nextBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "next");
			IBuildSpecifier previousBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "previous");
			IAbsoluteLink latestLink = new GeneralAbsoluteLink("test latest");
			IAbsoluteLink nextLink = new GeneralAbsoluteLink("test next");
			IAbsoluteLink previousLink = new GeneralAbsoluteLink("test previous");

			buildNameRetrieverMock.ExpectAndReturn("GetNextBuildSpecifier", nextBuildSpecifier, buildSpecifier, null);
			buildNameRetrieverMock.ExpectAndReturn("GetPreviousBuildSpecifier", previousBuildSpecifier, buildSpecifier, null);

			string action = BuildReportBuildPlugin.ACTION_NAME;

			linkFactoryMock.ExpectAndReturn("CreateProjectLink", latestLink, projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME);
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", nextLink, nextBuildSpecifier, "", action);
			linkFactoryMock.ExpectAndReturn("CreateBuildLink", previousLink, previousBuildSpecifier, "", action);

			velocityContext["links"] = links;
			velocityContext["recentBuildsTable"] = recentBuildsView;
			velocityContext["latestLink"] = latestLink;
			velocityContext["nextLink"] = nextLink;
			velocityContext["previousLink"] = previousLink;

			velocityViewGeneratorMock.ExpectAndReturn("GenerateView", velocityResponse, @"BuildSideBar.vm", new HashtableConstraint(velocityContext));

			// Execute
			HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.MockInstance as ICruiseRequest);

			// Verify
			Assert.AreEqual(velocityResponse, returnedResponse);
			VerifyAll();
		}
	}
}
