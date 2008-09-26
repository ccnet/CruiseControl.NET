using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class UserPasswordAuthenticationTest
    {
        [Test]
        public void TestValidUserNameAndPassword()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            credentials["password"] = "iknowyou";
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void TestMissingPassword()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestMissingUserName()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            UserNameCredentials credentials = new UserNameCredentials();
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestIncorrectPassword()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            credentials["password"] = "idontknowyou";
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestIncorrectUserName()
        {
            UserPasswordAuthentication authentication = new UserPasswordAuthentication("johndoe", "iknowyou");
            UserNameCredentials credentials = new UserNameCredentials("janedoe");
            credentials["password"] = "iknowyou";
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }
    }
}
