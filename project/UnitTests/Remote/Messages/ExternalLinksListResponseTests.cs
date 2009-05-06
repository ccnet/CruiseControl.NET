using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class ExternalLinksListResponseTests
    {
        [Test]
        public void InitialiseNewResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ExternalLinksListResponse response = new ExternalLinksListResponse();
            Assert.AreEqual(ResponseResult.Failure, response.Result, "Result wasn't set to failure");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromRequestSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ServerRequest request = new ServerRequest();
            ExternalLinksListResponse response = new ExternalLinksListResponse(request);
            Assert.AreEqual(ResponseResult.Failure, response.Result, "Result wasn't set to failure");
            Assert.AreEqual(request.Identifier, response.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ExternalLinksListResponse response1 = new ExternalLinksListResponse();
            response1.Result = ResponseResult.Success;
            response1.RequestIdentifier = "original id";
            response1.Timestamp = DateTime.Now.AddMinutes(-1);
            ExternalLinksListResponse response2 = new ExternalLinksListResponse(response1);
            Assert.AreEqual(ResponseResult.Success, response2.Result, "Result wasn't set to failure");
            Assert.AreEqual("original id", response2.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((response1.Timestamp == response2.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            ExternalLinksListResponse response = new ExternalLinksListResponse();
            string actual = response.ToString();
            string expected = string.Format("<externalLinksResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "result=\"{0}\" timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" />",
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            ExternalLinksListResponse response = new ExternalLinksListResponse();
            response.ErrorMessages.Add(new ErrorMessage("Error 1"));
            response.ErrorMessages.Add(new ErrorMessage("Error 2"));
            response.RequestIdentifier = "request";
            response.Result = ResponseResult.Success;
            response.Timestamp = DateTime.Now;
            response.ExternalLinks.Add(new ExternalLink("link name", "link url"));
            string actual = response.ToString();
            string expected = string.Format("<externalLinksResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "identifier=\"{0}\" result=\"{1}\" timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\">" +
                "<error>Error 1</error>" +
                "<error>Error 2</error>" +
                "<link name=\"link name\" url=\"link url\" />" + 
                "</externalLinksResponse>",
                response.RequestIdentifier,
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }
    }
}
