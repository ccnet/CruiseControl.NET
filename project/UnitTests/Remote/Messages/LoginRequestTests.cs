using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class LoginRequestTests
    {
        [Test]
        public void InitialiseRequestWithAUsernameSetsCorrectCredential()
        {
            LoginRequest request = new LoginRequest("johndoe");
            string actual = NameValuePair.FindNamedValue(request.Credentials, LoginRequest.UserNameCredential);
            Assert.AreEqual("johndoe", actual);
        }

        [Test]
        public void AddCredentialAddsToUnderlyingList()
        {
            LoginRequest request = new LoginRequest();
            request.AddCredential(LoginRequest.PasswordCredential, "whoami");
            string actual = NameValuePair.FindNamedValue(request.Credentials, LoginRequest.PasswordCredential);
            Assert.AreEqual("whoami", actual);
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            LoginRequest request = new LoginRequest();
            string actual = request.ToString();
            string expected = string.Format("<loginMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "identifier=\"{0}\" source=\"{1}\" timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" />",
                request.Identifier,
                request.SourceName,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            LoginRequest request = new LoginRequest();
            request.Identifier = "identifier";
            request.ServerName = "serverName";
            request.SessionToken = "sessionToken";
            request.SourceName = "sourceName";
            request.Timestamp = DateTime.Now;
            request.AddCredential(LoginRequest.UserNameCredential, "johnDoe");
            string actual = request.ToString();
            string expected = string.Format("<loginMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\" timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\">" + 
                "<credential name=\"userName\" value=\"johnDoe\" />" + 
                "</loginMessage>",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }
    }
}
