using NUnit.Framework;
using Rhino.Mocks;
using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Core.Security;
using ThoughtWorks.CruiseControl.Core.Security.Auditing;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Security
{
    [TestFixture]
    public class InternalSecurityManagerTest
    {
        private const string testSessionToken = "test session token";
        private InternalSecurityManager manager;
        private MockRepository mocks = new MockRepository();
        private IAuthentication authenticationMock;
        private ISessionCache sessionMock;

        [SetUp]
        public void SetUp()
        {
            authenticationMock = mocks.DynamicMock<IAuthentication>();
            SetupResult.For(authenticationMock.Identifier).Return("johndoe");
            sessionMock = mocks.DynamicMock<ISessionCache>();

            manager = new InternalSecurityManager();
            manager.Users = new IAuthentication[]{
                authenticationMock
            };
            manager.SessionCache = sessionMock;
        }

        [Test]
        public void ValidLoginReturnsSessionToken()
        {
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            Expect.Call(authenticationMock.Authenticate(credentials)).Return(true);
            Expect.Call(authenticationMock.GetUserName(credentials)).Return("johndoe");
            Expect.Call(authenticationMock.GetDisplayName(credentials)).Return("johndoe");
            Expect.Call(sessionMock.AddToCache("johndoe")).Return(testSessionToken);
            Expect.Call(delegate { sessionMock.StoreSessionValue(string.Empty, string.Empty, string.Empty); })
                .IgnoreArguments();

            mocks.ReplayAll();
            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.AreEqual(testSessionToken, sessionToken);
        }

        [Test]
        public void ValidWildCardLoginReturnsSessionToken()
        {
            IAuthentication wildcardMock = mocks.DynamicMock<IAuthentication>();
            SetupResult.For(wildcardMock.Identifier).Return("*doe");
            manager.Users = new IAuthentication[]{
                wildcardMock
            };

            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            Expect.Call(wildcardMock.Authenticate(credentials)).Return(true);
            Expect.Call(wildcardMock.GetUserName(credentials)).Return("johndoe");
            Expect.Call(wildcardMock.GetDisplayName(credentials)).Return("johndoe");
            Expect.Call(sessionMock.AddToCache("johndoe")).Return(testSessionToken);
            Expect.Call(delegate { sessionMock.StoreSessionValue(string.Empty, string.Empty, string.Empty); })
                .IgnoreArguments();

            mocks.ReplayAll();
            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.AreEqual(testSessionToken, sessionToken);
        }

        [Test]
        public void InvalidLoginReturnsNull()
        {
            UserNameCredentials credentials = new UserNameCredentials("johndoe");
            Expect.Call(authenticationMock.Authenticate(credentials)).Return(false);
            Expect.Call(authenticationMock.GetUserName(credentials)).Return("johndoe");

            mocks.ReplayAll();
            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.IsNull(sessionToken);
        }

        [Test]
        public void UnknownLoginReturnsNull()
        {
            UserNameCredentials credentials = new UserNameCredentials("janedoe");

            mocks.ReplayAll();
            manager.Initialise();

            string sessionToken = manager.Login(credentials);
            Assert.IsNull(sessionToken);
        }

        [Test]
        public void LogoutRemovesSessionFromCache()
        {
            Expect.Call(sessionMock.RetrieveFromCache(testSessionToken)).Return(testSessionToken);
            Expect.Call(delegate { sessionMock.RemoveFromCache(testSessionToken); });
            
            mocks.ReplayAll();
            manager.Initialise();

            manager.Logout(testSessionToken);
        }

        [Test]
        public void LogoutForNonExistantSessionIsSafe()
        {
            Expect.Call(sessionMock.RetrieveFromCache(testSessionToken)).Return(null);

            mocks.ReplayAll();
            manager.Initialise();

            manager.Logout(testSessionToken);
        }

        [Test]
        public void ValidateSessionReturnsTrueForAValidSession()
        {
            Expect.Call(sessionMock.RetrieveFromCache(testSessionToken)).Return("johndoe");

            mocks.ReplayAll();
            manager.Initialise();

            bool result = manager.ValidateSession(testSessionToken);
            Assert.IsTrue(result);
        }

        [Test]
        public void ValidateSessionReturnsFalseForAnInvalidSession()
        {
            Expect.Call(sessionMock.RetrieveFromCache(testSessionToken)).Return(null);

            mocks.ReplayAll();
            manager.Initialise();

            bool result = manager.ValidateSession(testSessionToken);
            Assert.IsFalse(result);
        }

        [Test]
        public void ValidateSessionReturnsFalseForANullSession()
        {
            mocks.ReplayAll();
            manager.Initialise();

            bool result = manager.ValidateSession(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void GetUserNameReturnsUserNameForAValidSession()
        {
            Expect.Call(sessionMock.RetrieveFromCache(testSessionToken)).Return("johndoe");

            mocks.ReplayAll();
            manager.Initialise();

            string result = manager.GetUserName(testSessionToken);
            Assert.AreEqual("johndoe", result);
        }

        [Test]
        public void GetUserNameReturnsNullForAnInvalidSession()
        {
            Expect.Call(sessionMock.RetrieveFromCache(testSessionToken)).Return(null);

            mocks.ReplayAll();
            manager.Initialise();

            string result = manager.GetUserName(testSessionToken);
            Assert.IsNull(result);
        }

        [Test]
        public void GetUserNameReturnsNullForANullSession()
        {
            mocks.ReplayAll();
            manager.Initialise();

            string result = manager.GetUserName(null);
            Assert.IsNull(result);
        }

        [Test]
        public void GetDisplayNameReturnsDisplayNameForAValidSession()
        {
            Expect.Call(sessionMock.RetrieveSessionValue(testSessionToken, "DisplayName")).Return("John Doe");

            mocks.ReplayAll();
            manager.Initialise();

            string result = manager.GetDisplayName(testSessionToken);
            Assert.AreEqual("John Doe", result);
        }

        [Test]
        public void GetDisplayNameReturnsNullForAnInvalidSession()
        {
            Expect.Call(sessionMock.RetrieveSessionValue(testSessionToken, "DisplayName")).Return(null);

            mocks.ReplayAll();
            manager.Initialise();

            string result = manager.GetDisplayName(testSessionToken);
            Assert.IsNull(result);
        }

        [Test]
        public void GetDisplayNameReturnsNullForANullSession()
        {
            mocks.ReplayAll();
            manager.Initialise();

            string result = manager.GetDisplayName(null);
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

            IAuditLogger logger = mocks.StrictMock<IAuditLogger>();
            Expect.Call(delegate { logger.LogEvent(projectName, userName, eventType, eventRight, message); });

            mocks.ReplayAll();
            manager.AuditLoggers = new IAuditLogger[] {
                logger
            };
            manager.Initialise();
            manager.LogEvent(projectName, userName, eventType, eventRight, message);
        }

        [Test]
        public void ListAllUsers()
        {
            SetupResult.For(authenticationMock.AuthenticationName).Return("Mocked");
            SetupResult.For(authenticationMock.DisplayName).Return("John Doe");
            SetupResult.For(authenticationMock.UserName).Return("johndoe");
            mocks.ReplayAll();
            manager.Initialise();
            List<UserDetails> users = manager.ListAllUsers();
            Assert.IsNotNull(users, "No data returned");
            Assert.AreEqual(1, users.Count, "Unexpected number of users returned");
            Assert.AreEqual("johndoe", users[0].UserName, "User name does not match");
            Assert.AreEqual("John Doe", users[0].DisplayName, "Display name does not match");
            Assert.AreEqual("Mocked", users[0].Type, "Type does not match");
        }

        [Test]
        public void ReadAuditEventsWithoutReader()
        {
            mocks.ReplayAll();
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100);
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void ReadFilteredAuditEventsWithoutReader()
        {
            mocks.ReplayAll();
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100, AuditFilters.ByProject("Project #1"));
            Assert.AreEqual(0, actual.Count);
        }

        [Test]
        public void ReadAuditEventsWithReader()
        {
            IAuditReader readerMock = mocks.DynamicMock<IAuditReader>();
            List<AuditRecord> records = new List<AuditRecord>();
            records.Add(new AuditRecord());
            SetupResult.For(readerMock.Read(0, 100)).Return(records);
            manager.AuditReader = readerMock;
            mocks.ReplayAll();
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100);
            Assert.AreEqual(1, actual.Count);
        }

        [Test]
        public void ReadFilteredAuditEventsWithReader()
        {
            IAuditFilter filter = AuditFilters.ByProject("Project #1");
            IAuditReader readerMock = mocks.DynamicMock<IAuditReader>();
            List<AuditRecord> records = new List<AuditRecord>();
            records.Add(new AuditRecord());
            SetupResult.For(readerMock.Read(0, 100, filter)).Return(records);
            manager.AuditReader = readerMock;
            mocks.ReplayAll();
            manager.Initialise();
            List<AuditRecord> actual = manager.ReadAuditRecords(0, 100, filter);
            Assert.AreEqual(1, actual.Count);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException),
            ExpectedMessage = "Password management is not allowed for this security manager")]
        public void ChangePasswordThrowsAnException()
        {
            manager.ChangePassword("session", "oldPassword", "newPassword");
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException),
            ExpectedMessage = "Password management is not allowed for this security manager")]
        public void ResetPasswordThrowsAnException()
        {
            manager.ResetPassword("session", "user", "newPassword");
        }
    }
}
