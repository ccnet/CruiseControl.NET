using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.Configuration;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public class ServerAggregatingCruiseManagerWrapper : ICruiseManagerWrapper, IFarmService
	{
		private readonly ICruiseManagerFactory managerFactory;
		private readonly IRemoteServicesConfiguration configuration;

		public ServerAggregatingCruiseManagerWrapper(IRemoteServicesConfiguration configuration, ICruiseManagerFactory managerFactory)
		{
			this.configuration = configuration;
			this.managerFactory = managerFactory;
		}

        public IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            DataResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetLatestBuildName(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
			return new DefaultBuildSpecifier(projectSpecifier, response.Data);
		}

        public string GetLog(IBuildSpecifier buildSpecifier, string sessionToken)
		{
            BuildRequest request = new BuildRequest(sessionToken, buildSpecifier.ProjectSpecifier.ProjectName);
            request.BuildName = buildSpecifier.BuildName;
            DataResponse response = GetCruiseManager(buildSpecifier)
                .GetLog(request);
            ValidateResponse(response);
            return response.Data;
		}

        public IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            DataListResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetBuildNames(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return CreateBuildSpecifiers(projectSpecifier, response.Data.ToArray());
		}

        public IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount, string sessionToken)
		{
            BuildListRequest request = new BuildListRequest(sessionToken, projectSpecifier.ProjectName);
            request.NumberOfBuilds = buildCount;
            DataListResponse response = GetCruiseManager(projectSpecifier)
                .GetMostRecentBuildNames(request);
            ValidateResponse(response);
            return CreateBuildSpecifiers(projectSpecifier, response.Data.ToArray());
		}

		private IBuildSpecifier[] CreateBuildSpecifiers(IProjectSpecifier projectSpecifier, string[] buildNames)
		{
			ArrayList buildSpecifiers = new ArrayList();
			foreach (string buildName in buildNames)
			{
				buildSpecifiers.Add(new DefaultBuildSpecifier(projectSpecifier, buildName));
			}
			return (IBuildSpecifier[]) buildSpecifiers.ToArray(typeof (IBuildSpecifier));
		}

        public void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment, string sessionToken)
		{
            ChangeConfigurationRequest request = new ChangeConfigurationRequest(sessionToken,
                projectSpecifier.ProjectName);
            request.PurgeArtifactDirectory = purgeArtifactDirectory;
            request.PurgeSourceControlEnvironment = purgeSourceControlEnvironment;
            request.PurgeWorkingDirectory = purgeWorkingDirectory;
            Response response = GetCruiseManager(projectSpecifier)
                .DeleteProject(request);
            ValidateResponse(response);
		}

        public void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            ForceBuild(projectSpecifier, sessionToken, new Dictionary<string, string>());
		}

        public void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, Dictionary<string, string> parameters)
        {
            BuildIntegrationRequest request = new BuildIntegrationRequest();
            request.SessionToken = sessionToken;
            request.ProjectName = projectSpecifier.ProjectName;
            request.BuildValues = NameValuePair.FromDictionary(parameters);
            var manager = GetCruiseManager(projectSpecifier.ServerSpecifier);
            var response = manager.ForceBuild(request);
            ValidateResponse(response);
        }

        public void AbortBuild(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            ProjectRequest request = GenerateProjectRequest(projectSpecifier, sessionToken);
            ValidateResponse(GetCruiseManager(projectSpecifier.ServerSpecifier).AbortBuild(request));
		}

        private string GetServerUrl(IServerSpecifier serverSpecifier)
        {
            var locations = ServerLocations;
            foreach (var serverLocation in locations)
            {
                if (StringUtil.EqualsIgnoreCase(serverLocation.Name, serverSpecifier.ServerName))
                {
                    return serverLocation.Url;
                }
            }

            throw new UnknownServerException(serverSpecifier.ServerName);
        }

        public ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            ExternalLinksListResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetExternalLinks(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.ExternalLinks.ToArray();
        }

		public ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(string sessionToken)
        {
			return GetProjectStatusListAndCaptureExceptions(GetServerSpecifiers(), sessionToken);
        }

        public ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier, string sessionToken)
				{
			return GetProjectStatusListAndCaptureExceptions(new IServerSpecifier[] {serverSpecifier}, sessionToken);
			}

        private ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier[] serverSpecifiers, string sessionToken)
        {
            ArrayList projectStatusOnServers = new ArrayList();
            ArrayList exceptions = new ArrayList();

            foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
            {
                try
                {
                    var manager = GetCruiseManager(serverSpecifier);
                    foreach (ProjectStatus projectStatus in manager
                        .GetProjectStatus(new ServerRequest(sessionToken))
                        .Projects)
                    {
                        projectStatusOnServers.Add(new ProjectStatusOnServer(projectStatus, serverSpecifier));
                    }
                }
                catch (SocketException)
                {
                    AddException(exceptions, serverSpecifier, new CruiseControlException("Unable to connect to CruiseControl.NET server.  Please either start the server or check the url."));
                }
                catch (Exception e)
                {
                    AddException(exceptions, serverSpecifier, e);
                }
            }

            return new ProjectStatusListAndExceptions((ProjectStatusOnServer[])projectStatusOnServers.ToArray(typeof(ProjectStatusOnServer)),
                                                      (CruiseServerException[])exceptions.ToArray(typeof(CruiseServerException)));
        }

		private void AddException(ArrayList exceptions, IServerSpecifier serverSpecifier, Exception e)
		{
			exceptions.Add(new CruiseServerException(serverSpecifier.ServerName, GetServerUrl(serverSpecifier), e));
		}


        public string GetServerLog(IServerSpecifier serverSpecifier, string sessionToken)
		{
            DataResponse response = GetCruiseManager(serverSpecifier)
                .GetServerLog(new ServerRequest(sessionToken));
            ValidateResponse(response);
            return response.Data;
		}

        public string GetServerLog(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            DataResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetServerLog(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.Data;
		}

        public void Start(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            ProjectRequest request = GenerateProjectRequest(projectSpecifier, sessionToken);
            ValidateResponse(GetCruiseManager(projectSpecifier.ServerSpecifier).Start(request));
		}

        public void Stop(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            ProjectRequest request = GenerateProjectRequest(projectSpecifier, sessionToken);
            ValidateResponse(GetCruiseManager(projectSpecifier.ServerSpecifier).Stop(request));
		}

		public string GetServerVersion(IServerSpecifier serverSpecifier)
		{
            DataResponse response = GetCruiseManager(serverSpecifier)
                .GetServerVersion(new ServerRequest());
            ValidateResponse(response);
            return response.Data;
		}

		public IServerSpecifier[] GetServerSpecifiers()
		{
			ArrayList serverSpecifiers = new ArrayList();
			foreach (ServerLocation serverLocation in ServerLocations)
			{
				serverSpecifiers.Add(new DefaultServerSpecifier(serverLocation.Name, serverLocation.AllowForceBuild, serverLocation.AllowStartStopBuild));
			}
			return (IServerSpecifier[]) serverSpecifiers.ToArray(typeof (IServerSpecifier));
		}

        public void AddProject(IServerSpecifier serverSpecifier, string serializedProject, string sessionToken)
		{
            ChangeConfigurationRequest request = new ChangeConfigurationRequest(sessionToken, null);
            request.ProjectDefinition = serializedProject;
            Response response = GetCruiseManager(serverSpecifier)
                .AddProject(request);
            ValidateResponse(response);
		}

        public string GetProject(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            DataResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetProject(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.Data;
		}

        public void UpdateProject(IProjectSpecifier projectSpecifier, string serializedProject, string sessionToken)
		{
            ChangeConfigurationRequest request = new ChangeConfigurationRequest(sessionToken, 
                projectSpecifier.ProjectName);
            request.ProjectDefinition = serializedProject;
            Response response = GetCruiseManager(projectSpecifier)
                .UpdateProject(request);
            ValidateResponse(response);
		}

        public string GetArtifactDirectory(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            DataResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetArtifactDirectory(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.Data;
		}

        public string GetStatisticsDocument(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            DataResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetStatisticsDocument(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.Data;
		}

        public string GetModificationHistoryDocument(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            DataResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetModificationHistoryDocument(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.Data;
        }

        public string GetRSSFeed(IProjectSpecifier projectSpecifier)
        {
            DataResponse response = GetCruiseManager(projectSpecifier.ServerSpecifier)
                .GetRSSFeed(GenerateProjectRequest(projectSpecifier, null));
            ValidateResponse(response);
            return response.Data;
        }

		private ICruiseServerClient GetCruiseManager(IBuildSpecifier buildSpecifier)
		{
			return GetCruiseManager(buildSpecifier.ProjectSpecifier);
		}

		private ICruiseServerClient GetCruiseManager(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier.ServerSpecifier);
		}

		private ICruiseServerClient GetCruiseManager(IServerSpecifier serverSpecifier)
		{
            var uri = GetServerUrl(serverSpecifier);
			var manager = managerFactory.GetCruiseServerClient(uri);
            return manager;
		}

		private ServerLocation[] ServerLocations
        {
			get { return configuration.Servers; }
        }

        public IServerSpecifier GetServerConfiguration(string serverName)
        {
            foreach (ServerLocation serverLocation in ServerLocations)
            {
                if (serverLocation.ServerName == serverName)
                    return serverLocation;
            }
            return null;
        }

        public CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(string sessionToken)
        {
            return GetCruiseServerSnapshotListAndExceptions(GetServerSpecifiers(), sessionToken);
        }

        public CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier serverSpecifier, string sessionToken)
        {
            return GetCruiseServerSnapshotListAndExceptions(new IServerSpecifier[] { serverSpecifier }, sessionToken);
        }

        public string Login(string server, LoginRequest credentials)
		{
            return GetCruiseManager(GetServerConfiguration(server)).Login(credentials).SessionToken;
		}

        public void Logout(string server, string sessionToken)
			{
            GetCruiseManager(GetServerConfiguration(server)).Logout(
                new ServerRequest(sessionToken));
		}

        /// <summary>
        /// Changes the current user's password.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="sessionToken"></param>
        /// <param name="oldPassword"></param>
        /// <param name="newPassword"></param>
        public void ChangePassword(string server, string sessionToken, string oldPassword, string newPassword)
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.SessionToken = sessionToken;
            request.OldPassword = oldPassword;
            request.NewPassword = newPassword;
            ValidateResponse(
                GetCruiseManager(GetServerConfiguration(server))
                .ChangePassword(request));
        }

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="sessionToken"></param>
        /// <param name="userName"></param>
        /// <param name="newPassword"></param>
        public virtual void ResetPassword(string server, string sessionToken, string userName, string newPassword)
        {
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.SessionToken = sessionToken;
            request.UserName = userName;
            request.NewPassword = newPassword;
            ValidateResponse(
                GetCruiseManager(GetServerConfiguration(server))
                .ResetPassword(request));
        }

        private CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier[] serverSpecifiers, string sessionToken)
        {
            ArrayList cruiseServerSnapshotsOnServers = new ArrayList();
            ArrayList exceptions = new ArrayList();

            foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
            {
                try
                {
                    SnapshotResponse response = GetCruiseManager(serverSpecifier).GetCruiseServerSnapshot(
                        new ServerRequest(sessionToken));
                    ValidateResponse(response);
                    cruiseServerSnapshotsOnServers.Add(
                        new CruiseServerSnapshotOnServer(response.Snapshot, serverSpecifier));
                }
                catch (SocketException)
                {
                    AddException(exceptions, serverSpecifier, new CruiseControlException("Unable to connect to CruiseControl.NET server.  Please either start the server or check the url."));
                }
                catch (Exception e)
                {
                    AddException(exceptions, serverSpecifier, e);
                }
            }

            return new CruiseServerSnapshotListAndExceptions(
                (CruiseServerSnapshotOnServer[])cruiseServerSnapshotsOnServers.ToArray(typeof(CruiseServerSnapshotOnServer)),
                (CruiseServerException[])exceptions.ToArray(typeof(CruiseServerException)));
        }

        /// <summary>
        /// Retrieves the configuration for security on a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to get the configuration from.</param>
        /// <returns>An XML fragment containing the security configuration.</returns>
        public virtual string GetServerSecurity(IServerSpecifier serverSpecifier, string sessionToken)
        {
            return GetCruiseManager(serverSpecifier)
                .GetSecurityConfiguration(new ServerRequest(sessionToken))
                .Data;
        }

        /// <summary>
        /// Lists all the users who have been defined on a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to get the users from.</param>
        /// <returns>
        /// A list of <see cref="UserNameCredentials"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListAllUsers(IServerSpecifier serverSpecifier, string sessionToken)
        {
            ListUsersResponse response = GetCruiseManager(serverSpecifier)
                .ListUsers(new ServerRequest(sessionToken));
            ValidateResponse(response);
            return response.Users;
        }

        /// <summary>
        /// Checks the security permissions for a user for a project.
        /// </summary>
        /// <param name="projectSpecifier">The project to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IProjectSpecifier projectSpecifier, string sessionToken, string userName)
        {
            DiagnoseSecurityRequest request = new DiagnoseSecurityRequest();
            request.SessionToken = sessionToken;
            request.UserName = userName;
            request.Projects.Add(projectSpecifier.ProjectName);
            DiagnoseSecurityResponse response = GetCruiseManager(projectSpecifier).DiagnoseSecurityPermissions(request);
            ValidateResponse(response);
            return response.Diagnostics;
        }

        /// <summary>
        /// Checks the security permissions for a user for a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IServerSpecifier serverSpecifier, string sessionToken, string userName)
        {
            DiagnoseSecurityRequest request = new DiagnoseSecurityRequest();
            request.SessionToken = sessionToken;
            request.UserName = userName;
            request.Projects.Add(string.Empty);
            DiagnoseSecurityResponse response = GetCruiseManager(serverSpecifier).DiagnoseSecurityPermissions(request);
            ValidateResponse(response);
            return response.Diagnostics;
        }

        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="projectSpecifier">The project to check.</param>
        /// <returns>The list of parameters (if any).</returns>
        public virtual List<ParameterBase> ListBuildParameters(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            return GetCruiseManager(projectSpecifier)
                .ListBuildParameters(GenerateProjectRequest(projectSpecifier, sessionToken))
                .Parameters;
        }
  
        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords)
        {
            ReadAuditRequest request = new ReadAuditRequest();
            request.SessionToken = sessionToken;
            request.StartRecord = startPosition;
            request.NumberOfRecords = numberOfRecords;
            ReadAuditResponse response = GetCruiseManager(serverSpecifier).ReadAuditRecords(request);
            ValidateResponse(response);
            return response.Records;
        }

        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords, AuditFilterBase filter)
        {
            ReadAuditRequest request = new ReadAuditRequest();
            request.SessionToken = sessionToken;
            request.StartRecord = startPosition;
            request.NumberOfRecords = numberOfRecords;
            request.Filter = filter;
            ReadAuditResponse response = GetCruiseManager(serverSpecifier).ReadAuditRecords(request);
            ValidateResponse(response);
            return response.Records;
        }

        /// <summary>
        /// Generates a project request to send to a remote server.
        /// </summary>
        /// <param name="sessionToken">The sesison token to use (optional).</param>
        /// <returns>The complete request.</returns>
        private ProjectRequest GenerateProjectRequest(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            ProjectRequest request = new ProjectRequest();
            request.SessionToken = sessionToken;
            request.ProjectName = projectSpecifier.ProjectName;
            return request;
        }

        /// <summary>
        /// Validates that the request processed ok.
        /// </summary>
        /// <param name="value">The response to check.</param>
        private void ValidateResponse(Response value)
        {
            if (value.Result == ResponseResult.Failure)
            {
                if (value.ErrorMessages.Count == 1)
                {
                    ErrorMessage message = value.ErrorMessages[0];
                    switch (message.Type)
                    {
                        case "SessionInvalidException":
                            throw new SessionInvalidException(message.Message);
                        case "PermissionDeniedException":
                            throw new PermissionDeniedException(message.Message);
                        default:
                            throw new CruiseControlException(message.Message);
                    }
                }
                else
                {
                    string message = "Request request has failed on the remote server:" + Environment.NewLine +
                        value.ConcatenateErrors();
                    throw new CruiseControlException(message);
                }
                }
            }

        /// <summary>
        /// Processes a message for a server.
        /// </summary>
        /// <param name="serverSpecifer">The server.</param>
        /// <param name="action">The action.</param>
        /// <param name="message">The message.</param>
        /// <returns>The response.</returns>
        public string ProcessMessage(IServerSpecifier serverSpecifer, string action, string message)
        {
            string response = GetCruiseManager(serverSpecifer)
                .ProcessMessage(action, message);
            return response;
        }

        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public virtual long GetFreeDiskSpace(IServerSpecifier serverSpecifier)
        {
            DataResponse response = GetCruiseManager(serverSpecifier).GetFreeDiskSpace(new ServerRequest());
            ValidateResponse(response);
            return Convert.ToInt64(response.Data);
        }

        #region RetrieveFileTransfer()
        public virtual RemotingFileTransfer RetrieveFileTransfer(IProjectSpecifier projectSpecifier, string fileName, string sessionToken)
        {
            return GetCruiseManager(projectSpecifier).RetrieveFileTransfer(projectSpecifier.ProjectName, fileName);
        }

        public virtual RemotingFileTransfer RetrieveFileTransfer(IBuildSpecifier buildSpecifier, string fileName, string sessionToken)
        {
            var logFile = new LogFile(buildSpecifier.BuildName);
            var fullName = string.Format("{0}\\{1}", logFile.Label, fileName);
            RemotingFileTransfer fileTransfer = GetCruiseManager(buildSpecifier)
                .RetrieveFileTransfer(buildSpecifier.ProjectSpecifier.ProjectName, fullName);
            return fileTransfer;
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// List the available packages for a project.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            var response = GetCruiseManager(projectSpecifier).RetrievePackageList(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.Packages.ToArray();
        }

        /// <summary>
        /// List the available packages for a build.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList(IBuildSpecifier buildSpecifier, string sessionToken)
        {
            var logFile = new LogFile(buildSpecifier.BuildName);
            var request = new BuildRequest(sessionToken, buildSpecifier.ProjectSpecifier.ProjectName);
            request.BuildName = logFile.Label;
            var response = GetCruiseManager(buildSpecifier).RetrievePackageList(request);
            ValidateResponse(response);
            return response.Packages.ToArray();
        }
        #endregion

        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        public ProjectStatusSnapshot TakeStatusSnapshot(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            var response = GetCruiseManager(projectSpecifier).TakeStatusSnapshot(GenerateProjectRequest(projectSpecifier, sessionToken));
            ValidateResponse(response);
            return response.Snapshot;
        }
    }
}
