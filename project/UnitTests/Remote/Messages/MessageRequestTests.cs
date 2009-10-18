using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class MessageRequestTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            MessageRequest request = new MessageRequest();
            request.Message = "new message";
            Assert.AreEqual("new message", request.Message, "Message fails the get/set test");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            MessageRequest request = new MessageRequest();
            string actual = request.ToString();
            string expected = string.Format("<messageMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" source=\"{1}\">{3}</messageMessage>",
                request.Identifier,
                request.SourceName,
                request.Timestamp,
                "<kind>NotDefined</kind>");
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            MessageRequest request = new MessageRequest();
            request.Identifier = "identifier";
            request.ProjectName = "projectName";
            request.ServerName = "serverName";
            request.SessionToken = "sessionToken";
            request.SourceName = "sourceName";
            request.Timestamp = DateTime.Now;
            request.Message = "message";
            string actual = request.ToString();
            string expected = string.Format("<messageMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\" project=\"{5}\">" + 
                "<message>{6}</message>" +
                "<kind>NotDefined</kind>" +
                "</messageMessage>",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp,
                request.ProjectName,
                request.Message);
            Assert.AreEqual(expected, actual);
        }
    }
}
