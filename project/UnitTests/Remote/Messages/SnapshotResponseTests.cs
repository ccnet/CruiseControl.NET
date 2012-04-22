using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class SnapshotResponseTests
    {
        [Test]
        public void InitialiseNewResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            SnapshotResponse response = new SnapshotResponse();
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromRequestSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ServerRequest request = new ServerRequest();
            SnapshotResponse response = new SnapshotResponse(request);
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.AreEqual(request.Identifier, response.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            SnapshotResponse response1 = new SnapshotResponse();
            response1.Result = ResponseResult.Success;
            response1.RequestIdentifier = "original id";
            response1.Timestamp = DateTime.Now.AddMinutes(-1);
            SnapshotResponse response2 = new SnapshotResponse(response1);
            Assert.AreEqual(ResponseResult.Success, response2.Result, "Result wasn't set to failure");
            Assert.AreEqual("original id", response2.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((response1.Timestamp == response2.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            SnapshotResponse response = new SnapshotResponse();
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<snapshotResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            QueueSnapshot queueSnapshot = new QueueSnapshot("queue1");
            queueSnapshot.Requests.Add(new QueuedRequestSnapshot("test project", ProjectActivity.Pending));
            QueueSetSnapshot queueSetSnapshot = new QueueSetSnapshot();
            queueSetSnapshot.Queues.Add(queueSnapshot);
            ProjectStatus projectStatus = new ProjectStatus("test project", IntegrationStatus.Success, DateTime.Now);
            CruiseServerSnapshot snapshot = new CruiseServerSnapshot(
                new ProjectStatus[] { projectStatus },
                queueSetSnapshot);
            SnapshotResponse response = new SnapshotResponse();
            response.ErrorMessages.Add(new ErrorMessage("Error 1"));
            response.ErrorMessages.Add(new ErrorMessage("Error 2"));
            response.RequestIdentifier = "request";
            response.Result = ResponseResult.Success;
            response.Timestamp = DateTime.Now;
            response.Snapshot = snapshot;
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<snapshotResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" result=\"{1}\">" +
                "<error>Error 1</error>" +
                "<error>Error 2</error>" +
                "<snapshot>" +
                    "<projects>" +
                        "<projectStatus showForceBuildButton=\"true\" showStartStopButton=\"true\" serverName=\"{6}\" status=\"Running\" buildStatus=\"Success\" name=\"test project\" " +
                            "queuePriority=\"0\" lastBuildDate=\"{3:yyyy-MM-ddTHH:mm:ss.FFFFFFF}\" nextBuildTime=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFF}\">" +
                            "<activity type=\"Sleeping\" />" +
                            "<parameters />" +
                        "</projectStatus>" + 
                    "</projects>" +
                    "<queueSet>" +
                        "<queue name=\"queue1\">" +
                            "<queueRequest projectName=\"test project\" time=\"{5:yyyy-MM-ddTHH:mm:ss.FFFFFFF}\">" +
                                "<activity type=\"Pending\" />" +
                            "</queueRequest>" + 
                        "</queue>" +
                    "</queueSet>" + 
                "</snapshot>" + 
                "</snapshotResponse>",
                response.RequestIdentifier,
                response.Result,
                response.Timestamp,
                projectStatus.LastBuildDate,
                projectStatus.NextBuildTime,
                DateTime.MinValue,
                Environment.MachineName);
            Assert.AreEqual(expected, actual);
        }
    }
}
