using System.Collections;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
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

		private Mock<IFarmService> farmServiceMock;
		private Mock<ICruiseRequest> requestMock;
		private Mock<IVelocityViewGenerator> viewGeneratorMock;
		private Mock<ICruiseUrlBuilder> cruiseUrlBuilderMock;

		[SetUp]
		public void Setup()
		{
			requestMock = new Mock<ICruiseRequest>();
			farmServiceMock = new Mock<IFarmService>();
			viewGeneratorMock = new Mock<IVelocityViewGenerator>();
			cruiseUrlBuilderMock = new Mock<ICruiseUrlBuilder>();

			action = new ServerLogServerPlugin((IFarmService) farmServiceMock.Object, (IVelocityViewGenerator) viewGeneratorMock.Object, 
				(ICruiseUrlBuilder) cruiseUrlBuilderMock.Object);
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
			HtmlFragmentResponse response = new HtmlFragmentResponse("foo");

			requestMock.SetupGet(request => request.ServerSpecifier).Returns(serverSpecifier);
			farmServiceMock.Setup(service => service.GetServerLog(serverSpecifier, null)).Returns(serverLog).Verifiable();
			farmServiceMock.Setup(service => service.GetProjectStatusListAndCaptureExceptions(serverSpecifier, null)).Returns(new ProjectStatusListAndExceptions(new ProjectStatusOnServer[0], null)).Verifiable();
			viewGeneratorMock.Setup(generator => generator.GenerateView(@"ServerLog.vm", It.Is<Hashtable>(t => t.Count == 2 && (string)t["log"] == serverLog && t.ContainsKey("projectLinks")))).Returns(response).Verifiable();

			// Execute
			Assert.AreEqual(response, action.Execute((ICruiseRequest) requestMock.Object));

			VerifyAll();
		}

		[Test]
		public void ReturnsServerLogFromRequestedServerForSpecificProject()
		{
			// Setup
			IServerSpecifier serverSpecifier = new DefaultServerSpecifier("foo");
			IProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(serverSpecifier, "foo-project");
			string serverLog = "server log";
			HtmlFragmentResponse response = new HtmlFragmentResponse("foo");

			requestMock.SetupGet(request => request.ServerSpecifier).Returns(serverSpecifier);
			requestMock.SetupGet(request => request.ProjectName).Returns(projectSpecifier.ProjectName);
			requestMock.SetupGet(request => request.ProjectSpecifier).Returns(projectSpecifier);
			farmServiceMock.Setup(service => service.GetServerLog(projectSpecifier, null)).Returns(serverLog).Verifiable();
			farmServiceMock.Setup(service => service.GetProjectStatusListAndCaptureExceptions(serverSpecifier, null)).Returns(new ProjectStatusListAndExceptions(new ProjectStatusOnServer[0], null)).Verifiable();
			viewGeneratorMock.Setup(generator => generator.GenerateView(@"ServerLog.vm",
				It.Is<Hashtable>(t => t.Count == 3 && (string)t["log"] == serverLog && t.ContainsKey("projectLinks") && (string)t["currentProject"] == projectSpecifier.ProjectName))).
				Returns(response).Verifiable();

			// Execute
			Assert.AreEqual(response, action.Execute((ICruiseRequest) requestMock.Object));

			VerifyAll();
		}
	}
}
