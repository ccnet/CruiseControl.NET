﻿using System;
using System.Xml.Linq;
using FluentAssertions;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class DataResponseTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            DataResponse response = new DataResponse();
            response.Data = "new data";
            Assert.AreEqual("new data", response.Data, "Data fails get/set test");
        }

        [Test]
        public void InitialiseNewResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            DataResponse response = new DataResponse();
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromRequestSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ServerRequest request = new ServerRequest();
            DataResponse response = new DataResponse(request);
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.AreEqual(request.Identifier, response.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            DataResponse response1 = new DataResponse();
            response1.Result = ResponseResult.Success;
            response1.RequestIdentifier = "original id";
            response1.Timestamp = DateTime.Now.AddMinutes(-1);
            DataResponse response2 = new DataResponse(response1);
            Assert.AreEqual(ResponseResult.Success, response2.Result, "Result wasn't set to failure");
            Assert.AreEqual("original id", response2.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((response1.Timestamp == response2.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            DataResponse response = new DataResponse();
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<dataResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                response.Result,
                response.Timestamp);

            XDocument.Parse(actual).Should().BeEquivalentTo(XDocument.Parse(expected));
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            DataResponse response = new DataResponse();
            response.ErrorMessages.Add(new ErrorMessage("Error 1"));
            response.ErrorMessages.Add(new ErrorMessage("Error 2"));
            response.RequestIdentifier = "request";
            response.Result = ResponseResult.Success;
            response.Timestamp = DateTime.Now;
            response.Data = "new data";
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<dataResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" result=\"{1}\">" +
                "<error>Error 1</error>" +
                "<error>Error 2</error>" +
                "<data>{3}</data>" +
                "</dataResponse>",
                response.RequestIdentifier,
                response.Result,
                response.Timestamp,
                response.Data);

            XDocument.Parse(actual).Should().BeEquivalentTo(XDocument.Parse(expected));
        }
    }
}
