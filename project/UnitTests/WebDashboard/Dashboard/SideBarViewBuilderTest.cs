using System;
using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;
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
		private Mock<ICruiseRequest> cruiseRequestWrapperMock;
		private Mock<IBuildNameRetriever> buildNameRetrieverMock;
		private Mock<IRecentBuildsViewBuilder> recentBuildsViewBuilderMock;
		private Mock<IPluginLinkCalculator> pluginLinkCalculatorMock;
		private Mock<IVelocityViewGenerator> velocityViewGeneratorMock;
		private Mock<ILinkFactory> linkFactoryMock;
		private Mock<ILinkListFactory> linkListFactoryMock;
		private Mock<IFarmService> farmServiceMock;

		private SideBarViewBuilder sideBarViewBuilder;

		private HtmlFragmentResponse velocityResponse;
		private Hashtable velocityContext;
		private IAbsoluteLink[] links;
		private IAbsoluteLink[] serverLinks;
		private IServerSpecifier[] serverSpecifiers;


		[SetUp]
		public void Setup()
		{
			cruiseRequestWrapperMock = new Mock<ICruiseRequest>();
			buildNameRetrieverMock = new Mock<IBuildNameRetriever>();
			recentBuildsViewBuilderMock = new Mock<IRecentBuildsViewBuilder>();
			pluginLinkCalculatorMock = new Mock<IPluginLinkCalculator>();
			velocityViewGeneratorMock = new Mock<IVelocityViewGenerator>();
			linkFactoryMock = new Mock<ILinkFactory>();
			linkListFactoryMock = new Mock<ILinkListFactory>();
			farmServiceMock = new Mock<IFarmService>();


			sideBarViewBuilder = new SideBarViewBuilder(
				(ICruiseRequest) cruiseRequestWrapperMock.Object,
				(IBuildNameRetriever) buildNameRetrieverMock.Object,
				(IRecentBuildsViewBuilder) recentBuildsViewBuilderMock.Object,
				(IPluginLinkCalculator) pluginLinkCalculatorMock.Object,
				(IVelocityViewGenerator) velocityViewGeneratorMock.Object,
				(ILinkFactory) linkFactoryMock.Object,
				(ILinkListFactory)linkListFactoryMock.Object,
				(IFarmService) farmServiceMock.Object,
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
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("").Verifiable();
			pluginLinkCalculatorMock.Setup(calculator => calculator.GetFarmPluginLinks()).Returns(links).Verifiable();
			farmServiceMock.Setup(service => service.GetServerSpecifiers()).Returns(serverSpecifiers).Verifiable();
			linkListFactoryMock.Setup(factory => factory.CreateServerLinkList(serverSpecifiers, "ViewServerReport")).Returns(serverLinks).Verifiable();

            ProjectStatus ps = new ProjectStatus("", "", null, 0, 0, null, DateTime.Now, null, null, DateTime.Now, null, "", 0, new List<ParameterBase>());
            ProjectStatusOnServer[] psosa = new ProjectStatusOnServer[] { new ProjectStatusOnServer(ps, serverSpecifiers[0]) };
            ProjectStatusListAndExceptions pslae = new ProjectStatusListAndExceptions(psosa, new CruiseServerException[0]);
            farmServiceMock.Setup(service => service.GetProjectStatusListAndCaptureExceptions(serverSpecifiers[0], null)).Returns(pslae).Verifiable();

			velocityContext["links"] = links;
			velocityContext["serverlinks"] = serverLinks;

            velocityContext["showCategories"] = false;
            velocityContext["categorylinks"] = null;
            CruiseControl.WebDashboard.Dashboard.DefaultLinkFactory x = new DefaultLinkFactory(new DefaultUrlBuilder(),null,null);

            IAbsoluteLink farmLink = x.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME);
            linkFactoryMock.Setup(factory => factory.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME)).Returns(farmLink).Verifiable();
            velocityContext["farmLink"] = farmLink;

            System.Diagnostics.Debug.WriteLine("starting");

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"FarmSideBar.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, velocityContext)).Returns(velocityResponse).Verifiable();

			// Execute
			HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.Object as ICruiseRequest);

			// Verify
			Assert.AreEqual(velocityResponse, returnedResponse);
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateServerViewIfServerButNoProjectSpecified()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("myServer");
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerSpecifier).Returns(serverSpecifier).Verifiable();

			pluginLinkCalculatorMock.Setup(calculator => calculator.GetServerPluginLinks(serverSpecifier)).Returns(links).Verifiable();

            ProjectStatus ps = new ProjectStatus("", "myCategory", null, 0, 0, null, DateTime.Now, null, null, DateTime.Now, null, "", 0, new List<ParameterBase>());
			ProjectStatusOnServer[] psosa = new ProjectStatusOnServer[] { new ProjectStatusOnServer(ps, serverSpecifier) };
			ProjectStatusListAndExceptions pslae = new ProjectStatusListAndExceptions(psosa, new CruiseServerException[0]);
			farmServiceMock.Setup(service => service.GetProjectStatusListAndCaptureExceptions(serverSpecifier, null)).Returns(pslae).Verifiable();

			IAbsoluteLink link = new GeneralAbsoluteLink("link");
			IAbsoluteLink[] categoryLinks = new GeneralAbsoluteLink[] { new GeneralAbsoluteLink("myCategory", "?Category=myCategory") };
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier, ServerReportServerPlugin.ACTION_NAME)).Returns(link).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier, ServerReportServerPlugin.ACTION_NAME)).Returns(link).Verifiable();

			velocityContext["links"] = links;
			velocityContext["serverlink"] = link;
			velocityContext["showCategories"] = true;
			velocityContext["categorylinks"] = categoryLinks;

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"ServerSideBar.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, velocityContext)).Returns(velocityResponse).Verifiable();

			// Execute
			HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.Object as ICruiseRequest);

			// Verify
			Assert.AreEqual(velocityResponse, returnedResponse);
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateProjectViewIfServerAndProjectButNoBuildSpecified()
		{
			// Setup
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpecifier).Verifiable();

			pluginLinkCalculatorMock.Setup(calculator => calculator.GetProjectPluginLinks(projectSpecifier)).Returns(links).Verifiable();
			string recentBuildsView = "";
			recentBuildsViewBuilderMock.Setup(builder => builder.BuildRecentBuildsTable(projectSpecifier, null)).Returns(recentBuildsView).Verifiable();

			velocityContext["links"] = links;
			velocityContext["recentBuildsTable"] = recentBuildsView;

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"ProjectSideBar.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, velocityContext)).Returns(velocityResponse).Verifiable();

			// Execute
			HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.Object as ICruiseRequest);

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
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("myBuild").Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildSpecifier).Returns(buildSpecifier).Verifiable();
			cruiseRequestWrapperMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpecifier).Verifiable();

			pluginLinkCalculatorMock.Setup(calculator => calculator.GetBuildPluginLinks(buildSpecifier)).Returns(links).Verifiable();
			string recentBuildsView = "";
			recentBuildsViewBuilderMock.Setup(builder => builder.BuildRecentBuildsTable(buildSpecifier, null)).Returns(recentBuildsView).Verifiable();

			IBuildSpecifier nextBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "next");
			IBuildSpecifier previousBuildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "previous");
			IAbsoluteLink latestLink = new GeneralAbsoluteLink("test latest");
			IAbsoluteLink nextLink = new GeneralAbsoluteLink("test next");
			IAbsoluteLink previousLink = new GeneralAbsoluteLink("test previous");

			buildNameRetrieverMock.Setup(retriever => retriever.GetNextBuildSpecifier(buildSpecifier, null)).Returns(nextBuildSpecifier).Verifiable();
			buildNameRetrieverMock.Setup(retriever => retriever.GetPreviousBuildSpecifier(buildSpecifier, null)).Returns(previousBuildSpecifier).Verifiable();

			string action = BuildReportBuildPlugin.ACTION_NAME;

			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, LatestBuildReportProjectPlugin.ACTION_NAME)).Returns(latestLink).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateBuildLink(nextBuildSpecifier, "", action)).Returns(nextLink).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateBuildLink(previousBuildSpecifier, "", action)).Returns(previousLink).Verifiable();

			velocityContext["links"] = links;
			velocityContext["recentBuildsTable"] = recentBuildsView;
			velocityContext["latestLink"] = latestLink;
			velocityContext["nextLink"] = nextLink;
			velocityContext["previousLink"] = previousLink;

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"BuildSideBar.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, velocityContext)).Returns(velocityResponse).Verifiable();

			// Execute
			HtmlFragmentResponse returnedResponse = sideBarViewBuilder.Execute(cruiseRequestWrapperMock.Object as ICruiseRequest);

			// Verify
			Assert.AreEqual(velocityResponse, returnedResponse);
			VerifyAll();
		}
	}
}
