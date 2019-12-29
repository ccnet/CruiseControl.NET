using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class InternalSecurityManagerTest
    {
        private const string testSessionToken = "test session token";
        private InternalSecurityManager manager;
        private MockRepository mocks = new MockRepository(MockBehavior.Default);
        private IAuthentication authenticationMock;
        private ISessionCache sessionMock;

        [SetUp]
        public void SetUp()
        {
            authenticationMock = mocks.Create<IAuthentication>().Object;
            Mock.Get(authenticationMock).SetupGet(_authenticationMock => _authenticationMock.Identifier).Returns("johndoe");
            sessionMock = mocks.Create<ISessionCache>().Object;

            manager = new InternalSecurityManager();
            manager.Users = new IAuthentication[]{
                authenticationMock
            };
            manager.SessionCache = sessionMock;
        }

        [Test]
        public void ValidLoginReturnsSessionToken()
        {
            LoginRequest credentials = new LoginRequest("johndoe");
            Mock.Get(authenticationMock).Setup(_authenticationMock => _authenticationMock.Authenticate(credentials)).Returns(true).Verifiable();
            Mock.Get(authenticationMock).Setup(_authenticationMock => _authenticationMock.GetUserName(credentials)).Returns("johndoe").Verifiable();
            Mock.Get(authenticationMock).Setup(_authenticationMock => _authenticationMock.GetDisplayName(credentials)).Returns("johndoe").Verifiable();
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.AddToCache("johndoe")).Returns(testSessionToken).Verifiable();
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.StoreSessionValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.AreEqual(testSessionToken, sessionToken);
        }

        [Test]
        public void ValidWildCardLoginReturnsSessionToken()
        {
            IAuthentication wildcardMock = mocks.Create<IAuthentication>().Object;
            Mock.Get(wildcardMock).SetupGet(_wildcardMock => _wildcardMock.Identifier).Returns("*doe");
            manager.Users = new IAuthentication[]{
                wildcardMock
            };

            LoginRequest credentials = new LoginRequest("johndoe");
            Mock.Get(wildcardMock).Setup(_wildcardMock => _wildcardMock.Authenticate(credentials)).Returns(true).Verifiable();
            Mock.Get(wildcardMock).Setup(_wildcardMock => _wildcardMock.GetUserName(credentials)).Returns("johndoe").Verifiable();
            Mock.Get(wildcardMock).Setup(_wildcardMock => _wildcardMock.GetDisplayName(credentials)).Returns("johndoe").Verifiable();
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.AddToCache("johndoe")).Returns(testSessionToken).Verifiable();
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.StoreSessionValue(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.AreEqual(testSessionToken, sessionToken);
        }

        [Test]
        public void InvalidLoginReturnsNull()
        {
            LoginRequest credentials = new LoginRequest("johndoe");
            Mock.Get(authenticationMock).Setup(_authenticationMock => _authenticationMock.Authenticate(credentials)).Returns(false).Verifiable();
            Mock.Get(authenticationMock).Setup(_authenticationMock => _authenticationMock.GetUserName(credentials)).Returns("johndoe").Verifiable();

            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.IsNull(sessionToken);
        }

        [Test]
        public void UnknownLoginReturnsNull()
        {
            LoginRequest credentials = new LoginRequest("janedoe");

            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.IsNull(sessionToken);
        }

        [Test]
        public void LogoutRemovesSessionFromCache()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveFromCache(testSessionToken)).Returns(testSessionToken).Verifiable();
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RemoveFromCache(testSessionToken)).Verifiable();

            manager.Initialise();

            manager.Logout(testSessionToken);
        }

        [Test]
        public void LogoutForNonExistantSessionIsSafe()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveFromCache(testSessionToken)).Returns((string)null).Verifiable();

            manager.Initialise();

            manager.Logout(testSessionToken);
        }

        [Test]
        public void ValidateSessionReturnsTrueForAValidSession()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveFromCache(testSessionToken)).Returns("johndoe").Verifiable();

            manager.Initialise();

            bool result = manager.ValidateSession(testSessionToken);
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateSessionReturnsFalseForAnInvalidSession()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveFromCache(testSessionToken)).Returns((string)null).Verifiable();

            manager.Initialise();

            bool result = manager.ValidateSession(testSessionToken);
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateSessionReturnsFalseForANullSession()
        {
            manager.Initialise();

            bool result = manager.ValidateSession(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void GetUserNameReturnsUserNameForAValidSession()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveFromCache(testSessionToken)).Returns("johndoe").Verifiable();

            manager.Initialise();

            string result = manager.GetUserName(testSessionToken);
            Assert.AreEqual("johndoe", result);
        }

        [Test]
        public void GetUserNameReturnsNullForAnInvalidSession()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveFromCache(testSessionToken)).Returns((string)null).Verifiable();

            manager.Initialise();

            string result = manager.GetUserName(testSessionToken);
            Assert.IsNull(result);
        }

        [Test]
        public void GetUserNameReturnsNullForANullSession()
        {
            manager.Initialise();

            string result = manager.GetUserName(null);
            Assert.IsNull(result);
        }

        [Test]
        public void GetDisplayNameReturnsDisplayNameForAValidSession()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveSessionValue(testSessionToken, "DisplayName")).Returns("John Doe").Verifiable();

            manager.Initialise();

            string result = manager.GetDisplayName(testSessionToken, null);
            Assert.AreEqual("John Doe", result);
        }

        [Test]
        public void GetDisplayNameReturnsNullForAnInvalidSession()
        {
            Mock.Get(sessionMock).Setup(_sessionMock => _sessionMock.RetrieveSessionValue(testSessionToken, "DisplayName")).Returns(null).Verifiable();

            manager.Initialise();

            string result = manager.GetDisplayName(testSessionToken, null);
            Assert.IsNull(result);
        }

        [Test]
        public void GetDisplayNameReturnsNullForANullSession()
        {
            manager.Initialise();

            string result = manager.GetDisplayName(null, null);
            Assert.IsNull(result);
        }

        [Test]
        public void LogEventSendsEventToLogger()
        {
            string projectName = "Test Project";
            string userName = "johnDoe";
            SecurityEvent eventType = SecurityEvent.ForceBuild;
            SecurityRight eventRight = SecurityRight.Allow;
            string message = "A message";

            IAuditLogger logger = mocks.Create<IAuditLogger>(MockBehavior.Strict).Object;
            Mock.Get(logger).Setup(_logger => _logger.LogEvent(projectName, userName, eventType, eventRight, message)).Verifiable();

            manager.AuditLoggers = new IAuditLogger[] {
                logger
            };
            manager.Initialise();
            manager.LogEvent(projectName, userName, eventType, eventRight, message);
        }

        [Test]
        public void ListAllUsers()
        {
            Mock.Get(authenticationMock).SetupGet(_authenticationMock => _authenticationMock.AuthenticationName).Returns("Mocked");
            Mock.Get(authenticationMock).SetupGet(_authenticationMock => _authenticationMock.DisplayName).Returns("John Doe");
            Mock.Get(authenticationMock).SetupGet(_authenticationMock => _authenticationMock.UserName).Returns("johndoe");
            manager.Initialise();
            List<UserDetails> users = manager.ListAllUsers();
            Assert.IsNotNull(users, "No data returned");
            Assert.AreEqual(1, users.Count, "Unexpected number of users returned");
        }

        [Test]
        public void ReadAuditEventsWithoutReader()
        {
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100);
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void ReadFilteredAuditEventsWithoutReader()
        {
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100, AuditFilters.ByProject("Project #1"));
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void ReadAuditEventsWithReader()
        {
            IAuditReader readerMock = mocks.Create<IAuditReader>().Object;
            List<AuditRecord> records = new List<AuditRecord>();
            records.Add(new AuditRecord());
            Mock.Get(readerMock).Setup(_readerMock => _readerMock.Read(0, 100)).Returns(records);
            manager.AuditReader = readerMock;
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100);
            Assert.AreEqual(1, actual.Count);
        }

        [Test]
        public void ReadFilteredAuditEventsWithReader()
        {
            AuditFilterBase filter = AuditFilters.ByProject("Project #1");
            IAuditReader readerMock = mocks.Create<IAuditReader>().Object;
            List<AuditRecord> records = new List<AuditRecord>();
            records.Add(new AuditRecord());
            Mock.Get(readerMock).Setup(_readerMock => _readerMock.Read(0, 100, filter)).Returns(records);
            manager.AuditReader = readerMock;
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100, filter);
            Assert.AreEqual(1, actual.Count);
        }

        //[Test]
        //[ExpectedException(typeof(NotImplementedException),
        //    ExpectedMessage = "Password management is not allowed for this security manager")]
        //public void ChangePasswordThrowsAnException()
        //{
        //    manager.ChangePassword("session", "oldPassword", "newPassword");
        //}

        //[Test]
        //[ExpectedException(typeof(NotImplementedException),
        //    ExpectedMessage = "Password management is not allowed for this security manager")]
        //public void ResetPasswordThrowsAnException()
        //{
        //    manager.ResetPassword("session", "user", "newPassword");
        //}
    }
}
