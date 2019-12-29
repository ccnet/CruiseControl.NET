namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System.Collections;
    using Moq;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class OhlohProjectPluginTests
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
            var plugin = new OhlohProjectPlugin(null, null);
            Assert.AreEqual("View Ohloh Stats", plugin.LinkDescription);
        }

        [Test]
        public void NamedActionsReturnedImmutableAction()
        {
            var plugin = new OhlohProjectPlugin(null, null);
            Assert.AreEqual(1, plugin.NamedActions.Length);
            Assert.IsInstanceOf<ImmutableNamedAction>(plugin.NamedActions[0]);
            Assert.AreEqual("ViewOhlohProjectStats", plugin.NamedActions[0].ActionName);
            Assert.AreSame(plugin, plugin.NamedActions[0].Action);
        }

        [Test]
        public void ExecuteWorksForNonLinkedSite()
        {
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var viewGenerator = this.mocks.Create<IVelocityViewGenerator>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            Mock.Get(request).SetupGet(_request => _request.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(request).Setup(_request => _request.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(farmService).Setup(_farmService => _farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Returns(string.Empty);

            var plugin = new OhlohProjectPlugin(farmService, viewGenerator);
            var response = plugin.Execute(request);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            Assert.AreEqual("<div>This project has not been linked to a project in Ohloh</div>", actual.ResponseFragment);
        }

        [Test]
        public void ExecuteWorksForLinkedSite()
        {
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var viewGenerator = this.mocks.Create<IVelocityViewGenerator>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            Mock.Get(request).SetupGet(_request => _request.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(request).SetupGet(_request => _request.ProjectName).Returns("Test Project");
            Mock.Get(request).Setup(_request => _request.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(farmService).Setup(_farmService => _farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Returns("1234567");
            Mock.Get(viewGenerator).Setup(_viewGenerator => _viewGenerator.GenerateView(It.IsAny<string>(), It.IsAny<Hashtable>()))
                .Callback<string, Hashtable>((n, ht) => {
                    Assert.AreEqual("OhlohStats.vm", n);
                    Assert.IsNotNull(ht);
                    Assert.IsTrue(ht.ContainsKey("ohloh"));
                    Assert.IsTrue(ht.ContainsKey("projectName"));
                    Assert.AreEqual("1234567", ht["ohloh"]);
                    Assert.AreEqual("Test Project", ht["projectName"]);
                })
                .Returns(new HtmlFragmentResponse("from nVelocity")).Verifiable();

            var plugin = new OhlohProjectPlugin(farmService, viewGenerator);
            var response = plugin.Execute(request);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            Assert.AreEqual("from nVelocity", actual.ResponseFragment);
        }
        #endregion
    }
}
