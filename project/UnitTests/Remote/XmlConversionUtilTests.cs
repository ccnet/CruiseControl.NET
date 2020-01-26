﻿namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using System.Xml.Linq;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Messages;

    [TestFixture]
    public class XmlConversionUtilTests
    {
        #region Test methods
        #region FindMessageType()
        [Test]
        public void FindMessageTypeMatchesKnownXmlMessage()
        {
            Type messageType = XmlConversionUtil.FindMessageType("response");
            Assert.AreEqual(typeof(Response), messageType);
        }

        [Test]
        public void FindMessageTypeReturnsNullForUnknownXmlMessage()
        {
            Type messageType = XmlConversionUtil.FindMessageType("garbage");
            Assert.IsNull(messageType);
        }
        #endregion

        #region ConvertXmlToObject()

        [Test]
        public void ConvertXmlToObjectConvertsCorrectly()
        {
            string xml = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<response xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                ResponseResult.Success,
                DateTime.Today);

            object result = XmlConversionUtil.ConvertXmlToObject(typeof(Response), xml);

            using (new AssertionScope())
            {
                result.Should().BeOfType<Response>();
                XDocument.Parse(result.ToString()).Should().BeEquivalentTo(XDocument.Parse(xml));
            }
        }

        #endregion

        #region ConvertXmlToRequest()

        [Test]
        public void ConvertXmlToRequestConvertsRequest()
        {
            var request = new ServerRequest("123456-789");
            request.ServerName = "theServer";
            var xmlString = request.ToString();

            var convertedRequest = XmlConversionUtil.ConvertXmlToRequest(xmlString);

            Assert.AreEqual("123456-789", convertedRequest.SessionToken);
            Assert.AreEqual("theServer", convertedRequest.ServerName);
        }

        #endregion

        #region ProcessResponse()

        [Test]
        public void ProcessResponseHandlesKnownMessage()
        {
            string xml = string.Format(System.Globalization.CultureInfo.CurrentCulture,"<response xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{1:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" result=\"{0}\" />",
                ResponseResult.Success,
                DateTime.Today);

            object result = XmlConversionUtil.ProcessResponse(xml);

            using (new AssertionScope())
            {
                result.Should().BeOfType<Response>();
                XDocument.Parse(result.ToString()).Should().BeEquivalentTo(XDocument.Parse(xml));
            }
        }

        [Test]
        public void ProcessResponseThrowsAnExceptionForUnknownMessage()
        {

            Action act = () => { XmlConversionUtil.ProcessResponse("<garbage/>");  };

            act.Should().Throw<CommunicationsException>();
        }

        #endregion
        #endregion
    }
}
