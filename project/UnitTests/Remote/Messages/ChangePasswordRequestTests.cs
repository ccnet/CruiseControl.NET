using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote.Messages
{
    [TestFixture]
    public class ChangePasswordRequestTests
    {
        [Test]
        public void GetSetAllPropertiesWorks()
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.OldPassword = "old password";
            Assert.AreEqual("old password", request.OldPassword, "OldPassword fails the get/set test");
            request.NewPassword = "new password";
            Assert.AreEqual("new password", request.NewPassword, "NewPassword fails the get/set test");
            request.UserName = "new user";
            Assert.AreEqual("new user", request.UserName, "UserName fails the get/set test");
        }

        [Test]
        public void ToStringSerialisesDefaultValues()
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            string actual = request.ToString();
            string expected = string.Format("<changePasswordMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{2:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" source=\"{1}\" />",
                request.Identifier,
                request.SourceName,
                request.Timestamp);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void ToStringSerialisesAllValues()
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.Identifier = "identifier";
            request.OldPassword = "old password";
            request.NewPassword = "new password";
            request.UserName = "user name";
            request.ServerName = "serverName";
            request.SessionToken = "sessionToken";
            request.SourceName = "sourceName";
            request.Timestamp = DateTime.Now;
            string actual = request.ToString();
            string expected = string.Format("<changePasswordMessage xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" " +
                "timestamp=\"{4:yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz}\" identifier=\"{0}\" server=\"{1}\" source=\"{2}\" session=\"{3}\" oldPassword=\"{5}\" newPassword=\"{6}\" userName=\"{7}\" />",
                request.Identifier,
                request.ServerName,
                request.SourceName,
                request.SessionToken,
                request.Timestamp,
                request.OldPassword,
                request.NewPassword,
                request.UserName);
            Assert.AreEqual(expected, actual);
        }
    }
}
