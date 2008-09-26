using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class NullSecurityManagerTest
    {
        [Test]
        public void LoginReturnsUserName()
        {
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            ISecurityManager manager = new NullSecurityManager();
            manager.Initialise();
            string sessionToken = manager.Login(credentials);
            Assert.AreEqual(credentials.UserName, sessionToken);
        }

        [Test]
        public void LogoutDoesNothing()
        {
            ISecurityManager manager = new NullSecurityManager();
            manager.Logout("anydetailsinhere");
        }

        [Test]
        public void ValidateSessionReturnsTrue()
        {
            ISecurityManager manager = new NullSecurityManager();
            bool result = manager.ValidateSession("anydetailsinhere");
            Assert.IsTrue(result);
        }

        [Test]
        public void GetUserNameReturnsSessionToken()
        {
            string sessionToken = "anydetailsinhere";
            ISecurityManager manager = new NullSecurityManager();
            string userName = manager.GetUserName(sessionToken);
            Assert.AreEqual(sessionToken, userName);
        }
    }
}
