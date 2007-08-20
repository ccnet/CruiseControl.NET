using NMock;
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
        private IMock mockFarmService;
        private IMock mockRequest;
        private ProjectXmlReport report;
        private IServerSpecifier serverSpecifier;

        [SetUp]
        protected void SetUp()
        {
            mockFarmService = new DynamicMock(typeof (IFarmService));
            mockRequest = new DynamicMock(typeof (ICruiseRequest));
            serverSpecifier = new DefaultServerSpecifier("local");
            mockRequest.SetupResult("ServerSpecifier", serverSpecifier);
            mockRequest.SetupResult("ProjectName", "test");
            report = new ProjectXmlReport((IFarmService)mockFarmService.MockInstance);
        }

        [TearDown]
        protected void TearDown()
        {
            mockFarmService.Verify();
            mockRequest.Verify();
        }

        [Test]
        public void GenerateXmlContentForSpecifiedProject()
        {
            ProjectStatusOnServer status = new ProjectStatusOnServer(ProjectStatusFixture.New("wrong"), serverSpecifier);
            ProjectStatusOnServer status2 = new ProjectStatusOnServer(ProjectStatusFixture.New("test"), serverSpecifier);
            mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions", ProjectStatusList(status, status2));

            IResponse response = report.Execute((ICruiseRequest) mockRequest.MockInstance);

            Assert.IsInstanceOfType(typeof (XmlFragmentResponse), response);
            string xml = ((XmlFragmentResponse) response).ResponseFragment;
            XPathAssert.Matches(XPathAssert.LoadAsDocument(xml), "/CruiseControl/Projects/Project/@name", "test");
        }

        [Test, ExpectedException(typeof (NoSuchProjectException))]
        public void ShouldThrowExceptionIfProjectNameIsInvalid()
        {
            mockFarmService.ExpectAndReturn("GetProjectStatusListAndCaptureExceptions", ProjectStatusList());
            report.Execute((ICruiseRequest)mockRequest.MockInstance);
        }

        private static ProjectStatusListAndExceptions ProjectStatusList(params ProjectStatusOnServer[] statuses)
        {
            return new ProjectStatusListAndExceptions(statuses, new CruiseServerException[0]);
        }
    }
}