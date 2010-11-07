namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class ProjectStatusResponseTests
    {
        [Test]
        public void InitialiseNewResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ProjectStatusResponse response = new ProjectStatusResponse();
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");

            var projects = new List<ProjectStatus>();
            response.Projects = projects;
            Assert.AreSame(projects, response.Projects);
        }

        [Test]
        public void InitialiseResponseFromRequestSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ServerRequest request = new ServerRequest();
            ProjectStatusResponse response = new ProjectStatusResponse(request);
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.AreEqual(request.Identifier, response.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ProjectStatusResponse response1 = new ProjectStatusResponse();
            response1.Result = ResponseResult.Success;
            response1.RequestIdentifier = "original id";
            response1.Timestamp = DateTime.Now.AddMinutes(-1);
            ProjectStatusResponse response2 = new ProjectStatusResponse(response1);
            Assert.AreEqual(ResponseResult.Success, response2.Result, "Result wasn't set to failure");
            Assert.AreEqual("original id", response2.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((response1.Timestamp == response2.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            ProjectStatusResponse response = new ProjectStatusResponse();
            string actual = response.ToString();
            string expected = string.Format("<projectStatusResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            ProjectStatus projectStatus = new ProjectStatus("test project", IntegrationStatus.Success, DateTime.Now);
            ProjectStatusResponse response = new ProjectStatusResponse();
            response.ErrorMessages.Add(new ErrorMessage("Error 1"));
            response.ErrorMessages.Add(new ErrorMessage("Error 2"));
            response.RequestIdentifier = "request";
            response.Result = ResponseResult.Success;
            response.Timestamp = DateTime.Now;
            response.Projects.Add(projectStatus);
            string actual = response.ToString();
            string expected = string.Format("<projectStatusResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" result=\"{1}\">" +
                "<error>Error 1</error>" +
                "<error>Error 2</error>" +
                "<project showForceBuildButton=\"true\" showStartStopButton=\"true\" serverName=\"{5}\" status=\"Running\" buildStatus=\"Success\" name=\"test project\" " +
                "queuePriority=\"0\" lastBuildDate=\"{3:yyyy-MM-ddTHH:mm:ss.FFFFFFF}\" nextBuildTime=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFF}\">" + 
                "<activity type=\"Sleeping\" />" +
                "</project>" + 
                "</projectStatusResponse>",
                response.RequestIdentifier,
                response.Result,
                response.Timestamp,
                projectStatus.LastBuildDate,
                projectStatus.NextBuildTime,
                Environment.MachineName);
            Assert.AreEqual(expected, actual);
        }
    }
}
