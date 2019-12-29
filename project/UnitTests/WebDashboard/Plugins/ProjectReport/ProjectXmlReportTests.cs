namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System;
    using System.Xml;
    using Moq;
    using NUnit.Framework;
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
            this.mocks = new MockRepository(MockBehavior.Default);
        }
        #endregion

        #region Tests
        [Test]
        public void ExecuteGeneratesReport()
        {
            var projectName = "daProject";
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var sessionRetriever = this.mocks.Create<ISessionRetriever>(MockBehavior.Strict).Object;
            var server = this.mocks.Create<IServerSpecifier>(MockBehavior.Strict).Object;
            var project = new ProjectStatus(projectName, IntegrationStatus.Success, new DateTime(2010, 1, 2, 3, 4, 5));
            project.ServerName = "TESTMACHINE";
            var status = new ProjectStatusOnServer(project, server);
            var snapshot = new ProjectStatusListAndExceptions(
                new ProjectStatusOnServer[] { status },
                new CruiseServerException[0]);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns(projectName);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ServerSpecifier).Returns(server);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken(sessionRetriever)).Returns((string)null);
            Mock.Get(farmService).Setup(_farmService => _farmService.GetProjectStatusListAndCaptureExceptions(server, null))
                .Returns(snapshot);

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
