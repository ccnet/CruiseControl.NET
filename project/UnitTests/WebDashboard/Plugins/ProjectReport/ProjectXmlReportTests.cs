namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System;
    using System.Xml;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class ProjectXmlReportTests
    {
        #region Private fields
        private MockRepository mocks;
        #endregion

        #region Setup
        [SetUp]
        public void Setup()
        {
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        [Test]
        public void ExecuteGeneratesReport()
        {
            var projectName = "daProject";
            var farmService = this.mocks.StrictMock<IFarmService>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var sessionRetriever = this.mocks.StrictMock<ISessionRetriever>();
            var server = this.mocks.StrictMock<IServerSpecifier>();
            var project = new ProjectStatus(projectName, IntegrationStatus.Success, new DateTime(2010, 1, 2, 3, 4, 5));
            project.ServerName = "TESTMACHINE";
            var status = new ProjectStatusOnServer(project, server);
            var snapshot = new ProjectStatusListAndExceptions(
                new ProjectStatusOnServer[] { status },
                new CruiseServerException[0]);
            SetupResult.For(cruiseRequest.ProjectName).Return(projectName);
            SetupResult.For(cruiseRequest.ServerSpecifier).Return(server);
            SetupResult.For(cruiseRequest.RetrieveSessionToken(sessionRetriever)).Return(null);
            SetupResult.For(farmService.GetProjectStatusListAndCaptureExceptions(server, null))
                .Return(snapshot);

            this.mocks.ReplayAll();
            var report = new ProjectXmlReport(farmService, sessionRetriever);
            var response = report.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<XmlFragmentResponse>(response);
            var actual = response as XmlFragmentResponse;
            var expected = "<CruiseControl>" +
                "<Projects>" +
                "<Project name=\"daProject\" category=\"\" activity=\"Sleeping\" " +
                    "status=\"Running\" lastBuildStatus=\"Success\" lastBuildLabel=\"\" " +
                    "lastBuildTime=\"" + XmlConvert.ToString(project.LastBuildDate, XmlDateTimeSerializationMode.Local) + 
                    "\" nextBuildTime=\"" + XmlConvert.ToString(project.NextBuildTime, XmlDateTimeSerializationMode.Local) + "\" " +
                    "webUrl=\"\" buildStage=\"\" serverName=\"TESTMACHINE\" />" +
                "</Projects>" +
                "<Queues />" +
                "</CruiseControl>";
            Assert.AreEqual(expected, actual.ResponseFragment);
        }
        #endregion
    }
}
