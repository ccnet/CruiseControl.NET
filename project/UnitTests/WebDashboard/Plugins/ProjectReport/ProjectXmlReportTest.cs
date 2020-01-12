using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    [TestFixture]
    public class ProjectXmlReportTest
    {
        private Mock<IFarmService> mockFarmService;
        private Mock<ICruiseRequest> mockRequest;
        private ProjectXmlReport report;
        private IServerSpecifier serverSpecifier;

        [SetUp]
        protected void SetUp()
        {
            mockFarmService = new Mock<IFarmService>();
            mockRequest = new Mock<ICruiseRequest>();
            serverSpecifier = new DefaultServerSpecifier("local");
            mockRequest.SetupGet(_request => _request.ServerSpecifier).Returns(serverSpecifier);
            mockRequest.SetupGet(_request => _request.ProjectName).Returns("test");
            report = new ProjectXmlReport((IFarmService)mockFarmService.Object, null);
        }

        [TearDown]
        protected void TearDown()
        {
            mockFarmService.Verify();
            mockRequest.Verify();
        }

        [Test]
        [Ignore("Cannot get the mocking to work properly")]
        public void GenerateXmlContentForSpecifiedProject()
        {
            ProjectStatusOnServer status = new ProjectStatusOnServer(ProjectStatusFixture.New("wrong"), serverSpecifier);
            ProjectStatusOnServer status2 = new ProjectStatusOnServer(ProjectStatusFixture.New("test"), serverSpecifier);
            mockFarmService.Setup(service => service.GetProjectStatusListAndCaptureExceptions(null)).Returns(ProjectStatusList(status, status2)).Verifiable();

            IResponse response = report.Execute((ICruiseRequest) mockRequest.Object);

            Assert.That(response, Is.InstanceOf<XmlFragmentResponse>());
            string xml = ((XmlFragmentResponse) response).ResponseFragment;
            XPathAssert.Matches(XPathAssert.LoadAsDocument(xml), "/CruiseControl/Projects/Project/@name", "test");
        }

        [Test]
        [Ignore("Cannot get the mocking to work properly")]
        public void ShouldThrowExceptionIfProjectNameIsInvalid()
        {
            mockFarmService.Setup(service => service.GetProjectStatusListAndCaptureExceptions(null)).Returns(ProjectStatusList()).Verifiable();
            Assert.That(delegate { report.Execute((ICruiseRequest)mockRequest.Object); },
                        Throws.TypeOf<NoSuchProjectException>());
        }

        private static ProjectStatusListAndExceptions ProjectStatusList(params ProjectStatusOnServer[] statuses)
        {
            return new ProjectStatusListAndExceptions(statuses, new CruiseServerException[0]);
        }
    }
}