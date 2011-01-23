namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using System.Collections;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;

    public class ProjectStatusPluginTests
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
            var farmService = this.mocks.StrictMock<IFarmService>();
            var viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var request = this.mocks.StrictMock<IRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            var urlBuilder = this.mocks.StrictMock<ICruiseUrlBuilder>();
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(request.ApplicationPath).Return(appPath);
            SetupResult.For(projectSpec.ProjectName).Return(projectName);
            SetupResult.For(farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Return("1234567");
            SetupResult.For(urlBuilder.BuildProjectUrl(ProjectStatusAction.ActionName, projectSpec)).Return(url);
            Expect.Call(viewGenerator.GenerateView(null, null))
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
                    return true;
                })
                .Return(new HtmlFragmentResponse("from nVelocity"));

            this.mocks.ReplayAll();
            var plugin = new ProjectStatusPlugin(farmService, viewGenerator, urlBuilder);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
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
            var farmService = this.mocks.StrictMock<IFarmService>();
            var viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var request = this.mocks.StrictMock<IRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            var urlBuilder = this.mocks.StrictMock<ICruiseUrlBuilder>();
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(request.ApplicationPath).Return(appPath);
            SetupResult.For(projectSpec.ProjectName).Return(projectName);
            SetupResult.For(farmService.GetLinkedSiteId(projectSpec, null, "ohloh")).Return("1234567");
            SetupResult.For(urlBuilder.BuildProjectUrl(ProjectStatusAction.ActionName, projectSpec)).Return(url);
            Expect.Call(viewGenerator.GenerateView(null, null))
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
                    return true;
                })
                .Return(new HtmlFragmentResponse("from nVelocity"));

            this.mocks.ReplayAll();
            var plugin = new ProjectStatusPlugin(farmService, viewGenerator, urlBuilder);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            Assert.AreEqual("from nVelocity", actual.ResponseFragment);
        }
        #endregion
    }
}
