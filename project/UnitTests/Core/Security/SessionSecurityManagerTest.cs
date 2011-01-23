using NMock;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class SessionSecurityManagerTest
    {
        private const string _sessionToken = "test session token";
        private SessionSecurityManager _manager;
        private DynamicMock _authenticationMock;
        private DynamicMock _sessionMock;

        [SetUp]
        public void SetUp()
        {
            _authenticationMock = new DynamicMock(typeof(IAuthentication));
            _authenticationMock.SetupResult("Identifier", "johndoe");
            _sessionMock = new DynamicMock(typeof(ISessionCache));

            _manager = new SessionSecurityManager();
            _manager.Settings = new IAuthentication[]{
                _authenticationMock.MockInstance as IAuthentication
            };
            _manager.SessionCache = _sessionMock.MockInstance as ISessionCache;
            _manager.Initialise();
        }

        [Test]
        public void ValidLoginReturnsSessionToken()
        {
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            _authenticationMock.ExpectAndReturn("Authenticate", true, credentials);
            _authenticationMock.ExpectAndReturn("GetUserName", "johndoe", credentials);
            _sessionMock.ExpectAndReturn("AddToCache", _sessionToken, "johndoe");

            string sessionToken = _manager.Login(credentials);
            Assert.AreEqual(_sessionToken, sessionToken);
            VerifyAll();
        }

        [Test]
        public void InvalidLoginReturnsNull()
        {
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            _authenticationMock.ExpectAndReturn("Authenticate", false, credentials);

            string sessionToken = _manager.Login(credentials);
            Assert.IsNull(sessionToken);
            VerifyAll();
        }

        [Test]
        public void UnknownLoginReturnsNull()
        {
            UserNameCredentials credentials = new UserNameCredentials("janedoe");

            string sessionToken = _manager.Login(credentials);
            Assert.IsNull(sessionToken);
            VerifyAll();
        }

        [Test]
        public void LogoutRemovesSessionFromCache()
        {
            _sessionMock.ExpectAndReturn("RetrieveFromCache", _sessionToken, _sessionToken);
            _sessionMock.Expect("RemoveFromCache", _sessionToken);
            _manager.Logout(_sessionToken);
            VerifyAll();
        }

        [Test]
        public void ValidateSessionReturnsTrueForAValidSession()
        {
            _sessionMock.ExpectAndReturn("RetrieveFromCache", "johndoe", _sessionToken);
            bool result = _manager.ValidateSession(_sessionToken);
            Assert.IsTrue(result);
            VerifyAll();
        }

        [Test]
        public void ValidateSessionReturnsFalseForAnInvalidSession()
        {
            _sessionMock.ExpectAndReturn("RetrieveFromCache", null, _sessionToken);
            bool result = _manager.ValidateSession(_sessionToken);
            Assert.IsFalse(result);
            VerifyAll();
        }

        [Test]
        public void GetUserNameReturnsUserNameForAValidSession()
        {
            _sessionMock.ExpectAndReturn("RetrieveFromCache", "johndoe", _sessionToken);
            string result = _manager.GetUserName(_sessionToken);
            Assert.AreEqual("johndoe", result);
            VerifyAll();
        }

        [Test]
        public void GetUserNameReturnsNullForAnInvalidSession()
        {
            _sessionMock.ExpectAndReturn("RetrieveFromCache", null, _sessionToken);
            string result = _manager.GetUserName(_sessionToken);
            Assert.IsNull(result);
            VerifyAll();
        }

        private void VerifyAll()
        {
            _authenticationMock.Verify();
            _sessionMock.Verify();
        }
    }
}
