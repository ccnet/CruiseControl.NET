using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;

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
            var credentials = new List<NameValuePair>();
            request.Credentials = credentials;
            Assert.AreSame(credentials, request.Credentials);
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
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" source=\"{1}\" />",
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
                "timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\">" + 
                "<credential name=\"userName\" value=\"johnDoe\" />" + 
                "</loginMessage>",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FindCredentialFindsExistingCredential()
        {
            var request = new LoginRequest();
            var password = "whoareyou";
            request.Credentials = new List<NameValuePair>
            {
                new NameValuePair("name", "me"),
                new NameValuePair("password", password)
            };
            var credential = request.FindCredential("password");
            Assert.IsNotNull(credential);
            Assert.AreEqual(password, credential.Value);
        }

        [Test]
        public void FindCredentialHandlesMissingCredential()
        {
            var request = new LoginRequest();
            request.Credentials = new List<NameValuePair>
            {
                new NameValuePair("name", "me")
            };
            var credential = request.FindCredential("password");
            Assert.IsNull(credential);
        }
    }
}
