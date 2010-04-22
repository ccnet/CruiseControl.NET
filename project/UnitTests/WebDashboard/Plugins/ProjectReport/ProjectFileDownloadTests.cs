namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System.IO;
    using NUnit.Framework;
    using Rhino.Mocks;
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
            this.mocks = new MockRepository();
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
            var farmService = this.mocks.StrictMock<IFarmService>();
            var action = new ProjectFileDownload(farmService);
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var request = this.mocks.StrictMock<IRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(request.GetText("file")).Return(fileName);
            SetupResult.For(request.GetText("label")).Return(label);
            SetupResult.For(farmService.RetrieveFileTransfer(projectSpec, label + "\\" + fileName, null)).Return(transfer);

            mocks.ReplayAll();
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
