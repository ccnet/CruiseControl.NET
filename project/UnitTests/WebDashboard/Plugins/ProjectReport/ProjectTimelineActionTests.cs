namespace ThoughtWorks.CruiseControl.UnitTests.WebDashboard.Plugins.ProjectReport
{
    using System.Collections;
    using Moq;
    using NUnit.Framework;
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
            this.mocks = new MockRepository(MockBehavior.Default);
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
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var viewGenerator = this.mocks.Create<IVelocityViewGenerator>(MockBehavior.Strict).Object;
            var urlBuilder = this.mocks.Create<ICruiseUrlBuilder>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            var build1 = new DefaultBuildSpecifier(projectSpec, "log20100406071725Lbuild.1.xml");
            var build2 = new DefaultBuildSpecifier(projectSpec, "log20100406071725.xml");
            var builds = new IBuildSpecifier[] { build1, build2 };
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(cruiseRequest).Setup(_cruiseRequest => _cruiseRequest.RetrieveSessionToken()).Returns((string)null);
            Mock.Get(request).SetupGet(_request => _request.FileNameWithoutExtension).Returns(ProjectTimelineAction.DataActionName);
            Mock.Get(request).SetupGet(_request => _request.ApplicationPath).Returns(appPath);
            Mock.Get(farmService).Setup(_farmService => _farmService.GetBuildSpecifiers(projectSpec, null)).Returns(builds);
            Mock.Get(urlBuilder).Setup(_urlBuilder => _urlBuilder.BuildBuildUrl(BuildReportBuildPlugin.ACTION_NAME, build1)).Returns(url1);
            Mock.Get(urlBuilder).Setup(_urlBuilder => _urlBuilder.BuildBuildUrl(BuildReportBuildPlugin.ACTION_NAME, build2)).Returns(url2);

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
            var farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            var viewGenerator = this.mocks.Create<IVelocityViewGenerator>(MockBehavior.Strict).Object;
            var urlBuilder = this.mocks.Create<ICruiseUrlBuilder>(MockBehavior.Strict).Object;
            var cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(request).SetupGet(_request => _request.FileNameWithoutExtension).Returns("SomeOtherAction");

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
            farmService = this.mocks.Create<IFarmService>(MockBehavior.Strict).Object;
            viewGenerator = this.mocks.Create<IVelocityViewGenerator>(MockBehavior.Strict).Object;
            urlBuilder = this.mocks.Create<ICruiseUrlBuilder>(MockBehavior.Strict).Object;
            cruiseRequest = this.mocks.Create<ICruiseRequest>(MockBehavior.Strict).Object;
            var request = this.mocks.Create<IRequest>(MockBehavior.Strict).Object;
            var projectSpec = this.mocks.Create<IProjectSpecifier>(MockBehavior.Strict).Object;
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.Request).Returns(request);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectName).Returns(projectName);
            Mock.Get(cruiseRequest).SetupGet(_cruiseRequest => _cruiseRequest.ProjectSpecifier).Returns(projectSpec);
            Mock.Get(request).SetupGet(_request => _request.FileNameWithoutExtension).Returns(ProjectTimelineAction.TimelineActionName);
            Mock.Get(request).SetupGet(_request => _request.ApplicationPath).Returns(appPath);
            Mock.Get(urlBuilder).Setup(_urlBuilder => _urlBuilder.BuildProjectUrl(ProjectTimelineAction.DataActionName, projectSpec)).Returns(url);
            Mock.Get(viewGenerator).Setup(_viewGenerator => _viewGenerator.GenerateView(It.IsAny<string>(), It.IsAny<Hashtable>()))
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
                })
                .Returns(new HtmlFragmentResponse("from nVelocity")).Verifiable();
        }
        #endregion
    }
}
