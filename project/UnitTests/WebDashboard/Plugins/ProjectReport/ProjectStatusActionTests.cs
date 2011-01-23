namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System;
    using NUnit.Framework;
    using Rhino.Mocks;
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
            this.mocks = new MockRepository();
        }
        #endregion

        #region Tests
        [Test]
        public void ExecuteGeneratesXmlOutputByDefault()
        {
            var farmService = this.mocks.StrictMock<IFarmService>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            var request = this.mocks.StrictMock<IRequest>();
            var snapshot = this.GenerateSnapshot();
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(request.GetText("view")).Return(null);
            SetupResult.For(farmService.TakeStatusSnapshot(projectSpec, null)).Return(snapshot);

            this.mocks.ReplayAll();
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
            var farmService = this.mocks.StrictMock<IFarmService>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            var request = this.mocks.StrictMock<IRequest>();
            var snapshot = this.GenerateSnapshot();
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(request.GetText("view")).Return("xml");
            SetupResult.For(farmService.TakeStatusSnapshot(projectSpec, null)).Return(snapshot);

            this.mocks.ReplayAll();
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
            var farmService = this.mocks.StrictMock<IFarmService>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            var request = this.mocks.StrictMock<IRequest>();
            var snapshot = this.GenerateSnapshot();
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(request.GetText("view")).Return("json");
            SetupResult.For(farmService.TakeStatusSnapshot(projectSpec, null)).Return(snapshot);

            this.mocks.ReplayAll();
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
