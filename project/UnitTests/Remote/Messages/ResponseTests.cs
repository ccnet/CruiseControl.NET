using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class ResponseTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            Response response = new Response();
            response.RequestIdentifier = "new id";
            Assert.AreEqual("new id", response.RequestIdentifier, "RequestIdentifier fails the get/set test");
            response.Result = ResponseResult.Success;
            Assert.AreEqual(ResponseResult.Success, response.Result, "Result fails the get/set test");
            DateTime now = DateTime.Now;
            response.Timestamp = now;
            Assert.AreEqual(now, response.Timestamp, "Timestamp fails the get/set test");
        }

        [Test]
        public void InitialiseNewResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            Response response = new Response();
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromRequestSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ServerRequest request = new ServerRequest();
            Response response = new Response(request);
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.AreEqual(request.Identifier, response.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            Response response1 = new Response();
            response1.Result = ResponseResult.Success;
            response1.RequestIdentifier = "original id";
            response1.Timestamp = DateTime.Now.AddMinutes(-1);
            Response response2 = new Response(response1);
            Assert.AreEqual(ResponseResult.Success, response2.Result, "Result wasn't set to failure");
            Assert.AreEqual("original id", response2.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((response1.Timestamp == response2.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void EqualsMatchesResponseWithTheSameIdentifierAndTimestamp()
        {
            Response response1 = new Response();
            Response response2 = new Response();
            response1.RequestIdentifier = response2.RequestIdentifier;
            response1.Timestamp = response2.Timestamp;
            Assert.IsTrue(response1.Equals(response2));
        }

        [Test]
        public void EqualsDoesNotMatchesResponseWithDifferentIdentifier()
        {
            Response response1 = new Response();
            Response response2 = new Response();
            response1.RequestIdentifier = "response1";
            response2.RequestIdentifier = "response2";
            Assert.IsFalse(response1.Equals(response2));
        }

        [Test]
        public void EqualsDoesNotMatchesResponseWithDifferentTimestamp()
        {
            Response response1 = new Response();
            Response response2 = new Response();
            response1.Timestamp = DateTime.Now.AddMilliseconds(-1);
            response2.Timestamp = DateTime.Now;
            Assert.IsFalse(response1.Equals(response2));
        }

        [Test]
        public void GetHashCodeReturnsHashCodeOfIdentifierAndTimestamp()
        {
            Response response = new Response();
            int expected = string.Empty.GetHashCode() & response.Timestamp.GetHashCode();
            Assert.AreEqual(expected, response.GetHashCode());
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            Response response = new Response();
            string actual = response.ToString();
            string expected = string.Format("<response xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            Response response = new Response();
            response.ErrorMessages.Add(new ErrorMessage("Error 1"));
            response.ErrorMessages.Add(new ErrorMessage("Error 2"));
            response.RequestIdentifier = "request";
            response.Result = ResponseResult.Success;
            response.Timestamp = DateTime.Now;
            string actual = response.ToString();
            string expected = string.Format("<response xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" result=\"{1}\">" + 
                "<error>Error 1</error>" + 
                "<error>Error 2</error>" + 
                "</response>",
                response.RequestIdentifier,
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }
    }
}
