namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class DataListResponseTests
    {
        [Test]
        public void InitialiseNewResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            DataListResponse response = new DataListResponse();
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");

            var data = new List<string>();
            response.Data = data;
            Assert.AreSame(data, response.Data);
        }

        [Test]
        public void InitialiseResponseFromRequestSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ServerRequest request = new ServerRequest();
            DataListResponse response = new DataListResponse(request);
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.AreEqual(request.Identifier, response.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            DataListResponse response1 = new DataListResponse();
            response1.Result = ResponseResult.Success;
            response1.RequestIdentifier = "original id";
            response1.Timestamp = DateTime.Now.AddMinutes(-1);
            DataListResponse response2 = new DataListResponse(response1);
            Assert.AreEqual(ResponseResult.Success, response2.Result, "Result wasn't set to failure");
            Assert.AreEqual("original id", response2.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((response1.Timestamp == response2.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            DataListResponse response = new DataListResponse();
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<dataListResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            DataListResponse response = new DataListResponse();
            response.ErrorMessages.Add(new ErrorMessage("Error 1"));
            response.ErrorMessages.Add(new ErrorMessage("Error 2"));
            response.RequestIdentifier = "request";
            response.Result = ResponseResult.Success;
            response.Timestamp = DateTime.Now;
            response.Data.Add("new data");
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<dataListResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" result=\"{1}\">" +
                "<error>Error 1</error>" +
                "<error>Error 2</error>" +
                "<data>{3}</data>" +
                "</dataListResponse>",
                response.RequestIdentifier,
                response.Result,
                response.Timestamp,
                response.Data[0]);
            Assert.AreEqual(expected, actual);
        }
    }
}
