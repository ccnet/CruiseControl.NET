using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote;

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
        private MockRepository mocks = new MockRepository(MockBehavior.Default);
        #endregion

        [Test]
        public void GetCruiseServerSnapshotSetsCredentialsOnWebClient()
        {
            var url = "http://test1:test2@test3";
            var webClient = mocks.Create<WebClient>().Object;
            SetupWebClient(webClient, url, xmlFrom11);
            var client = new CruiseServerHttpClient("http://test1:test2@test3", webClient);

            client.GetCruiseServerSnapshot();

            Assert.IsNotNull(webClient.Credentials, "No credentials set");
            var cred = webClient.Credentials.GetCredential(new Uri(url), "Basic");
            Assert.AreEqual("test1", cred.UserName, "Unexpected username");
            Assert.AreEqual("test2", cred.Password, "Unexpected password");
        }

        [Test]
        public void GetProjectStatusSetsCredentialsOnWebClient()
        {
            var url = "http://test1:test2@test3";
            var webClient = mocks.Create<WebClient>().Object;
            SetupWebClient(webClient, url, xmlFrom11);
            var client = new CruiseServerHttpClient("http://test1:test2@test3", webClient);

            client.GetProjectStatus();

            Assert.IsNotNull(webClient.Credentials, "No credentials set");
            var cred = webClient.Credentials.GetCredential(new Uri(url), "Basic");
            Assert.AreEqual("test1", cred.UserName, "Unexpected username");
            Assert.AreEqual("test2", cred.Password, "Unexpected password");
        }

        [Test]
        public void StartProjectSetsCredentialsOnWebClient()
        {
            var url = "http://test1:test2@test3";
            var webClient = mocks.Create<WebClient>().Object;
            SetupWebClient(webClient, url);
            var client = new CruiseServerHttpClient("http://test1:test2@test3", webClient);

            client.StartProject(null);

            Assert.IsNotNull(webClient.Credentials, "No credentials set");
            var cred = webClient.Credentials.GetCredential(new Uri(url), "Basic");
            Assert.AreEqual("test1", cred.UserName, "Unexpected username");
            Assert.AreEqual("test2", cred.Password, "Unexpected password");
        }

        [Test]
        public void GetProjectStatusForcesAuthorizationIf403ForbiddenIsReceived()
        {
            var url = "http://test1:test2@test3";
            var webClient = mocks.Create<WebClient>().Object;
            SetupWebClient(webClient, url, xmlFrom11, true);
            webClient.Headers = new WebHeaderCollection();
            var client = new CruiseServerHttpClient("http://test1:test2@test3", webClient);

            client.GetProjectStatus();

            Assert.AreEqual("Basic dGVzdDE6dGVzdDI=", webClient.Headers["Authorization"], "Unexpected Authorization header");
        }

        [Test]
        public void GetProjectStatusCorrectlyHandlesRelativePath()
        {
            var webClient = mocks.Create<WebClient>(MockBehavior.Strict).Object;
            SetupWebClient(webClient, "http://relative/XmlStatusReport.aspx", xmlFrom11);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            var results = client.GetProjectStatus();

            Assert.IsNotNull(results, "No projects parsed");
            Assert.AreEqual(1, results.Length, "Unexpected number of projects returned");
            CompareProjects(projectFrom11, results[0]);
            mocks.Verify();
        }

        [Test]
        public void ForceBuildCorrectlyHandlesRelativePath()
        {
            var webClient = mocks.Create<WebClient>(MockBehavior.Strict).Object;
            SetupWebClient(webClient, "http://relative/ViewFarmReport.aspx");
            var client = new CruiseServerHttpClient("http://relative", webClient);

            client.ForceBuild("Project1");

            mocks.VerifyAll();
        }

        [Test]
        public void GetProjectStatusCorrectlyHandles1_1Data()
        {
            var webClient = mocks.Create<WebClient>(MockBehavior.Strict).Object;
            SetupWebClient(webClient, "http://relative/XmlStatusReport.aspx", xmlFrom11);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            var results = client.GetProjectStatus();

            Assert.IsNotNull(results, "No projects parsed");
            Assert.AreEqual(1, results.Length, "Unexpected number of projects returned");
            CompareProjects(projectFrom11, results[0]);
            mocks.Verify();
        }

        [Test]
        public void GetProjectStatusCorrectlyHandles1_4_4Data()
        {
            var webClient = mocks.Create<WebClient>(MockBehavior.Strict).Object;
            SetupWebClient(webClient, "http://relative/XmlServerReport.aspx", xmlFrom144);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            var results = client.GetCruiseServerSnapshot();

            Assert.IsNotNull(results, "Snapshot not parsed");
            Assert.AreEqual(1, results.ProjectStatuses.Length, "Unexpected number of projects returned");
            CompareProjects(projectFrom144, results.ProjectStatuses[0]);
            Assert.AreEqual(1, results.QueueSetSnapshot.Queues.Count, "Unexpected number of queues returned");
            Assert.AreEqual(queueFrom144.QueueName, results.QueueSetSnapshot.Queues[0].QueueName, "Queue name does not match");
            Assert.AreEqual(1, results.QueueSetSnapshot.Queues[0].Requests.Count, "Unexpected number of queue requests returned");
            Assert.AreEqual(requestFrom144.Activity, results.QueueSetSnapshot.Queues[0].Requests[0].Activity, "Queue request activity does not match");
            Assert.AreEqual(requestFrom144.ProjectName, results.QueueSetSnapshot.Queues[0].Requests[0].ProjectName, "Queue request project name does not match");

            mocks.Verify();
        }

        [Test]
        public void GetProjectStatusCorrectlyHandlesCCData()
        {
            var webClient = mocks.Create<WebClient>(MockBehavior.Strict).Object;
            SetupWebClient(webClient, "http://relative/XmlStatusReport.aspx", xmlFromCC);
            var client = new CruiseServerHttpClient("http://relative", webClient);

            var results = client.GetProjectStatus();

            Assert.IsNotNull(results, "No projects parsed");
            Assert.AreEqual(1, results.Length, "Unexpected number of projects returned");
            CompareProjects(projectFromCC, results[0]);
            mocks.Verify();
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

        private void SetupWebClient(WebClient webClient, string requestUrl, string response = null, bool exceptionOnFirstRequest = false)
        {
            var webRequest = mocks.Create<WebRequest>().Object;
            Mock.Get(webRequest).SetupGet(_webRequest => _webRequest.RequestUri).Returns(new Uri(requestUrl));
            Mock.Get(webRequest).Setup(_webRequest => _webRequest.GetRequestStream()).Returns(() => new MemoryStream());
            var webResponse = mocks.Create<WebResponse>().Object;
            if (response != null)
            {
                MemoryStream responseStream = new MemoryStream(Encoding.UTF8.GetBytes(response));
                Mock.Get(webResponse).SetupGet(_webResponse => _webResponse.ContentLength).Returns(responseStream.Length);
                Mock.Get(webResponse).Setup(_webResponse => _webResponse.GetResponseStream()).Returns(responseStream);
            }
            Mock.Get(webClient).Protected().Setup<WebRequest>("GetWebRequest", ItExpr.Is<Uri>(uri => uri.OriginalString.StartsWith(requestUrl))).Returns(webRequest);
            if (exceptionOnFirstRequest)
            {
                Mock.Get(webClient).Protected().SetupSequence<WebResponse>("GetWebResponse", ItExpr.IsAny<WebRequest>())
                    .Throws(new WebException("The remote server returned an error: (403) Forbidden."))
                    .Returns(webResponse);
            }
            else
            {
                Mock.Get(webClient).Protected().Setup<WebResponse>("GetWebResponse", ItExpr.IsAny<WebRequest>()).Returns(webResponse);
            }
        }
    }
}
