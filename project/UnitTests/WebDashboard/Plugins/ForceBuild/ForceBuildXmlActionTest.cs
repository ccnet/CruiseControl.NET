using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ForceBuild
{
	[TestFixture]
	public class ForceBuildXmlActionTest
	{
		private Mock<IFarmService> mockFarmService;
		private ForceBuildXmlAction reportAction;
		private Mock<ICruiseRequest> cruiseRequestMock;
		private ICruiseRequest cruiseRequest;

		[SetUp]
		public void SetUp()
		{
			mockFarmService = new Mock<IFarmService>();
			reportAction = new ForceBuildXmlAction((IFarmService) mockFarmService.Object);
			cruiseRequestMock = new Mock<ICruiseRequest>();
			cruiseRequest = (ICruiseRequest) cruiseRequestMock.Object;
		}

		public void VerifyAll()
		{
			mockFarmService.Verify();
			cruiseRequestMock.Verify();
		}

		[Test]
		public void ShouldReturnCorrectMessageIfBuildForcedSuccessfully()
		{
			DefaultProjectSpecifier projectSpecifier = new DefaultProjectSpecifier(
				new DefaultServerSpecifier("myServer"), "myProject");
			cruiseRequestMock.SetupGet(request => request.ProjectSpecifier).Returns(projectSpecifier);
			cruiseRequestMock.SetupGet(request => request.ProjectName).Returns("myProject");

            mockFarmService.Setup(service => service.ForceBuild(projectSpecifier, (string)null)).Verifiable();

			IResponse response = reportAction.Execute(cruiseRequest);
			Assert.IsTrue(response is XmlFragmentResponse);
			Assert.AreEqual("<ForceBuildResult>Build Forced for myProject</ForceBuildResult>",
			                ((XmlFragmentResponse) response).ResponseFragment);
		}
	}
}