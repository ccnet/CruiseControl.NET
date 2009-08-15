using System;
using NUnit.Framework;
using Rhino.Mocks;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.UnitTests.Remote
{
    [TestFixture]
    public class CruiseServerClientTests
    {
        #region Private fields
        private MockRepository mocks = new MockRepository();
        #endregion

        #region Test methods
        #region GetProjectStatus()
        [Test]
        public void GetProjectStatusThrowsExceptionOnFailure()
        {
            ProjectStatusResponse response = new ProjectStatusResponse();
            IServerConnection connection = mocks.DynamicMock<IServerConnection>();
            SetupResult.For(connection.SendMessage("GetProjectStatus", null))
                .IgnoreArguments()
                .Return(response);
            mocks.ReplayAll();

            CruiseServerClient client = new CruiseServerClient(connection);
            Assert.That(delegate { client.GetProjectStatus(); },
                        Throws.TypeOf<CommunicationsException>());
        }

        [Test]
        public void GetProjectStatusReturnsProjects()
        {
            ProjectStatus status = new ProjectStatus("Test project", IntegrationStatus.Success, DateTime.Now);
            ProjectStatusResponse response = new ProjectStatusResponse();
            response.Result = ResponseResult.Success;
            response.Projects.Add(status);
            IServerConnection connection = mocks.DynamicMock<IServerConnection>();
            SetupResult.For(connection.SendMessage("GetProjectStatus", null))
                .IgnoreArguments()
                .Return(response);
            mocks.ReplayAll();

            CruiseServerClient client = new CruiseServerClient(connection);
            ProjectStatus[] results = client.GetProjectStatus();
            Assert.AreEqual(1, results.Length);
            Assert.AreEqual(status, results[0]);
        }

        [Test]
        public void GetProjectStatusSendsRequest()
        {
            ProjectStatus status = new ProjectStatus("Test project", IntegrationStatus.Success, DateTime.Now);
            ProjectStatusResponse response = new ProjectStatusResponse();
            response.Result = ResponseResult.Success;
            response.Projects.Add(status);
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetProjectStatus", typeof(ServerRequest), response));
            client.GetProjectStatus();
        }
        #endregion

        #region ForceBuild()
        [Test]
        public void ForceBuildSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("ForceBuild", typeof(ProjectRequest), "Project #1"));
            client.ForceBuild("Project #1");
        }
        #endregion

        #region AbortBuild()
        [Test]
        public void AbortBuildSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("AbortBuild", typeof(ProjectRequest), "Project #1"));
            client.AbortBuild("Project #1");
        }
        #endregion

        #region Request()
        [Test]
        public void RequestSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("ForceBuild", typeof(BuildIntegrationRequest), "Project #1"));
            client.Request("Project #1", new IntegrationRequest(BuildCondition.ForceBuild, "Me", null));
        }
        #endregion

        #region StartProject()
        [Test]
        public void StartProjectSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("Start", typeof(ProjectRequest), "Project #1"));
            client.StartProject("Project #1");
        }
        #endregion

        #region StopProject()
        [Test]
        public void StopProjectSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("Stop", typeof(ProjectRequest), "Project #1"));
            client.StopProject("Project #1");
        }
        #endregion

        #region SendMessage()
        [Test]
        public void SendMessageSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("SendMessage", typeof(MessageRequest), "Project #1"));
            client.SendMessage("Project #1", new Message("Testing"));
        }
        #endregion

        #region WaitForExit()
        [Test]
        public void WaitForExitSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("WaitForExit", typeof(ProjectRequest), "Project #1"));
            client.WaitForExit("Project #1");
        }
        #endregion

        #region CancelPendingRequest()
        [Test]
        public void CancelPendingRequestSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("CancelPendingRequest", typeof(ProjectRequest), "Project #1"));
            client.CancelPendingRequest("Project #1");
        }
        #endregion

        #region GetCruiseServerSnapshot()
        [Test]
        public void GetCruiseServerSnapshot()
        {
            SnapshotResponse response = new SnapshotResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetCruiseServerSnapshot", typeof(ServerRequest), null, response));
            client.GetCruiseServerSnapshot();
        }
        #endregion

        #region GetLatestBuildName()
        [Test]
        public void GetLatestBuildName()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetLatestBuildName", typeof(ProjectRequest), "Project #1", response));
            string result = client.GetLatestBuildName("Project #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region GetBuildNames()
        [Test]
        public void GetBuildNames()
        {
            DataListResponse response = new DataListResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetBuildNames", typeof(ProjectRequest), "Project #1", response));
            client.GetBuildNames("Project #1");
        }
        #endregion

        #region GetMostRecentBuildNames()
        [Test]
        public void GetMostRecentBuildNames()
        {
            DataListResponse response = new DataListResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetMostRecentBuildNames", typeof(BuildListRequest), "Project #1", response));
            client.GetMostRecentBuildNames("Project #1", 5);
        }
        #endregion

        #region GetLog()
        [Test]
        public void GetLog()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetLog", typeof(BuildRequest), "Project #1", response));
            string result = client.GetLog("Project #1", "Build #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region GetServerLog()
        [Test]
        public void GetServerLogForServer()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetServerLog", typeof(ServerRequest), null, response));
            string result = client.GetServerLog();
            Assert.AreEqual(response.Data, result);
        }

        [Test]
        public void GetServerLogForProject()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetServerLog", typeof(ProjectRequest), "Project #1", response));
            string result = client.GetServerLog("Project #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region GetServerVersion()
        [Test]
        public void GetServerVersion()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetServerVersion", typeof(ServerRequest), null, response));
            string result = client.GetServerVersion();
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region AddProject()
        [Test]
        public void AddProjectSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("AddProject", typeof(ChangeConfigurationRequest)));
            client.AddProject("Project #1");
        }
        #endregion

        #region DeleteProject()
        [Test]
        public void DeleteProjectSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("DeleteProject", typeof(ChangeConfigurationRequest), "Project #1"));
            client.DeleteProject("Project #1", true, true, true);
        }
        #endregion

        #region GetProject()
        [Test]
        public void GetProject()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetProject", typeof(ProjectRequest), "Project #1", response));
            string result = client.GetProject("Project #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region UpdateProject()
        [Test]
        public void UpdateProjectSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("UpdateProject", typeof(ChangeConfigurationRequest), "Project #1"));
            client.UpdateProject("Project #1", "Data");
        }
        #endregion

        #region GetExternalLinks()
        [Test]
        public void GetExternalLinks()
        {
            ExternalLinksListResponse response = new ExternalLinksListResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetExternalLinks", typeof(ProjectRequest), "Project #1", response));
            client.GetExternalLinks("Project #1");
        }
        #endregion

        #region GetArtifactDirectory()
        [Test]
        public void GetArtifactDirectory()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetArtifactDirectory", typeof(ProjectRequest), "Project #1", response));
            string result = client.GetArtifactDirectory("Project #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region GetStatisticsDocument()
        [Test]
        public void GetStatisticsDocument()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetStatisticsDocument", typeof(ProjectRequest), "Project #1", response));
            string result = client.GetStatisticsDocument("Project #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region GetModificationHistoryDocument()
        [Test]
        public void GetModificationHistoryDocument()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetModificationHistoryDocument", typeof(ProjectRequest), "Project #1", response));
            string result = client.GetModificationHistoryDocument("Project #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region GetRSSFeed()
        [Test]
        public void GetRSSFeed()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetRSSFeed", typeof(ProjectRequest), "Project #1", response));
            string result = client.GetRSSFeed("Project #1");
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region Login()
        [Test]
        public void LoginIsSuccessful()
        {
            LoginResponse response = new LoginResponse();
            response.Result = ResponseResult.Success;
            response.SessionToken = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("Login", typeof(LoginRequest), null, response));
            List<NameValuePair> credentials = new List<NameValuePair>();
            bool result = client.Login(credentials);
            Assert.IsTrue(result);
            Assert.AreEqual(response.SessionToken, client.SessionToken);
        }

        [Test]
        public void LoginIsFailure()
        {
            LoginResponse response = new LoginResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("Login", typeof(LoginRequest), null, response));
            List<NameValuePair> credentials = new List<NameValuePair>();
            bool result = client.Login(credentials);
            Assert.IsFalse(result);
            Assert.AreEqual(null, client.SessionToken);
        }
        #endregion

        #region Logout()
        [Test]
        public void Logout()
        {
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("Logout", typeof(ServerRequest)));
            client.Logout();
            Assert.AreEqual(null, client.SessionToken);
        }
        #endregion

        #region GetSecurityConfiguration()
        [Test]
        public void GetSecurityConfiguration()
        {
            DataResponse response = new DataResponse();
            response.Result = ResponseResult.Success;
            response.Data = "Some data";
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("GetSecurityConfiguration", typeof(ServerRequest), null, response));
            string result = client.GetSecurityConfiguration();
            Assert.AreEqual(response.Data, result);
        }
        #endregion

        #region ListUsers()
        [Test]
        public void ListUsers()
        {
            ListUsersResponse response = new ListUsersResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("ListUsers", typeof(ServerRequest), null, response));
            client.ListUsers();
        }
        #endregion

        #region DiagnoseSecurityPermissions()
        [Test]
        public void DiagnoseSecurityPermissions()
        {
            DiagnoseSecurityResponse response = new DiagnoseSecurityResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("DiagnoseSecurityPermissions", typeof(DiagnoseSecurityRequest), null, response));
            client.DiagnoseSecurityPermissions("johnDoe");
        }
        #endregion

        #region ReadAuditRecords()
        [Test]
        public void ReadAuditRecordsWithoutFilter()
        {
            ReadAuditResponse response = new ReadAuditResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("ReadAuditRecords", typeof(ReadAuditRequest), null, response));
            client.ReadAuditRecords(0, 10);
        }

        [Test]
        public void ReadAuditRecordsWithFilter()
        {
            ReadAuditResponse response = new ReadAuditResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("ReadAuditRecords", typeof(ReadAuditRequest), null, response));
            AuditFilterBase filter = AuditFilters.ByProject("Project #1");
            client.ReadAuditRecords(0, 10, filter);
        }
        #endregion

        #region ListBuildParameters()
        [Test]
        public void ListBuildParameters()
        {
            BuildParametersResponse response = new BuildParametersResponse();
            response.Result = ResponseResult.Success;
            CruiseServerClient client = new CruiseServerClient(
                new ServerStub("ListBuildParameters", typeof(ProjectRequest), "Project #1", response));
            client.ListBuildParameters("Project #1");
        }
        #endregion

        #region ChangePassword()
        [Test]
        public void ChangePasswordSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("ChangePassword", typeof(ChangePasswordRequest), "Project #1"));
            client.ChangePassword("oldPassword", "newPassword");
        }
        #endregion

        #region ResetPassword()
        [Test]
        public void ResetPasswordSendsRequest()
        {
            CruiseServerClient client = new CruiseServerClient(new ServerStub("ResetPassword", typeof(ChangePasswordRequest), "Project #1"));
            client.ResetPassword("userName", "newPassword");
        }
        #endregion

        #region SessionToken
        [Test]
        public void SessionTokenIsSetCorrectly()
        {
            CruiseServerClient client = new CruiseServerClient(null);
            client.SessionToken = "sessionId";
            Assert.AreEqual("sessionId", client.SessionToken);
        }
        #endregion

        #region IsBusy
        [Test]
        public void IsBusyReturnsUnderlyingConnectionIsBusy()
        {
            IServerConnection connection = mocks.DynamicMock<IServerConnection>();
            SetupResult.For(connection.IsBusy).Return(true);
            mocks.ReplayAll();

            CruiseServerClient client = new CruiseServerClient(connection);
            Assert.IsTrue(client.IsBusy);
        }
        #endregion
        #endregion

        #region Private classes
        #region ServerStub
        private class ServerStub
            : IServerConnection
        {
            private string action;
            private Type message;
            private string projectName;
            private Response response;

            public ServerStub(string action, Type message)
                : this(action, message, null, null)
            {
            }

            public ServerStub(string action, Type message, Response response)
                : this(action, message, null, response)
            {
            }

            public ServerStub(string action, Type message, string projectName)
                : this(action, message, projectName, null)
            {
            }

            public ServerStub(string action, Type message, string projectName, Response response)
            {
                this.action = action;
                this.message = message;
                this.projectName = projectName;
                this.response = response;
            }

            #region IServerConnection Members

            public string Type
            {
                get { throw new NotImplementedException(); }
            }

            public string ServerName
            {
                get { return "serverName"; }
            }

            public bool IsBusy
            {
                get { throw new NotImplementedException(); }
            }

            public string Address
            {
                get { return "Address"; }
            }

            public Response SendMessage(string action, ServerRequest request)
            {
                Assert.AreEqual(this.action, action);
                Assert.AreEqual(this.message, request.GetType());
                if (!string.IsNullOrEmpty(projectName) && (request is ProjectRequest))
                {
                    Assert.AreEqual(this.projectName, (request as ProjectRequest).ProjectName);
                }
                if (response == null)
                {
                    Response result = new Response(request);
                    result.Result = ResponseResult.Success;
                    return result;
                }
                else
                {
                    return response;
                }
            }

            public void SendMessageAsync(string action, ServerRequest request)
            {
                throw new NotImplementedException();
            }

            public void SendMessageAsync(string action, ServerRequest request, object userState)
            {
                throw new NotImplementedException();
            }

            public void CancelAsync()
            {
                throw new NotImplementedException();
            }

            public void CancelAsync(object userState)
            {
                throw new NotImplementedException();
            }

            public event EventHandler<MessageReceivedEventArgs> SendMessageCompleted;

            public event EventHandler<CommunicationsEventArgs> RequestSending;

            public event EventHandler<CommunicationsEventArgs> ResponseReceived;

            #endregion
        }
        #endregion
        #endregion
    }
}
