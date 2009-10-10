using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    /// <summary>
    /// Tests the active directory authentication
    /// </summary>
    /// <remarks>
    /// Most of these tests will not run "as-is" because they require a domain name (for the LDAP components
    /// to use). As this will be specific to the machine, these tests have been set to be ignored by nUnit.
    /// </remarks>
    [TestFixture]
    public class ActiveDirectoryAuthenticationTests
    {
        private const string domainName = "ADomain";
        private const string userName = "johndoe";

        [Test]
        [Ignore("This requires a valid DomainName - which cannot be set in a generic way.")]
        public void TestValidUserName()
        {
            //todo pass in a stub/mock ldap service so we can test
            ActiveDirectoryAuthentication authentication = new ActiveDirectoryAuthentication(userName, null);
            authentication.DomainName = domainName;
            LoginRequest credentials = new LoginRequest(userName);
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsTrue(isValid);
        }

        [Test]
        [Ignore("This requires a valid DomainName - which cannot be set in a generic way.")]
        public void TestInvalidUserName()
        {
            //todo pass in a stub/mock ldap service so we can test
            ActiveDirectoryAuthentication authentication = new ActiveDirectoryAuthentication("janedoe",null);
            authentication.DomainName = domainName;
            LoginRequest credentials = new LoginRequest(userName);
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void TestMissingUserName()
        {
            //todo pass in a stub/mock ldap service so we can test
            ActiveDirectoryAuthentication authentication = new ActiveDirectoryAuthentication("janedoe",null);
            LoginRequest credentials = new LoginRequest();
            bool isValid = authentication.Authenticate(credentials);
            Assert.IsFalse(isValid);
        }

        [Test]
        public void GetSetAllProperties()
        {
            ActiveDirectoryAuthentication authentication = new ActiveDirectoryAuthentication();
            authentication.UserName = userName;
            Assert.AreEqual(userName, authentication.UserName, "UserName not correctly set");
            Assert.AreEqual(userName, authentication.Identifier, "Identifier not correctly set");
            authentication.DomainName = domainName;
            Assert.AreEqual(domainName, authentication.DomainName, "DomainName not correctly set");
        }

        [Test]
        public void GetUserNameReturnsName()
        {
            LoginRequest credentials = new LoginRequest(userName);
            ActiveDirectoryAuthentication authentication = new ActiveDirectoryAuthentication();
            string result = authentication.GetUserName(credentials);
            Assert.AreEqual(userName, result);
        }

        [Test]
        [Ignore("This requires a valid DomainName - which cannot be set in a generic way.")]
        public void GetDisplayNameReturnsDisplayName()
        {
            string displayName = "John Doe";
            LoginRequest credentials = new LoginRequest(userName);
            ActiveDirectoryAuthentication authentication = new ActiveDirectoryAuthentication();
            authentication.DomainName = domainName;
            string result = authentication.GetDisplayName(credentials);
            Assert.AreEqual(displayName, result);
        }

        [Test]
        [Ignore("This requires a valid DomainName - which cannot be set in a generic way.")]
        public void GetDisplayNameReturnsUserName()
        {
            LoginRequest credentials = new LoginRequest(userName);
            ActiveDirectoryAuthentication authentication = new ActiveDirectoryAuthentication();
            authentication.DomainName = domainName;
            string result = authentication.GetDisplayName(credentials);
            Assert.AreEqual(userName, result);
        }
    }
}
