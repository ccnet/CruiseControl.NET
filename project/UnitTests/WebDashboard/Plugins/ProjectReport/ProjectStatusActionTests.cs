namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class ProjectStatusActionTests
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
        public void ExecuteGeneratesXmlOutputByDefault()
        {
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var snapshot = this.GenerateSnapshot();
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(request).Setup(_request => _request.GetText("view")).Returns((string)null);
            Mock.Get(farmService).Setup(_farmService => _farmService.TakeStatusSnapshot(projectSpec, null)).Returns(snapshot);

            var plugin = new ProjectStatusAction(farmService);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<XmlFragmentResponse>(response);
            var actual = response as XmlFragmentResponse;
            var expected = snapshot.ToString();
            Assert.AreEqual(expected, actual.ResponseFragment);
        }

        [Test]
        public void ExecuteGeneratesXmlOutputForXml()
        {
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var snapshot = this.GenerateSnapshot();
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(request).Setup(_request => _request.GetText("view")).Returns("xml");
            Mock.Get(farmService).Setup(_farmService => _farmService.TakeStatusSnapshot(projectSpec, null)).Returns(snapshot);

            var plugin = new ProjectStatusAction(farmService);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<XmlFragmentResponse>(response);
            var actual = response as XmlFragmentResponse;
            var expected = snapshot.ToString();
            Assert.AreEqual(expected, actual.ResponseFragment);
        }

        [Test]
        public void ExecuteGeneratesJsonOutputForJson()
        {
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var snapshot = this.GenerateSnapshot();
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(request).Setup(_request => _request.GetText("view")).Returns("json");
            Mock.Get(farmService).Setup(_farmService => _farmService.TakeStatusSnapshot(projectSpec, null)).Returns(snapshot);

            var plugin = new ProjectStatusAction(farmService);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<JsonFragmentResponse>(response);
            var actual = response as JsonFragmentResponse;
            var date = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}, {1}, {2}, {3}, {4}, {5}",
                snapshot.TimeOfSnapshot.Year,
                snapshot.TimeOfSnapshot.Month - 1,
                snapshot.TimeOfSnapshot.Day,
                snapshot.TimeOfSnapshot.Hour,
                snapshot.TimeOfSnapshot.Minute,
                snapshot.TimeOfSnapshot.Second);
            var expected = "{time:new Date(" + date + ")," +
                "id:'" + snapshot.Identifier.ToString() + "'," +
                "name:'root'," +
                "status:'CompletedSuccess'" +
                ",description:'Root level'," +
                "started:new Date(2010, 0, 2, 3, 4, 5)," +
                "completed:new Date(2010, 0, 2, 3, 4, 6)," +
                "children:[{id:'" + snapshot.ChildItems[0].Identifier.ToString() + "',name:'child',status:'Cancelled'}]}";
            Assert.AreEqual(expected, actual.ResponseFragment);
        }
        #endregion

        #region Helper methods
        private ProjectStatusSnapshot GenerateSnapshot()
        {
            var snapshot = new ProjectStatusSnapshot
            {
                Description = "Root level",
                Name = "root",
                Status = ItemBuildStatus.CompletedSuccess,
                TimeStarted = new DateTime(2010, 1, 2, 3, 4, 5),
                TimeCompleted = new DateTime(2010, 1, 2, 3, 4, 6)
            };
            snapshot.AddChild(new ItemStatus
            {
                Name = "child",
                Status = ItemBuildStatus.Cancelled
            });
            return snapshot;
        }
        #endregion
    }
}
