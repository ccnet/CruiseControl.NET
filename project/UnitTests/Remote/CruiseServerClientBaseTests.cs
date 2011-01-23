namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using ThoughtWorks.CruiseControl.Remote;
    using ThoughtWorks.CruiseControl.Remote.Security;

    public class CruiseServerClientBaseTests
    {
        #region Tests
        [Test]
        public void LoginConvertsObjectToCredentials()
        {
            var client = new TestClient();
            var credentials = new
            {
                UserName = "JohnDoe",
                Password = (string)null
            };
            client.Login(credentials);
            Assert.AreEqual(2, client.Credentials.Count);
            Assert.AreEqual("UserName", client.Credentials[0].Name);
            Assert.AreEqual(credentials.UserName, client.Credentials[0].Value);
            Assert.AreEqual("Password", client.Credentials[1].Name);
            Assert.AreEqual(credentials.Password, client.Credentials[1].Value);
        }

        [Test]
        public void DisposeLogsOutWhenLoggedIn()
        {
            var client = new TestClient();
            Assert.IsFalse(client.LoggedOut);
            client.Login(new { });
            client.Dispose();
            Assert.IsTrue(client.LoggedOut);
        }

        [Test]
        public void DisposeDoesNothingWhenNotLoggedIn()
        {
            var client = new TestClient();
            Assert.IsFalse(client.LoggedOut);
            client.Dispose();
            Assert.IsFalse(client.LoggedOut);
        }

        [Test]
        public void ForceBuildIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(
                () => client.ForceBuild("test", new List<NameValuePair>(), BuildCondition.ForceBuild));
        }

        [Test]
        public void AbortBuildIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.AbortBuild("test"));
        }

        [Test]
        public void RequestIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.Request("test", null));
        }

        [Test]
        public void StartProjectIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.StartProject("test"));
        }

        [Test]
        public void StopProjectIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.StopProject("test"));
        }

        [Test]
        public void SendMessageIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.SendMessage("test", new Message()));
        }

        [Test]
        public void WaitForExitIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.WaitForExit("test"));
        }

        [Test]
        public void CancelPendingRequestIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.CancelPendingRequest("test"));
        }

        [Test]
        public void GetCruiseServerSnapshotIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetCruiseServerSnapshot());
        }

        [Test]
        public void GetLatestBuildNameIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetLatestBuildName("test"));
        }

        [Test]
        public void GetBuildNamesIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetBuildNames("test"));
        }

        [Test]
        public void GetMostRecentBuildNamesIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetMostRecentBuildNames("test", 5));
        }

        [Test]
        public void GetLogIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetLog("test", "testing"));
        }

        [Test]
        public void GetFinalBuildStatusIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetFinalBuildStatus("test", "testing"));
        }

        [Test]
        public void GetServerLogForServerIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetServerLog());
        }

        [Test]
        public void GetServerLogForProjectIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetServerLog("test"));
        }

        [Test]
        public void GetServerVersionIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetServerVersion());
        }

        [Test]
        public void AddProjectIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.AddProject("test"));
        }

        [Test]
        public void DeleteProjectIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.DeleteProject("test", true, true, true));
        }

        [Test]
        public void GetProjectIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetProject("test"));
        }

        [Test]
        public void UpdateProjectIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.UpdateProject("test", "testing"));
        }

        [Test]
        public void GetExternalLinksIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetExternalLinks("test"));
        }

        [Test]
        public void GetArtifactDirectoryIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetArtifactDirectory("test"));
        }

        [Test]
        public void GetStatisticsDocumentIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetStatisticsDocument("test"));
        }

        [Test]
        public void GetModificationHistoryDocumentIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetModificationHistoryDocument("test"));
        }

        [Test]
        public void GetRSSFeedIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetRSSFeed("test"));
        }

        [Test]
        public void GetSecurityConfigurationIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetSecurityConfiguration());
        }

        [Test]
        public void ListUsersIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.ListUsers());
        }

        [Test]
        public void DiagnoseSecurityPermissionsIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.DiagnoseSecurityPermissions("test"));
        }

        [Test]
        public void ReadAuditRecordsIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.ReadAuditRecords(0, 10));
        }

        [Test]
        public void ReadAuditRecordsWithFilterIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(
                () => client.ReadAuditRecords(0, 10, AuditFilters.ByRight(SecurityRight.Allow)));
        }

        [Test]
        public void ListBuildParametersIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.ListBuildParameters("test"));
        }

        [Test]
        public void ChangePasswordIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.ChangePassword("test", "testing"));
        }

        [Test]
        public void ResetPasswordIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.ResetPassword("test", "testing"));
        }

        [Test]
        public void TakeStatusSnapshotIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.TakeStatusSnapshot("test"));
        }

        [Test]
        public void RetrievePackageListIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.RetrievePackageList("test"));
        }

        [Test]
        public void RetrievePackageListForLabelIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.RetrievePackageList("test", "testing"));
        }

        [Test]
        public void RetrieveFileTransferIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.RetrieveFileTransfer("test", "testing"));
        }

        [Test]
        public void GetFreeDiskSpaceIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetFreeDiskSpace());
        }

        [Test]
        public void GetLinkedSiteIdIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.GetLinkedSiteId("test", "testing"));
        }

        [Test]
        public void ListServersIsNotImplemented()
        {
            var client = new TestClient();
            Assert.Throws<NotImplementedException>(() => client.ListServers());
        }
        #endregion

        #region Classes
        private class TestClient
            : CruiseServerClientBase
        {
            public List<NameValuePair> Credentials { get; set; }
            public bool LoggedOut { get; set; }

            public override bool Login(List<NameValuePair> Credentials)
            {
                this.Credentials = Credentials;
                this.SessionToken = "1234";
                return true;
            }

            public override void Logout()
            {
                this.LoggedOut = true;
            }

            public override string TargetServer
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public override bool IsBusy
            {
                get { throw new NotImplementedException(); }
            }

            public override string Address
            {
                get { throw new NotImplementedException(); }
            }
        }
        #endregion
    }
}
