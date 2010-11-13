namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote.Messages;
    using ThoughtWorks.CruiseControl.Remote.Parameters;

    [TestFixture]
    public class BuildParametersResponseTests
    {
        [Test]
        public void InitialiseNewResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            BuildParametersResponse response = new BuildParametersResponse();
            var parameters = new List<ParameterBase>();
            response.Parameters = parameters;
            Assert.AreSame(parameters, response.Parameters);
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromRequestSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            ServerRequest request = new ServerRequest();
            BuildParametersResponse response = new BuildParametersResponse(request);
            Assert.AreEqual(ResponseResult.Unknown, response.Result, "Result wasn't set to failure");
            Assert.AreEqual(request.Identifier, response.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((now <= response.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void InitialiseResponseFromResponseSetsTheDefaultValues()
        {
            DateTime now = DateTime.Now;
            BuildParametersResponse response1 = new BuildParametersResponse();
            response1.Result = ResponseResult.Success;
            response1.RequestIdentifier = "original id";
            response1.Timestamp = DateTime.Now.AddMinutes(-1);
            BuildParametersResponse response2 = new BuildParametersResponse(response1);
            Assert.AreEqual(ResponseResult.Success, response2.Result, "Result wasn't set to failure");
            Assert.AreEqual("original id", response2.RequestIdentifier, "RequestIdentifier wasn't set to the identifier of the request");
            Assert.IsTrue((response1.Timestamp == response2.Timestamp), "Timestamp was not set");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            BuildParametersResponse response = new BuildParametersResponse();
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<buildParametersResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            BuildParametersResponse response = new BuildParametersResponse();
            response.ErrorMessages.Add(new ErrorMessage("Error 1"));
            response.ErrorMessages.Add(new ErrorMessage("Error 2"));
            response.RequestIdentifier = "request";
            response.Result = ResponseResult.Success;
            response.Timestamp = DateTime.Now;
            response.Parameters.Add(new TextParameter("text"));
            response.Parameters.Add(new NumericParameter("numeric"));
            response.Parameters.Add(new SelectParameter("select"));
            string actual = response.ToString();
            string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<buildParametersResponse xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" result=\"{1}\">" +
                "<error>Error 1</error>" +
                "<error>Error 2</error>" +
                "<parameter xsi:type=\"TextParameter\" name=\"text\" display=\"text\" />" +
                "<parameter xsi:type=\"NumericParameter\" name=\"numeric\" display=\"numeric\" />" +
                "<parameter xsi:type=\"SelectParameter\" name=\"select\" display=\"select\" />" +
                "</buildParametersResponse>",
                response.RequestIdentifier,
                response.Result,
                response.Timestamp);
            Assert.AreEqual(expected, actual);
        }
    }
}
