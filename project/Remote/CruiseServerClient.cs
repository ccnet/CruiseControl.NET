using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A client connection to a remote CruiseControl.Net server.
    /// </summary>
    public class CruiseServerClient
    {
        #region Private fields
        private readonly IServerConnection connection;
        private string targetServer;
        private string sessionToken;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new <see cref="CruiseServerClient"/>.
        /// </summary>
        /// <param name="connection">The client connection to use.</param>
        public CruiseServerClient(IServerConnection connection)
        {
            this.connection = connection;
        }
        #endregion

        #region Public properties
        #region TargetServer
        /// <summary>
        /// The server that will be targeted by all messages.
        /// </summary>
        public string TargetServer
        {
            get
            {
                if (string.IsNullOrEmpty(targetServer))
                {
                    return connection.ServerName;
                }
                else
                {
                    return targetServer;
                }
            }
            set { targetServer = value; }
        }
        #endregion

        #region SessionToken
        /// <summary>
        /// The session token to use.
        /// </summary>
        public string SessionToken
        {
            get { return sessionToken; }
            set { sessionToken = value; }
        }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this client busy performing an operation.
        /// </summary>
        public bool IsBusy
        {
            get { return connection.IsBusy; }
        }
        #endregion

        #region Connection
        /// <summary>
        /// The underlying connection.
        /// </summary>
        public IServerConnection Connection
        {
            get { return connection; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public ProjectStatus[] GetProjectStatus()
        {
            ProjectStatusResponse resp = ValidateResponse(
                connection.SendMessage("GetProjectStatus", GenerateServerRequest()))
                as ProjectStatusResponse;
            return resp.Projects.ToArray();
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="projectName">project to force</param>
        public void ForceBuild(string projectName)
        {
            Response resp = connection.SendMessage("ForceBuild", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }

        /// <summary>
        /// Forces a build for the named project with some parameters.
        /// </summary>
        /// <param name="projectName">project to force</param>
        /// <param name="parameters"></param>
        public void ForceBuild(string projectName, List<NameValuePair> parameters)
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest(sessionToken, projectName);
            request.BuildValues = parameters;
            Response resp = connection.SendMessage("ForceBuild", request);
            ValidateResponse(resp);
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Attempts to abort a current project build.
        /// </summary>
        /// <param name="projectName">The name of the project to abort.</param>
        public void AbortBuild(string projectName)
        {
            Response resp = connection.SendMessage("AbortBuild", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }
        #endregion

        #region Request()
        /// <summary>
        /// Sends a build request to the server.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="integrationRequest"></param>
        public void Request(string projectName, IntegrationRequest integrationRequest)
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest(null, projectName);
            request.BuildCondition = integrationRequest.BuildCondition;
            Response resp = connection.SendMessage("ForceBuild", request);
            ValidateResponse(resp);
        }
        #endregion

        #region StartProject()
        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="project"></param>
        public void StartProject(string project)
        {
            Response resp = connection.SendMessage("Start", GenerateProjectRequest(project));
            ValidateResponse(resp);
        }
        #endregion

        #region StopProject()
        /// <summary>
        /// Stop project.
        /// </summary>
        /// <param name="project"></param>
        public void StopProject(string project)
        {
            Response resp = connection.SendMessage("Stop", GenerateProjectRequest(project));
            ValidateResponse(resp);
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Sends a message for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="message"></param>
        public void SendMessage(string projectName, Message message)
        {
            MessageRequest request = new MessageRequest();
            request.ProjectName = projectName;
            request.Message = message.Text;
            Response resp = connection.SendMessage("SendMessage", request);
            ValidateResponse(resp);
        }
        #endregion

        #region WaitForExit()
        /// <summary>
        /// Waits for a project to stop.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        public void WaitForExit(string projectName)
        {
            Response resp = connection.SendMessage("WaitForExit", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }
        #endregion

        #region CancelPendingRequest()
        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        public void CancelPendingRequest(string projectName)
        {
            Response resp = connection.SendMessage("CancelPendingRequest", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            SnapshotResponse resp = ValidateResponse(
                connection.SendMessage("GetCruiseServerSnapshot", GenerateServerRequest()))
                as SnapshotResponse;
            return resp.Snapshot;
        }
        #endregion

        #region GetLatestBuildName()
        /// <summary>
        /// Returns the name of the most recent build for the specified project
        /// </summary>
        public string GetLatestBuildName(string projectName)
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetLatestBuildName", GenerateProjectRequest(projectName)))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region GetBuildNames()
        /// <summary>
        /// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public string[] GetBuildNames(string projectName)
        {
            DataListResponse resp = ValidateResponse(
                connection.SendMessage("GetBuildNames", GenerateProjectRequest(projectName)))
                as DataListResponse;
            return resp.Data.ToArray();
        }
        #endregion

        #region GetMostRecentBuildNames()
        /// <summary>
        /// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public string[] GetMostRecentBuildNames(string projectName, int buildCount)
        {
            BuildListRequest request = new BuildListRequest(null, projectName);
            request.NumberOfBuilds = buildCount;
            DataListResponse resp = ValidateResponse(
                connection.SendMessage("GetMostRecentBuildNames", request))
                as DataListResponse;
            return resp.Data.ToArray();
        }
        #endregion

        #region GetLog()
        /// <summary>
        /// Returns the build log contents for requested project and build name
        /// </summary>
        public string GetLog(string projectName, string buildName)
        {
            BuildRequest request = new BuildRequest(null, projectName);
            request.BuildName = buildName;
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetLog", request))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
        /// </summary>
        public string GetServerLog()
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetServerLog", GenerateServerRequest()))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Returns a log of recent build server activity for a specific project. How much information that is returned is configured on the build server.
        /// </summary>
        public string GetServerLog(string projectName)
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetServerLog", GenerateProjectRequest(projectName)))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region GetServerVersion()
        /// <summary>
        /// Returns the version of the server
        /// </summary>
        public string GetServerVersion()
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetServerVersion", GenerateServerRequest()))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region AddProject()
        /// <summary>
        /// Adds a project to the server
        /// </summary>
        public void AddProject(string serializedProject)
        {
            ChangeConfigurationRequest request = new ChangeConfigurationRequest();
            request.ProjectDefinition = serializedProject;
            Response resp = connection.SendMessage("AddProject", request);
            ValidateResponse(resp);
        }
        #endregion

        #region DeleteProject()
        /// <summary>
        /// Deletes the specified project from the server
        /// </summary>
        public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            ChangeConfigurationRequest request = new ChangeConfigurationRequest(null, projectName);
            request.PurgeWorkingDirectory = purgeWorkingDirectory;
            request.PurgeArtifactDirectory = purgeArtifactDirectory;
            request.PurgeSourceControlEnvironment = purgeSourceControlEnvironment;
            Response resp = connection.SendMessage("DeleteProject", request);
            ValidateResponse(resp);
        }
        #endregion

        #region GetProject()
        /// <summary>
        /// Returns the serialized form of the requested project from the server
        /// </summary>
        public string GetProject(string projectName)
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetProject", GenerateProjectRequest(projectName)))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region UpdateProject()
        /// <summary>
        /// Updates the selected project on the server
        /// </summary>
        public void UpdateProject(string projectName, string serializedProject)
        {
            ChangeConfigurationRequest request = new ChangeConfigurationRequest(null, projectName);
            request.ProjectDefinition = serializedProject;
            Response resp = connection.SendMessage("UpdateProject", request);
            ValidateResponse(resp);
        }
        #endregion

        #region GetExternalLinks()
        /// <summary>
        /// Retrieves any external links.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public ExternalLink[] GetExternalLinks(string projectName)
        {
            ExternalLinksListResponse resp = ValidateResponse(
                connection.SendMessage("GetExternalLinks", GenerateProjectRequest(projectName)))
                as ExternalLinksListResponse;
            return resp.ExternalLinks.ToArray();
        }
        #endregion

        #region GetArtifactDirectory()
        /// <summary>
        /// Retrieves the name of the artifact directory.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public string GetArtifactDirectory(string projectName)
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetArtifactDirectory", GenerateProjectRequest(projectName)))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region GetStatisticsDocument()
        /// <summary>
        /// Retrieves the statistics document.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public string GetStatisticsDocument(string projectName)
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetStatisticsDocument", GenerateProjectRequest(projectName)))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region GetModificationHistoryDocument()
        /// <summary>
        /// Retrieves a document containing the history of all the changes.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public string GetModificationHistoryDocument(string projectName)
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetModificationHistoryDocument", GenerateProjectRequest(projectName)))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region GetRSSFeed()
        /// <summary>
        /// Retrieves the RSS feed URL.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public string GetRSSFeed(string projectName)
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetRSSFeed", GenerateProjectRequest(projectName)))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region Login()
        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <returns>True if the request is successful, false otherwise.</returns>
        public virtual bool Login(List<NameValuePair> Credentials)
        {
            sessionToken=null;

            // Generate the response and send it
            LoginRequest request = new LoginRequest();
            request.Credentials.AddRange(Credentials);
            request.ServerName = TargetServer;
            LoginResponse resp = ValidateResponse(
                connection.SendMessage("Login", request))
                as LoginResponse;
            ValidateResponse(resp);

            // Check the results
            if (!string.IsNullOrEmpty(resp.SessionToken))
            {
                sessionToken = resp.SessionToken;
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Logout()
        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        public virtual void Logout()
        {
            sessionToken = null;
            ValidateResponse(
                connection.SendMessage("Logout",
                    GenerateServerRequest()));
        }
        #endregion

        #region GetSecurityConfiguration()
        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        public virtual string GetSecurityConfiguration()
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetSecurityConfiguration", GenerateServerRequest()))
                as DataResponse;
            return resp.Data;
        }
        #endregion

        #region ListUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListUsers()
        {
            ListUsersResponse resp = ValidateResponse(
                connection.SendMessage("ListUsers", GenerateServerRequest()))
                as ListUsersResponse;
            return resp.Users;
        }
        #endregion

        #region DiagnoseSecurityPermissions()
        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string userName, params string[] projects)
        {
            DiagnoseSecurityRequest request = new DiagnoseSecurityRequest();
            request.ServerName = TargetServer;
            request.UserName = userName;
            if (projects != null) request.Projects.AddRange(projects);
            DiagnoseSecurityResponse resp = ValidateResponse(
                connection.SendMessage("DiagnoseSecurityPermissions", request))
                as DiagnoseSecurityResponse;
            return resp.Diagnostics;
        }
        #endregion

        #region ReadAuditRecords()
        /// <summary>
        /// Reads the specified number of audit events.
        /// </summary>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startRecord, int numberOfRecords)
        {
            return ReadAuditRecords(startRecord, numberOfRecords, null);
        }

        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startRecord, int numberOfRecords, AuditFilterBase filter)
        {
            ReadAuditRequest request = new ReadAuditRequest();
            request.ServerName = TargetServer;
            request.StartRecord = startRecord;
            request.NumberOfRecords = numberOfRecords;
            request.Filter = filter;
            ReadAuditResponse resp = ValidateResponse(
                connection.SendMessage("ReadAuditRecords", request))
                as ReadAuditResponse;
            return resp.Records;
        }
        #endregion

        #region ListBuildParameters()
        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to retrieve the parameters for.</param>
        /// <returns>The list of parameters (if any).</returns>
        public virtual List<ParameterBase> ListBuildParameters(string projectName)
        {
            BuildParametersResponse resp = ValidateResponse(
                connection.SendMessage("ListBuildParameters", GenerateProjectRequest(projectName)))
                as BuildParametersResponse;
            return resp.Parameters;
        }
        #endregion

        #region RetrieveCapacities()
        /// <summary>
        /// Retrieves the capacities of the server.
        /// </summary>
        /// <returns>The allowed functions of the server.</returns>
        public virtual List<string> RetrieveCapacities()
        {
            DataListResponse resp = ValidateResponse(
                connection.SendMessage("RetrieveCapacities", GenerateServerRequest()))
                as DataListResponse;
            return resp.Data;
        }
        #endregion

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        public virtual void ChangePassword(string oldPassword, string newPassword)
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.ServerName = TargetServer;
            request.OldPassword = oldPassword;
            request.NewPassword = newPassword;
            ValidateResponse(
                connection.SendMessage("ChangePassword", request));
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        public virtual void ResetPassword(string userName, string newPassword)
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.ServerName = TargetServer;
            request.UserName = userName;
            request.NewPassword = newPassword;
            ValidateResponse(
                connection.SendMessage("ResetPassword", request));
        }
        #endregion
        #endregion

        #region Private methods
        #region GenerateServerRequest()
        /// <summary>
        /// Generates a server request.
        /// </summary>
        /// <returns></returns>
        private ServerRequest GenerateServerRequest()
        {
            ServerRequest request = new ServerRequest(sessionToken);
            request.ServerName = TargetServer;
            return request;
        }
        #endregion

        #region GenerateProjectRequest()
        /// <summary>
        /// Generates a project request.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        private ProjectRequest GenerateProjectRequest(string projectName)
        {
            ProjectRequest request = new ProjectRequest(sessionToken, projectName);
            request.ServerName = TargetServer;
            return request;
        }
        #endregion

        #region ValidateResponse()
        /// <summary>
        /// Validates the response from the server.
        /// </summary>
        /// <param name="response"></param>
        private Response ValidateResponse(Response response)
        {
            if (response.Result == ResponseResult.Failure)
            {
                string message = "Request processing has failed on the remote server:" + Environment.NewLine +
                    response.ConcatenateErrors();
                throw new CommunicationsException(message);
            }
            return response;
        }
        #endregion
        #endregion
    }
}
