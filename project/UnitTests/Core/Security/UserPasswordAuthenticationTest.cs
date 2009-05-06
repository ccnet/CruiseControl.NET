using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class UserPasswordAuthenticationTest
    {
        [Test]
        public void TestValidUserNameAndPassword()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            LoginRequest credentials = new LoginRequest("johndoe");
            credentials.AddCredential(LoginRequest.PasswordCredential, "iknowyou");
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void TestMissingPassword()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            LoginRequest credentials = new LoginRequest("johndoe");
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestMissingUserName()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            LoginRequest credentials = new LoginRequest();
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestIncorrectPassword()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            LoginRequest credentials = new LoginRequest("johndoe");
            credentials.AddCredential(LoginRequest.PasswordCredential, "idontknowyou");
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestIncorrectUserName()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            LoginRequest credentials = new LoginRequest("janedoe");
            credentials.AddCredential(LoginRequest.PasswordCredential, "iknowyou");
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void GetSetAllProperties()
        {
            string userName = "johndoe";
            string displayName = "John Doe";
            string password = "whoareyou";
            UserPasswordAuthentication authentication = new UserPasswordAuthentication();
            authentication.UserName = userName;
            Assert.AreEqual(userName, authentication.UserName, "UserName not correctly set");
            Assert.AreEqual(userName, authentication.Identifier, "Identifier not correctly set");
            authentication.Password = password;
            Assert.AreEqual(password, authentication.Password, "Password not correctly set");
            authentication.DisplayName = displayName;
            Assert.AreEqual(displayName, authentication.DisplayName, "DisplayName not correctly set");
        }

        [Test]
        public void GetUserNameReturnsName()
        {
            string userName = "johndoe";
            LoginRequest credentials = new LoginRequest(userName);
            UserPasswordAuthentication authentication = new UserPasswordAuthentication();
            string result = authentication.GetUserName(credentials);
            Assert.AreEqual(userName, result);
        }

        [Test]
        public void GetDisplayNameReturnsDisplayName()
        {
            string userName = "johndoe";
            string displayName = "John Doe";
            LoginRequest credentials = new LoginRequest(userName);
            UserPasswordAuthentication authentication = new UserPasswordAuthentication();
            authentication.DisplayName = "John Doe";
            string result = authentication.GetDisplayName(credentials);
            Assert.AreEqual(displayName, result);
        }

        [Test]
        public void GetDisplayNameReturnsUserName()
        {
            string userName = "johndoe";
            LoginRequest credentials = new LoginRequest(userName);
            UserPasswordAuthentication authentication = new UserPasswordAuthentication();
            string result = authentication.GetDisplayName(credentials);
            Assert.AreEqual(userName, result);
        }
    }
}
