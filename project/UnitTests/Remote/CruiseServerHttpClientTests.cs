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
        #region Private fields
        private const string xmlFrom11 = @"<Projects CCType=""CCNet"">
<Project name=""CCNet"" category="""" activity=""Sleeping"" lastBuildStatus=""Failure"" lastBuildLabel=""1.5.0.4286"" lastBuildTime=""2009-05-26T21:54:27.00000"" nextBuildTime=""2009-05-26T22:24:04.00000"" webUrl=""http://ccnetlive.thoughtworks.com/ccnet/server/local/project/CCNet/ViewLatestBuildReport.aspx"" CurrentMessage="""" BuildStage="""" serverName=""local""/>
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
            Category = string.Empty,
            Name = "website-validation",
            Activity = ProjectActivity.Sleeping,
            BuildStatus = IntegrationStatus.Failure,
            LastBuildLabel = string.Empty,
            LastBuildDate = new DateTime(2008, 9, 23, 2, 3, 45),
            NextBuildTime = DateTime.MaxValue,
            BuildStage = string.Empty,
            ServerName = string.Empty,
            WebURL = "http://cclive.thoughtworks.com/dashboard/build/detail/website-validation"
        };
        private const string xmlFrom144 = @"<CruiseControl>
<Projects>
<Project name=""CCNet"" category="""" activity=""Sleeping"" status=""Running"" lastBuildStatus=""Failure"" lastBuildLabel=""1.5.0.4286"" lastBuildTime=""2009-05-26T21:54:27.21875"" nextBuildTime=""2009-05-26T22:24:04.90625"" webUrl=""http://ccnetlive.thoughtworks.com/ccnet/server/local/project/CCNet/ViewLatestBuildReport.aspx"" buildStage="""" serverName=""CCNETLIVE""/>
</Projects>
<Queues>
<Queue name=""CCNet"">
<Request projectName=""CCNet"" activity=""Building"" />
</Queue>
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
        private readonly QueueSnapshot queueFrom144 = new QueueSnapshot
        {
            QueueName = "CCNet"
        };
        private readonly QueuedRequestSnapshot requestFrom144 = new QueuedRequestSnapshot
        {
            Activity = ProjectActivity.Building,
            ProjectName = "CCNet"
        };
        private MockRepository mocks = new MockRepository();
        #endregion

        [Test]
        public void GetProjectStatusCorrectlyHandlesRelativePath()
        {
            var webClient = mocks.StrictMock<WebClient>();
            Expect.Call(webClient.DownloadString("http://relative/XmlStatusReport.aspx"))
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
            Expect.Call(webClient.UploadValues("http://relative/ViewFarmReport.aspx", null))
                .IgnoreArguments()
                .Return(new byte[0]);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            mocks.ReplayAll();
            client.ForceBuild("Project1");

            mocks.VerifyAll();
        }

        [Test]
        public void GetProjectStatusCorrectlyHandles1_1Data()
        {
            var webClient = mocks.StrictMock<WebClient>();
            Expect.Call(webClient.DownloadString("http://relative/XmlStatusReport.aspx"))
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
        public void GetProjectStatusCorrectlyHandles1_4_4Data()
        {
            var webClient = mocks.StrictMock<WebClient>();
            Expect.Call(webClient.DownloadString("http://relative/XmlServerReport.aspx"))
                .Return(xmlFrom144);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            mocks.ReplayAll();
            var results = client.GetCruiseServerSnapshot();

            Assert.IsNotNull(results, "Snapshot not parsed");
            Assert.AreEqual(1, results.ProjectStatuses.Length, "Unexpected number of projects returned");
            CompareProjects(projectFrom144, results.ProjectStatuses[0]);
            Assert.AreEqual(1, results.QueueSetSnapshot.Queues.Count, "Unexpected number of queues returned");
            Assert.AreEqual(queueFrom144.QueueName, results.QueueSetSnapshot.Queues[0].QueueName, "Queue name does not match");
            Assert.AreEqual(1, results.QueueSetSnapshot.Queues[0].Requests.Count, "Unexpected number of queue requests returned");
            Assert.AreEqual(requestFrom144.Activity, results.QueueSetSnapshot.Queues[0].Requests[0].Activity, "Queue request activity does not match");
            Assert.AreEqual(requestFrom144.ProjectName, results.QueueSetSnapshot.Queues[0].Requests[0].ProjectName, "Queue request project name does not match");

            mocks.VerifyAll();
        }

        [Test]
        public void GetProjectStatusCorrectlyHandlesCCData()
        {
            var webClient = mocks.StrictMock<WebClient>();
            Expect.Call(webClient.DownloadString("http://relative/XmlStatusReport.aspx"))
                .Return(xmlFromCC);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            mocks.ReplayAll();
            var results = client.GetProjectStatus();

            Assert.IsNotNull(results, "No projects parsed");
            Assert.AreEqual(1, results.Length, "Unexpected number of projects returned");
            CompareProjects(projectFromCC, results[0]);
            mocks.VerifyAll();
        }

        private void CompareProjects(ProjectStatus expected, ProjectStatus actual)
        {
            var dateFormat = "yyyy-MM-ddTHH:mm:ss";     // For some reason .NET will give slightly different results for date/times
            Assert.AreEqual(expected.Name, actual.Name, "Name does not match");
            Assert.AreEqual(expected.Category, actual.Category, "Category does not match");
            Assert.AreEqual(expected.Activity, actual.Activity, "Activity does not match");
            Assert.AreEqual(expected.BuildStatus, actual.BuildStatus, "BuildStatus does not match");
            Assert.AreEqual(expected.LastBuildLabel, actual.LastBuildLabel, "LastBuildLabel does not match");
            Assert.AreEqual(expected.LastBuildDate.ToString(dateFormat), actual.LastBuildDate.ToString(dateFormat), "LastBuildDate does not match");
            Assert.AreEqual(expected.NextBuildTime.ToString(dateFormat), actual.NextBuildTime.ToString(dateFormat), "NextBuildTime does not match");
            Assert.AreEqual(expected.WebURL, actual.WebURL, "WebURL does not match");
            Assert.AreEqual(expected.BuildStage, actual.BuildStage, "BuildStage does not match");
            Assert.AreEqual(expected.ServerName, actual.ServerName, "ServerName does not match");
        }
    }
}
