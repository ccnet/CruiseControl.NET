namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System.IO;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class ProjectFileDownloadTests
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
        public void ExecuteGeneratesDownload()
        {
            var fileName = "nameOfDaFile";
            var label = "daLabel";
            var stream = new MemoryStream(new byte[0]);
            var transfer = new RemotingFileTransfer(stream);
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var action = new ProjectFileDownload(farmService);
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(request).Setup(_request => _request.GetText("file")).Returns(fileName);
            Mock.Get(request).Setup(_request => _request.GetText("label")).Returns(label);
            Mock.Get(farmService).Setup(_farmService => _farmService.RetrieveFileTransfer(projectSpec, System.IO.Path.Combine(label, fileName), null)).Returns(transfer);

            var response = action.Execute(cruiseRequest);

            mocks.VerifyAll();
            Assert.IsInstanceOf<FileTransferResponse>(response);
            var actual = response as FileTransferResponse;
            Assert.AreEqual(fileName, actual.FileName);
            Assert.AreSame(transfer, actual.FileTransfer);
        }
        #endregion
    }
}
