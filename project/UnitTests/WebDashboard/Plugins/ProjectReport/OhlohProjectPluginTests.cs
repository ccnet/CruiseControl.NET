namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System.Collections;
    using NUnit.Framework;
    using Rhino.Mocks;
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
            this.mocks = new MockRepository();
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
            var farmService = this.mocks.StrictMock<IFarmService>();
            var viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            var request = this.mocks.StrictMock<ICruiseRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            SetupResult.For(request.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(request.RetrieveSessionToken()).Return(null);
            SetupResult.For(farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Return(string.Empty);

            this.mocks.ReplayAll();
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
            var farmService = this.mocks.StrictMock<IFarmService>();
            var viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            var request = this.mocks.StrictMock<ICruiseRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            SetupResult.For(request.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(request.ProjectName).Return("Test Project");
            SetupResult.For(request.RetrieveSessionToken()).Return(null);
            SetupResult.For(farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Return("1234567");
            Expect.Call(viewGenerator.GenerateView(null, null))
                .Callback<string, Hashtable>((n, ht) => {
                    Assert.AreEqual("OhlohStats.vm", n);
                    Assert.IsNotNull(ht);
                    Assert.IsTrue(ht.ContainsKey("ohloh"));
                    Assert.IsTrue(ht.ContainsKey("projectName"));
                    Assert.AreEqual("1234567", ht["ohloh"]);
                    Assert.AreEqual("Test Project", ht["projectName"]);
                    return true;
                })
                .Return(new HtmlFragmentResponse("from nVelocity"));

            this.mocks.ReplayAll();
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
