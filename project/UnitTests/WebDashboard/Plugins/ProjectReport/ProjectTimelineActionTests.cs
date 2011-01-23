namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System.Collections;
    using NUnit.Framework;
    using Rhino.Mocks;
    using ThoughtWorks.CruiseControl.Core;
    using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
    using ThoughtWorks.CruiseControl.WebDashboard.IO;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC;
    using ThoughtWorks.CruiseControl.WebDashboard.MVC.View;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.BuildReport;
    using ThoughtWorks.CruiseControl.WebDashboard.Plugins.ProjectReport;
    using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

    public class ProjectTimelineActionTests
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
        public void ExecuteWorksForTimeLineRequest()
        {
            IFarmService farmService;
            IVelocityViewGenerator viewGenerator;
            ICruiseUrlBuilder urlBuilder;
            ICruiseRequest cruiseRequest;
            GenerateTimelineMocks("/ccnet/", "/ccnet/", out farmService, out viewGenerator, out urlBuilder, out cruiseRequest);

            this.mocks.ReplayAll();
            var plugin = new ProjectTimelineAction(viewGenerator, farmService, urlBuilder);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            Assert.AreEqual("from nVelocity", actual.ResponseFragment);
        }

        [Test]
        public void ExecuteWorksForRootTimeLineRequest()
        {
            IFarmService farmService;
            IVelocityViewGenerator viewGenerator;
            ICruiseUrlBuilder urlBuilder;
            ICruiseRequest cruiseRequest;
            GenerateTimelineMocks("/", string.Empty, out farmService, out viewGenerator, out urlBuilder, out cruiseRequest);

            this.mocks.ReplayAll();
            var plugin = new ProjectTimelineAction(viewGenerator, farmService, urlBuilder);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<HtmlFragmentResponse>(response);
            var actual = response as HtmlFragmentResponse;
            Assert.AreEqual("from nVelocity", actual.ResponseFragment);
        }

        [Test]
        public void ExecuteWorksForDataRequest()
        {
            var url1 = "build/1";
            var url2 = "build/2";
            var appPath = "/";
            var farmService = this.mocks.StrictMock<IFarmService>();
            var viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            var urlBuilder = this.mocks.StrictMock<ICruiseUrlBuilder>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var request = this.mocks.StrictMock<IRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            var build1 = new DefaultBuildSpecifier(projectSpec, "log20100406071725Lbuild.1.xml");
            var build2 = new DefaultBuildSpecifier(projectSpec, "log20100406071725.xml");
            var builds = new IBuildSpecifier[] { build1, build2 };
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(cruiseRequest.RetrieveSessionToken()).Return(null);
            SetupResult.For(request.FileNameWithoutExtension).Return(ProjectTimelineAction.DataActionName);
            SetupResult.For(request.ApplicationPath).Return(appPath);
            SetupResult.For(farmService.GetBuildSpecifiers(projectSpec, null)).Return(builds);
            SetupResult.For(urlBuilder.BuildBuildUrl(BuildReportBuildPlugin.ACTION_NAME, build1)).Return(url1);
            SetupResult.For(urlBuilder.BuildBuildUrl(BuildReportBuildPlugin.ACTION_NAME, build2)).Return(url2);

            this.mocks.ReplayAll();
            var plugin = new ProjectTimelineAction(viewGenerator, farmService, urlBuilder);
            var response = plugin.Execute(cruiseRequest);

            this.mocks.VerifyAll();
            Assert.IsInstanceOf<XmlFragmentResponse>(response);
            var actual = response as XmlFragmentResponse;
            var expected = "<data>" +
                "<event start=\"Tue, 06 Apr 2010 07:17:25 GMT\" title=\"Success (1)\" color=\"green\" icon=\"/javascript/Timeline/images/dark-green-circle.png\">&lt;a href=\"build/1\"&gt;View Build&lt;/a&gt;</event>" +
                "<event start=\"Tue, 06 Apr 2010 07:17:25 GMT\" title=\"Failure\" color=\"red\" icon=\"/javascript/Timeline/images/dark-red-circle.png\">&lt;a href=\"build/2\"&gt;View Build&lt;/a&gt;</event>" +
                "</data>";
            Assert.AreEqual(expected, actual.ResponseFragment);
        }

        [Test]
        public void ExecuteFailsForUnknownRequest()
        {
            var farmService = this.mocks.StrictMock<IFarmService>();
            var viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            var urlBuilder = this.mocks.StrictMock<ICruiseUrlBuilder>();
            var cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var request = this.mocks.StrictMock<IRequest>();
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(request.FileNameWithoutExtension).Return("SomeOtherAction");

            this.mocks.ReplayAll();
            var plugin = new ProjectTimelineAction(viewGenerator, farmService, urlBuilder);
            var error = Assert.Throws<CruiseControlException>(() =>
            {
                var response = plugin.Execute(cruiseRequest);
            });

            this.mocks.VerifyAll();
            Assert.AreEqual("Unknown action: SomeOtherAction", error.Message);
        }
        #endregion

        #region Helper methods
        private void GenerateTimelineMocks(string appPath, string expected, out IFarmService farmService, out IVelocityViewGenerator viewGenerator, out ICruiseUrlBuilder urlBuilder, out ICruiseRequest cruiseRequest)
        {
            var url = "/somewhere.aspx";
            var projectName = "Test Project";
            farmService = this.mocks.StrictMock<IFarmService>();
            viewGenerator = this.mocks.StrictMock<IVelocityViewGenerator>();
            urlBuilder = this.mocks.StrictMock<ICruiseUrlBuilder>();
            cruiseRequest = this.mocks.StrictMock<ICruiseRequest>();
            var request = this.mocks.StrictMock<IRequest>();
            var projectSpec = this.mocks.StrictMock<IProjectSpecifier>();
            SetupResult.For(cruiseRequest.Request).Return(request);
            SetupResult.For(cruiseRequest.ProjectName).Return(projectName);
            SetupResult.For(cruiseRequest.ProjectSpecifier).Return(projectSpec);
            SetupResult.For(request.FileNameWithoutExtension).Return(ProjectTimelineAction.TimelineActionName);
            SetupResult.For(request.ApplicationPath).Return(appPath);
            SetupResult.For(urlBuilder.BuildProjectUrl(ProjectTimelineAction.DataActionName, projectSpec)).Return(url);
            Expect.Call(viewGenerator.GenerateView(null, null))
                .Callback<string, Hashtable>((n, ht) =>
                {
                    Assert.AreEqual("ProjectTimeline.vm", n);
                    Assert.IsNotNull(ht);
                    Assert.IsTrue(ht.ContainsKey("applicationPath"));
                    Assert.IsTrue(ht.ContainsKey("projectName"));
                    Assert.IsTrue(ht.ContainsKey("dataUrl"));
                    Assert.AreEqual(expected, ht["applicationPath"]);
                    Assert.AreEqual(projectName, ht["projectName"]);
                    Assert.AreEqual(url, ht["dataUrl"]);
                    return true;
                })
                .Return(new HtmlFragmentResponse("from nVelocity"));
        }
        #endregion
    }
}
