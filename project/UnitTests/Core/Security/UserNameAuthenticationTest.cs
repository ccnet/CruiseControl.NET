using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class UserNameAuthenticationTest
    {
        [Test]
        public void TestValidUserName()
        {
            UserNameAuthentication authentication = new UserNameAuthentication("johndoe");
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsTrue(isValid);
        }

        [Test]
        public void TestInvalidUserName()
        {
            UserNameAuthentication authentication = new UserNameAuthentication("janedoe");
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestMissingUserName()
        {
            UserNameAuthentication authentication = new UserNameAuthentication("janedoe");
            UserNameCredentials credentials = new UserNameCredentials();
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }
    }
}
