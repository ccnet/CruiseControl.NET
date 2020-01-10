namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System.Collections;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class ProjectStatusPluginTests
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
        public void DescriptionIsCorrect()
        {
            var plugin = new ProjectStatusPlugin(null, null, null);
            Assert.AreEqual("Project Status", plugin.LinkDescription);
        }

        [Test]
        public void NamedActionsReturnsSingleAction()
        {
            var plugin = new ProjectStatusPlugin(null, null, null);
            var actions = plugin.NamedActions;
            Assert.AreEqual(1, actions.Length);
            Assert.IsInstanceOf<ImmutableNamedAction>(actions[0]);
            Assert.AreEqual("ViewProjectStatus", actions[0].ActionName);
            Assert.AreSame(plugin, actions[0].Action);
        }

        [Test]
        public void ExecuteGeneratesStatusForRoot()
        {
            var url = "/somewhere/action";
            var appPath = "/";
            var projectName = "The Project";
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var viewGenerator = this.mocks.Create<IVelocityViewGenerator>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            var urlBuilder = this.mocks.Create<ICruiseUrlBuilder>(MockBehavior.Strict).Object;
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(request).SetupGet(_request => _request.ApplicationPath).Returns(appPath);
            Mock.Get(projectSpec).SetupGet(_projectSpec => _projectSpec.ProjectName).Returns(projectName);
            Mock.Get(farmService).Setup(_farmService => _farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Returns("1234567");
            Mock.Get(urlBuilder).Setup(_urlBuilder => _urlBuilder.BuildProjectUrl(ProjectStatusAction.ActionName, projectSpec)).Returns(url);
            Mock.Get(viewGenerator).Setup(_viewGenerator => _viewGenerator.GenerateView(It.IsAny<string>(), It.IsAny<Hashtable>()))
                .Callback<string, Hashtable>((n, ht) =>
                {
                    Assert.AreEqual("ProjectStatusReport.vm", n);
                    Assert.IsNotNull(ht);
                    Assert.IsTrue(ht.ContainsKey("dataUrl"));
                    Assert.AreEqual("/somewhere/action?view=json", ht["dataUrl"]);
                    Assert.IsTrue(ht.ContainsKey("projectName"));
                    Assert.AreEqual(projectName, ht["projectName"]);
                    Assert.IsTrue(ht.ContainsKey("applicationPath"));
                    Assert.AreEqual(string.Empty, ht["applicationPath"]);
                })
                .Returns(new HtmlFragmentResponse("from nVelocity")).Verifiable();

            var plugin = new ProjectStatusPlugin(farmService, viewGenerator, urlBuilder);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.Verify();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            Assert.AreEqual("from nVelocity", actual.ResponseFragment);
        }

        [Test]
        public void ExecuteGeneratesStatusForNonRoot()
        {
            var url = "/somewhere/action";
            var appPath = "/ccnet/";
            var projectName = "The Project";
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var viewGenerator = this.mocks.Create<IVelocityViewGenerator>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            var urlBuilder = this.mocks.Create<ICruiseUrlBuilder>(MockBehavior.Strict).Object;
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(request).SetupGet(_request => _request.ApplicationPath).Returns(appPath);
            Mock.Get(projectSpec).SetupGet(_projectSpec => _projectSpec.ProjectName).Returns(projectName);
            Mock.Get(farmService).Setup(_farmService => _farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Returns("1234567");
            Mock.Get(urlBuilder).Setup(_urlBuilder => _urlBuilder.BuildProjectUrl(ProjectStatusAction.ActionName, projectSpec)).Returns(url);
            Mock.Get(viewGenerator).Setup(_viewGenerator => _viewGenerator.GenerateView(It.IsAny<string>(), It.IsAny<Hashtable>()))
                .Callback<string, Hashtable>((n, ht) =>
                {
                    Assert.AreEqual("ProjectStatusReport.vm", n);
                    Assert.IsNotNull(ht);
                    Assert.IsTrue(ht.ContainsKey("dataUrl"));
                    Assert.AreEqual("/somewhere/action?view=json", ht["dataUrl"]);
                    Assert.IsTrue(ht.ContainsKey("projectName"));
                    Assert.AreEqual(projectName, ht["projectName"]);
                    Assert.IsTrue(ht.ContainsKey("applicationPath"));
                    Assert.AreEqual("/ccnet/", ht["applicationPath"]);
                })
                .Returns(new HtmlFragmentResponse("from nVelocity")).Verifiable();

            var plugin = new ProjectStatusPlugin(farmService, viewGenerator, urlBuilder);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.Verify();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            Assert.AreEqual("from nVelocity", actual.ResponseFragment);
        }
        #endregion
    }
}
