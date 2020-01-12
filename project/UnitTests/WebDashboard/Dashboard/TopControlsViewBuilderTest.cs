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
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ServerReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Dashboard
{
	// ToDo - actually look at html
	[TestFixture]
	public class TopControlsViewBuilderTest
	{
		private TopControlsViewBuilder viewBuilder;

		private Mock<ICruiseRequest> cruiseRequestMock;
		private Mock<IRequest> requestMock;
		private Mock<ILinkFactory> linkFactoryMock;
		private Mock<IVelocityViewGenerator> velocityViewGeneratorMock;
		private Mock<IFarmService> farmServiceMock;

		private DefaultServerSpecifier serverSpecifier;
		private DefaultProjectSpecifier projectSpecifier;
		private DefaultBuildSpecifier buildSpecifier;
		private Hashtable expectedVelocityContext;
		private HtmlFragmentResponse response;
		private IAbsoluteLink link1;
		private IAbsoluteLink link2;
		private IAbsoluteLink link3;
		private IAbsoluteLink link4;

		[SetUp]
		public void Setup()
		{
			cruiseRequestMock = new Mock<ICruiseRequest>();
			requestMock = new Mock<IRequest>();
			linkFactoryMock = new Mock<ILinkFactory>();
			velocityViewGeneratorMock = new Mock<IVelocityViewGenerator>();
			farmServiceMock = new Mock<IFarmService>();

			viewBuilder = new TopControlsViewBuilder(
				(ICruiseRequest) cruiseRequestMock.Object,
				(ILinkFactory) linkFactoryMock.Object,
				(IVelocityViewGenerator) velocityViewGeneratorMock.Object,
				(IFarmService) farmServiceMock.Object,
				null, null);

			serverSpecifier = new DefaultServerSpecifier("myServer");
			projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "myProject");
			buildSpecifier = new DefaultBuildSpecifier(projectSpecifier, "myBuild");
			expectedVelocityContext = new Hashtable();
			response = new HtmlFragmentResponse("foo");
			link1 = new GeneralAbsoluteLink("1");
			link2 = new GeneralAbsoluteLink("2");
			link3 = new GeneralAbsoluteLink("3");
			link4 = new GeneralAbsoluteLink("4");
		}

		private void VerifyAll()
		{
			cruiseRequestMock.Verify();
			requestMock.Verify();
			linkFactoryMock.Verify();
			velocityViewGeneratorMock.Verify();
			farmServiceMock.Verify();
		}

		[Test]
		public void ShouldGenerateFarmLinkIfNothingSpecified()
		{
			// Setup
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(requestMock.Object).Verifiable();
			requestMock.Setup(_request => _request.GetText("Category")).Returns("").Verifiable();

			expectedVelocityContext["serverName"] = "";
			expectedVelocityContext["categoryName"] = "";
			expectedVelocityContext["projectName"] = "";
			expectedVelocityContext["buildName"] = "";

			linkFactoryMock.Setup(factory => factory.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME)).Returns(link1).Verifiable();
			expectedVelocityContext["farmLink"] = link1;

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"TopMenu.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedVelocityContext)).Returns(response).Verifiable();

			// Execute & Verify
			Assert.AreEqual(response, viewBuilder.Execute());
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateFarmAndServerLinksIfServerButNoProjectSpecified()
		{
			// Setup
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerSpecifier).Returns(serverSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(requestMock.Object).Verifiable();
			requestMock.Setup(_request => _request.GetText("Category")).Returns("").Verifiable();

			expectedVelocityContext["serverName"] = "myServer";
			expectedVelocityContext["categoryName"] = "";
			expectedVelocityContext["projectName"] = "";
			expectedVelocityContext["buildName"] = "";

			linkFactoryMock.Setup(factory => factory.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME)).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier, ServerReportServerPlugin.ACTION_NAME)).Returns(link2).Verifiable();
			expectedVelocityContext["farmLink"] = link1;
			expectedVelocityContext["serverLink"] = link2;

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"TopMenu.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedVelocityContext)).Returns(response).Verifiable();

			// Execute & Verify
			Assert.AreEqual(response, viewBuilder.Execute());
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateFarmServerAndProjectLinksIfServerAndProjectButNoBuildSpecified()
		{
			// Setup
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerSpecifier).Returns(serverSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerSpecifier).Returns(serverSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(requestMock.Object).Verifiable();
			requestMock.Setup(_request => _request.GetText("Category")).Returns("").Verifiable();

            ProjectStatus ps = new ProjectStatus("myProject", "", null, 0, 0, null, DateTime.Now, null, null, DateTime.Now, null, "Queue 1", 1, new List<ParameterBase>());
			ProjectStatusOnServer[] psosa = new ProjectStatusOnServer[] { new ProjectStatusOnServer(ps, serverSpecifier) };
			ProjectStatusListAndExceptions pslae = new ProjectStatusListAndExceptions(psosa, new CruiseServerException[0]);
			farmServiceMock.Setup(service => service.GetProjectStatusListAndCaptureExceptions(serverSpecifier, null)).Returns(pslae).Verifiable();

			expectedVelocityContext["serverName"] = "myServer";
			expectedVelocityContext["categoryName"] = "";
			expectedVelocityContext["projectName"] = "myProject";
			expectedVelocityContext["buildName"] = "";

			linkFactoryMock.Setup(factory => factory.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME)).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier, ServerReportServerPlugin.ACTION_NAME)).Returns(link2).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, ProjectReportProjectPlugin.ACTION_NAME)).Returns(link3).Verifiable();
			expectedVelocityContext["farmLink"] = link1;
			expectedVelocityContext["serverLink"] = link2;
			expectedVelocityContext["projectLink"] = link3;

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"TopMenu.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedVelocityContext)).Returns(response).Verifiable();

			// Execute & Verify
			Assert.AreEqual(response, viewBuilder.Execute());
			VerifyAll();
		}

		[Test]
		public void ShouldGenerateFarmServerProjectAndBuildLinksIfServerProjectAndBuildSpecified()
		{
			// Setup
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerName).Returns("myServer").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns("myProject").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildName).Returns("myBuild").Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerSpecifier).Returns(serverSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ServerSpecifier).Returns(serverSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.BuildSpecifier).Returns(buildSpecifier).Verifiable();
			cruiseRequestMock.SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(requestMock.Object).Verifiable();
			requestMock.Setup(_request => _request.GetText("Category")).Returns("").Verifiable();

            ProjectStatus ps = new ProjectStatus("myProject", "", null, 0, 0, null, DateTime.Now, null, null, DateTime.Now, null, "Queue 1", 1, new List<ParameterBase>());
			ProjectStatusOnServer[] psosa = new ProjectStatusOnServer[] { new ProjectStatusOnServer(ps, serverSpecifier) };
			ProjectStatusListAndExceptions pslae = new ProjectStatusListAndExceptions(psosa, new CruiseServerException[0]);
			farmServiceMock.Setup(service => service.GetProjectStatusListAndCaptureExceptions(serverSpecifier, null)).Returns(pslae).Verifiable();

			expectedVelocityContext["serverName"] = "myServer";
			expectedVelocityContext["categoryName"] = "";
			expectedVelocityContext["projectName"] = "myProject";
			expectedVelocityContext["buildName"] = "myBuild";

			linkFactoryMock.Setup(factory => factory.CreateFarmLink("Dashboard", FarmReportFarmPlugin.ACTION_NAME)).Returns(link1).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateServerLink(serverSpecifier, ServerReportServerPlugin.ACTION_NAME)).Returns(link2).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateProjectLink(projectSpecifier, ProjectReportProjectPlugin.ACTION_NAME)).Returns(link3).Verifiable();
			linkFactoryMock.Setup(factory => factory.CreateBuildLink(buildSpecifier, BuildReportBuildPlugin.ACTION_NAME)).Returns(link4).Verifiable();
			expectedVelocityContext["farmLink"] = link1;
			expectedVelocityContext["serverLink"] = link2;
			expectedVelocityContext["projectLink"] = link3;
			expectedVelocityContext["buildLink"] = link4;

			velocityViewGeneratorMock.Setup(generator => generator.GenerateView(@"TopMenu.vm", It.IsAny<Hashtable>())).
				Callback<string, Hashtable>((name, context) => Assert.AreEqual(context, expectedVelocityContext)).Returns(response).Verifiable();

			// Execute & Verify
			Assert.AreEqual(response, viewBuilder.Execute());
			VerifyAll();
		}
	}
}
