using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using System.Xml;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A client connection to a remote CruiseControl.Net server.
    /// </summary>
    public class CruiseServerClient
        : CruiseServerClientBase
    {
        #region Private fields
        private readonly IServerConnection connection;
        private string targetServer;
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
        public override string TargetServer
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

        #region IsBusy
        /// <summary>
        /// Is this client busy performing an operation.
        /// </summary>
        public override bool IsBusy
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

        #region Address
        /// <summary>
        /// The address of the client.
        /// </summary>
        public override string Address
        {
            get { return connection.Address; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public override ProjectStatus[] GetProjectStatus()
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
        public override void ForceBuild(string projectName)
        {
            Response resp = connection.SendMessage("ForceBuild", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }

        /// <summary>
        /// Forces a build for the named project with some parameters.
        /// </summary>
        /// <param name="projectName">project to force</param>
        /// <param name="parameters"></param>
        public override void ForceBuild(string projectName, List<NameValuePair> parameters)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

            BuildIntegrationRequest request = new BuildIntegrationRequest(SessionToken, projectName);
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
        public override void AbortBuild(string projectName)
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
        public override void Request(string projectName, IntegrationRequest integrationRequest)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

            BuildIntegrationRequest request = new BuildIntegrationRequest(SessionToken, projectName);
            request.BuildCondition = integrationRequest.BuildCondition;
            Response resp = connection.SendMessage("ForceBuild", request);
            ValidateResponse(resp);
        }
        #endregion

        #region StartProject()
        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="projectName"></param>
        public override void StartProject(string projectName)
        {
            Response resp = connection.SendMessage("Start", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }
        #endregion

        #region StopProject()
        /// <summary>
        /// Stop project.
        /// </summary>
        /// <param name="projectName"></param>
        public override void StopProject(string projectName)
        {
            Response resp = connection.SendMessage("Stop", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Sends a message for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="message"></param>
        public override void SendMessage(string projectName, Message message)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

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
        public override void WaitForExit(string projectName)
        {
            Response resp = connection.SendMessage("WaitForExit", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }
        #endregion

        #region CancelPendingRequest()
        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        public override void CancelPendingRequest(string projectName)
        {
            Response resp = connection.SendMessage("CancelPendingRequest", GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        public override CruiseServerSnapshot GetCruiseServerSnapshot()
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
        public override string GetLatestBuildName(string projectName)
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
        public override string[] GetBuildNames(string projectName)
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
        public override string[] GetMostRecentBuildNames(string projectName, int buildCount)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

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
        public override string GetLog(string projectName, string buildName)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

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
        public override string GetServerLog()
        {
            DataResponse resp = ValidateResponse(
                connection.SendMessage("GetServerLog", GenerateServerRequest()))
                as DataResponse;
            return resp.Data;
        }

        /// <summary>
        /// Returns a log of recent build server activity for a specific project. How much information that is returned is configured on the build server.
        /// </summary>
        public override string GetServerLog(string projectName)
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
        public override string GetServerVersion()
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
        public override void AddProject(string serializedProject)
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
        public override void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

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
        public override string GetProject(string projectName)
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
        public override void UpdateProject(string projectName, string serializedProject)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

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
        public override ExternalLink[] GetExternalLinks(string projectName)
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
        public override string GetArtifactDirectory(string projectName)
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
        public override string GetStatisticsDocument(string projectName)
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
        public override string GetModificationHistoryDocument(string projectName)
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
        public override string GetRSSFeed(string projectName)
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
        public override bool Login(List<NameValuePair> Credentials)
        {
            SessionToken = null;

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
                SessionToken = resp.SessionToken;
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
        public override void Logout()
        {
            if (SessionToken != null)
            {
                ValidateResponse(
                    connection.SendMessage("Logout",
                        GenerateServerRequest()));
                SessionToken = null;
            }
        }
        #endregion

        #region GetSecurityConfiguration()
        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        public override string GetSecurityConfiguration()
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
        public override List<UserDetails> ListUsers()
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
        public override List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string userName, params string[] projects)
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
        public override List<AuditRecord> ReadAuditRecords(int startRecord, int numberOfRecords)
        {
            return ReadAuditRecords(startRecord, numberOfRecords, null);
        }

        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public override List<AuditRecord> ReadAuditRecords(int startRecord, int numberOfRecords, AuditFilterBase filter)
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
        public override List<ParameterBase> ListBuildParameters(string projectName)
        {
            BuildParametersResponse resp = ValidateResponse(
                connection.SendMessage("ListBuildParameters", GenerateProjectRequest(projectName)))
                as BuildParametersResponse;
            return resp.Parameters;
        }
        #endregion

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        public override void ChangePassword(string oldPassword, string newPassword)
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
        public override void ResetPassword(string userName, string newPassword)
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.ServerName = TargetServer;
            request.UserName = userName;
            request.NewPassword = newPassword;
            ValidateResponse(
                connection.SendMessage("ResetPassword", request));
        }
        #endregion

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a snapshot of the current project status.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The current status snapshot.</returns>
        public override ProjectStatusSnapshot TakeStatusSnapshot(string projectName)
        {
            var request = GenerateProjectRequest(projectName);
            var response = connection.SendMessage("TakeStatusSnapshot", request);
            ValidateResponse(response);
            return (response as StatusSnapshotResponse).Snapshot;
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of packages for a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The currently available packages.</returns>
        public override List<PackageDetails> RetrievePackageList(string projectName)
        {
            var request = GenerateProjectRequest(projectName);
            var response = connection.SendMessage("RetrievePackageList", request);
            ValidateResponse(response);
            return (response as ListPackagesResponse).Packages;
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieves a file transfer instance.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file transfer instance.</returns>
        public override IFileTransfer RetrieveFileTransfer(string projectName, string fileName)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

            var request = new FileTransferRequest(SessionToken, projectName, fileName);
            var response = connection.SendMessage("RetrieveFileTransfer", request);
            ValidateResponse(response);
            return (response as FileTransferResponse).FileTransfer;
        }
        #endregion

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public override long GetFreeDiskSpace()
        {
            var request = GenerateServerRequest();
            var response = connection.SendMessage("GetFreeDiskSpace", request);
            ValidateResponse(response);
            return Convert.ToInt64((response as DataResponse).Data);
        }
        #endregion

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieve the identifer for this project on a linked site.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public override string GetLinkedSiteId(string projectName, string siteName)
        {
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

            var request = new ProjectItemRequest(SessionToken, projectName);
            request.ItemName = siteName;
            var response = connection.SendMessage("GetLinkedSiteId", request);
            ValidateResponse(response);
            return (response as DataResponse).Data;
        }
        #endregion

        #region ProcessMessage()
        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message.</param>
        /// <returns>The response message.</returns>
        public override Response ProcessMessage(string action, ServerRequest message)
        {
            Response response = connection.SendMessage(action, message);
            return response;
        }

        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message in an XML format.</param>
        /// <returns>The response message in an XML format.</returns>
        public override string ProcessMessage(string action, string message)
        {
            Response response = new Response();

            try
            {
                // Find the type of message
                var messageXml = new XmlDocument();
                messageXml.LoadXml(message);
                var messageType = XmlConversionUtil.FindMessageType(messageXml.DocumentElement.Name);
                if (messageType == null)
                {
                    response.Result = ResponseResult.Failure;
                    response.ErrorMessages.Add(
                        new ErrorMessage(
                            string.Format(
                                "Unable to translate message: '{0}' is unknown",
                                messageXml.DocumentElement.Name)));
                }

                // Convert the message and invoke the action
                var request = XmlConversionUtil.ConvertXmlToObject(messageType, message);
                response = connection.SendMessage(action, request as ServerRequest);
            }
            catch (Exception error)
            {
                response.Result = ResponseResult.Failure;
                response.ErrorMessages.Add(
                    new ErrorMessage("Unable to process error: " + error.Message));
            }

            return response.ToString();
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
            ServerRequest request = new ServerRequest(SessionToken);
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
            if (string.IsNullOrEmpty(projectName)) throw new ArgumentNullException("projectName");

            ProjectRequest request = new ProjectRequest(SessionToken, projectName);
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
                var errorType = response.ErrorMessages.Count == 1 ?
                    response.ErrorMessages[0].Type :
                    null;
                throw new CommunicationsException(message, errorType);
            }
            return response;
        }
        #endregion
        #endregion
    }
}
