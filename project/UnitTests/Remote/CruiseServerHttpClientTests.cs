using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;
using Rhino.Mocks;
using System.Net;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class CruiseServerHttpClientTests
    {
        private const string xmlFrom11 = @"<Projects CCType=""CCNet"">
<Project name=""CCNet"" category="""" activity=""Sleeping"" lastBuildStatus=""Failure"" lastBuildLabel=""1.5.0.4286"" lastBuildTime=""2009-05-26T21:54:27.00000-00:00"" nextBuildTime=""2009-05-26T22:24:04.00000-00:00"" webUrl=""http://ccnetlive.thoughtworks.com/ccnet/server/local/project/CCNet/ViewLatestBuildReport.aspx"" CurrentMessage="""" BuildStage="""" serverName=""local""/>
</Projects>";
        private readonly ProjectStatus projectFrom11 = new ProjectStatus
        {
            Name = "CCNet",
            Category = string.Empty,
            Activity = ProjectActivity.Sleeping,
            BuildStatus = IntegrationStatus.Failure,
            LastBuildLabel = "1.5.0.4286",
            LastBuildDate = new DateTime(2009, 5, 26, 21, 54, 27),
            NextBuildTime = new DateTime(2009, 5, 26, 22, 24, 04),
            WebURL = "http://ccnetlive.thoughtworks.com/ccnet/server/local/project/CCNet/ViewLatestBuildReport.aspx",
            BuildStage = string.Empty,
            ServerName = "local"
        };
        private const string xmlFromCC = @"<Projects>
<Project name=""website-validation"" activity=""Sleeping"" lastBuildStatus=""Failure"" lastBuildLabel="""" lastBuildTime=""2008-09-23T02:03:45"" webUrl=""http://cclive.thoughtworks.com/dashboard/build/detail/website-validation""/>
</Projects>";
        private readonly ProjectStatus projectFromCC = new ProjectStatus
        {
            Name = "website-validation",
            Activity = ProjectActivity.Sleeping,
            BuildStatus = IntegrationStatus.Failure,
            LastBuildLabel = string.Empty,
            LastBuildDate = new DateTime(2008, 9, 23, 2, 3, 45),
            WebURL = "http://cclive.thoughtworks.com/dashboard/build/detail/website-validation"
        };
        private const string xmlFrom144 = @"<CruiseControl>
<Projects>
<Project name=""CCNet"" category="""" activity=""Sleeping"" status=""Running"" lastBuildStatus=""Failure"" lastBuildLabel=""1.5.0.4286"" lastBuildTime=""2009-05-26T21:54:27.21875-05:00"" nextBuildTime=""2009-05-26T22:24:04.90625-05:00"" webUrl=""http://ccnetlive.thoughtworks.com/ccnet/server/local/project/CCNet/ViewLatestBuildReport.aspx"" buildStage="""" serverName=""CCNETLIVE""/>
</Projects>
<Queues>
<Queue name=""CCNet""/>
</Queues>
</CruiseControl>";
        private readonly ProjectStatus projectFrom144 = new ProjectStatus
        {
            Name = "CCNet",
            Category = string.Empty,
            Activity = ProjectActivity.Sleeping,
            BuildStatus = IntegrationStatus.Failure,
            LastBuildLabel = "1.5.0.4286",
            LastBuildDate = new DateTime(2009, 5, 26, 21, 54, 27),
            NextBuildTime = new DateTime(2009, 5, 26, 22, 24, 04),
            WebURL = "http://ccnetlive.thoughtworks.com/ccnet/server/local/project/CCNet/ViewLatestBuildReport.aspx",
            BuildStage = string.Empty,
            ServerName = "CCNETLIVE"
        };
        private MockRepository mocks = new MockRepository();

        [Test]
        public void GetProjectStatusCorrectlyHandlesRelativePath()
        {
            var webClient = mocks.StrictMock<WebClient>();
            Expect.Call(webClient.DownloadString("http://relative/xmlstatusreport.aspx"))
                .Return(xmlFrom11);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            mocks.ReplayAll();
            var results = client.GetProjectStatus();

            Assert.IsNotNull(results, "No projects parsed");
            Assert.AreEqual(1, results.Length, "Unexpected number of projects returned");
            CompareProjects(projectFrom11, results[0]);
            mocks.VerifyAll();
        }

        [Test]
        public void ForceBuildCorrectlyHandlesRelativePath()
        {
            var webClient = mocks.StrictMock<WebClient>();
            Expect.Call(webClient.UploadValues("http://relative/xmlstatusreport.aspx", null))
                .IgnoreArguments();
            var client = new CruiseServerHttpClient("http://relative", webClient);

            mocks.ReplayAll();
            client.ForceBuild("Project1");

            mocks.VerifyAll();
        }

        private void CompareProjects(ProjectStatus expected, ProjectStatus actual)
        {
            Assert.AreEqual(expected.Name, actual.Name, "Name does not match");
            Assert.AreEqual(expected.Category, actual.Category, "Category does not match");
            Assert.AreEqual(expected.Activity, actual.Activity, "Activity does not match");
            Assert.AreEqual(expected.BuildStatus, actual.BuildStatus, "BuildStatus does not match");
            Assert.AreEqual(expected.LastBuildLabel, actual.LastBuildLabel, "LastBuildLabel does not match");
            Assert.AreEqual(expected.LastBuildDate, actual.LastBuildDate, "LastBuildDate does not match");
            Assert.AreEqual(expected.NextBuildTime, actual.NextBuildTime, "NextBuildTime does not match");
            Assert.AreEqual(expected.WebURL, actual.WebURL, "WebURL does not match");
            Assert.AreEqual(expected.BuildStage, actual.BuildStage, "BuildStage does not match");
            Assert.AreEqual(expected.ServerName, actual.ServerName, "ServerName does not match");
        }
    }
}
