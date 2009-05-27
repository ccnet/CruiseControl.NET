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
        private readonly ICruiseServerClientFactory clientFactory;
		private readonly IRemoteServicesConfiguration configuration;

        public ServerAggregatingCruiseManagerWrapper(IRemoteServicesConfiguration configuration, ICruiseServerClientFactory managerFactory)
		{
			this.configuration = configuration;
			this.clientFactory = managerFactory;
		}

        public IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(projectSpecifier.ServerSpecifier, sessionToken)
                .GetLatestBuildName(projectSpecifier.ProjectName);
			return new DefaultBuildSpecifier(projectSpecifier, response);
		}

        public string GetLog(IBuildSpecifier buildSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(buildSpecifier, sessionToken)
                .GetLog(buildSpecifier.ProjectSpecifier.ProjectName, buildSpecifier.BuildName);
            return response;
		}

        public IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(projectSpecifier.ServerSpecifier, sessionToken)
                .GetBuildNames(projectSpecifier.ProjectName);
            return CreateBuildSpecifiers(projectSpecifier, response);
		}

        public IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount, string sessionToken)
		{
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .GetMostRecentBuildNames(projectSpecifier.ProjectName, buildCount);
            return CreateBuildSpecifiers(projectSpecifier, response);
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
            GetCruiseManager(projectSpecifier, sessionToken)
                .DeleteProject(projectSpecifier.ProjectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

        public void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            ForceBuild(projectSpecifier, sessionToken, new Dictionary<string, string>());
		}

        public void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, Dictionary<string, string> parameters)
        {
            var manager = GetCruiseManager(projectSpecifier, sessionToken);
            manager.ForceBuild(projectSpecifier.ProjectName, NameValuePair.FromDictionary(parameters));
        }

        public void AbortBuild(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            GetCruiseManager(projectSpecifier, sessionToken).AbortBuild(projectSpecifier.ProjectName);
		}

        private ServerLocation GetServerUrl(IServerSpecifier serverSpecifier)
        {
            var locations = ServerLocations;
            foreach (var serverLocation in locations)
            {
                if (StringUtil.EqualsIgnoreCase(serverLocation.Name, serverSpecifier.ServerName))
                {
                    return serverLocation;
                }
            }

            throw new UnknownServerException(serverSpecifier.ServerName);
        }

        public ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            var response = GetCruiseManager(projectSpecifier.ServerSpecifier, sessionToken)
                .GetExternalLinks(projectSpecifier.ProjectName);
            return response;
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
                    var manager = GetCruiseManager(serverSpecifier, sessionToken);
                    foreach (ProjectStatus projectStatus in manager.GetProjectStatus())
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
			exceptions.Add(new CruiseServerException(serverSpecifier.ServerName, GetServerUrl(serverSpecifier).Url, e));
		}


        public string GetServerLog(IServerSpecifier serverSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(serverSpecifier, sessionToken)
                .GetServerLog();
            return response;
		}

        public string GetServerLog(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(projectSpecifier.ServerSpecifier, sessionToken)
                .GetServerLog(projectSpecifier.ProjectName);
            return response;
		}

        public void Start(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            GetCruiseManager(projectSpecifier, sessionToken)
                .StartProject(projectSpecifier.ProjectName);
		}

        public void Stop(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            GetCruiseManager(projectSpecifier.ServerSpecifier, sessionToken)
                .StopProject(projectSpecifier.ProjectName);
		}

		public string GetServerVersion(IServerSpecifier serverSpecifier)
		{
            var response = GetCruiseManager(serverSpecifier, null)
                .GetServerVersion();
            return response;
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
            GetCruiseManager(serverSpecifier, sessionToken).AddProject(serializedProject);
		}

        public string GetProject(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(projectSpecifier.ServerSpecifier, sessionToken)
                .GetProject(projectSpecifier.ProjectName);
            return response;
		}

        public void UpdateProject(IProjectSpecifier projectSpecifier, string serializedProject, string sessionToken)
		{
            GetCruiseManager(projectSpecifier, sessionToken)
                .UpdateProject(projectSpecifier.ProjectName, serializedProject);
		}

        public string GetArtifactDirectory(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .GetArtifactDirectory(projectSpecifier.ProjectName);
            return response;
		}

        public string GetStatisticsDocument(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .GetStatisticsDocument(projectSpecifier.ProjectName);
            return response;
		}

        public string GetModificationHistoryDocument(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .GetModificationHistoryDocument(projectSpecifier.ProjectName);
            return response;
        }

        public string GetRSSFeed(IProjectSpecifier projectSpecifier)
        {
            var response = GetCruiseManager(projectSpecifier, null)
                .GetRSSFeed(projectSpecifier.ProjectName);
            return response;
        }

        private CruiseServerClientBase GetCruiseManager(IBuildSpecifier buildSpecifier, string sessionToken)
		{
			return GetCruiseManager(buildSpecifier.ProjectSpecifier.ServerSpecifier, sessionToken);
		}

        private CruiseServerClientBase GetCruiseManager(IProjectSpecifier projectSpecifier, string sessionToken)
		{
            return GetCruiseManager(projectSpecifier.ServerSpecifier, sessionToken);
		}

        private CruiseServerClientBase GetCruiseManager(IServerSpecifier serverSpecifier, string sessionToken)
		{
            var config = GetServerUrl(serverSpecifier);
            CruiseServerClientBase manager = clientFactory.GenerateClient(config.Url,
                new ClientStartUpSettings
                {
                    BackwardsCompatable = config.BackwardCompatible
                });
            if (!string.IsNullOrEmpty(sessionToken)) manager.SessionToken = sessionToken;
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
            var manager = GetCruiseManager(GetServerConfiguration(server), null);
            manager.Login(credentials.Credentials);
            return manager.SessionToken;
		}

        public void Logout(string server, string sessionToken)
			{
            GetCruiseManager(GetServerConfiguration(server), sessionToken).Logout();
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
            GetCruiseManager(GetServerConfiguration(server), sessionToken)
                .ChangePassword(oldPassword, newPassword);
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
            GetCruiseManager(GetServerConfiguration(server), sessionToken)
                .ResetPassword(userName, newPassword);
        }

        private CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier[] serverSpecifiers, string sessionToken)
        {
            ArrayList cruiseServerSnapshotsOnServers = new ArrayList();
            ArrayList exceptions = new ArrayList();

            foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
            {
                try
                {
                    var response = GetCruiseManager(serverSpecifier, sessionToken)
                        .GetCruiseServerSnapshot();
                    cruiseServerSnapshotsOnServers.Add(
                        new CruiseServerSnapshotOnServer(response, serverSpecifier));
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
            return GetCruiseManager(serverSpecifier, sessionToken)
                .GetSecurityConfiguration();
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
            var response = GetCruiseManager(serverSpecifier, sessionToken)
                .ListUsers();
            return response;
        }

        /// <summary>
        /// Checks the security permissions for a user for a project.
        /// </summary>
        /// <param name="projectSpecifier">The project to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IProjectSpecifier projectSpecifier, string sessionToken, string userName)
        {
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .DiagnoseSecurityPermissions(userName, new string[] {
                    projectSpecifier.ProjectName
                });
            return response;
        }

        /// <summary>
        /// Checks the security permissions for a user for a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IServerSpecifier serverSpecifier, string sessionToken, string userName)
        {
            var response = GetCruiseManager(serverSpecifier, sessionToken)
                .DiagnoseSecurityPermissions(userName, new string [] {
                    string.Empty
                });
            return response;
        }

        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="projectSpecifier">The project to check.</param>
        /// <returns>The list of parameters (if any).</returns>
        public virtual List<ParameterBase> ListBuildParameters(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            return GetCruiseManager(projectSpecifier, sessionToken)
                .ListBuildParameters(projectSpecifier.ProjectName);
        }
  
        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords)
        {
            var response = GetCruiseManager(serverSpecifier, sessionToken)
                .ReadAuditRecords(startPosition, numberOfRecords);
            return response;
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
            var response = GetCruiseManager(serverSpecifier, sessionToken)
                .ReadAuditRecords(startPosition, numberOfRecords, filter);
            return response;
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
            var client = GetCruiseManager(serverSpecifer, null);
            var response = client.ProcessMessage(action, message);
            return response;
        }

        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public virtual long GetFreeDiskSpace(IServerSpecifier serverSpecifier)
        {
            var response = GetCruiseManager(serverSpecifier, null)
                .GetFreeDiskSpace();
            return Convert.ToInt64(response);
        }

        #region RetrieveFileTransfer()
        public virtual RemotingFileTransfer RetrieveFileTransfer(IProjectSpecifier projectSpecifier, string fileName, string sessionToken)
        {
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .RetrieveFileTransfer(projectSpecifier.ProjectName, fileName);
            return response as RemotingFileTransfer;
        }

        public virtual RemotingFileTransfer RetrieveFileTransfer(IBuildSpecifier buildSpecifier, string fileName, string sessionToken)
        {
            var logFile = new LogFile(buildSpecifier.BuildName);
            var fullName = string.Format("{0}\\{1}", logFile.Label, fileName);
            var fileTransfer = GetCruiseManager(buildSpecifier, sessionToken)
                .RetrieveFileTransfer(buildSpecifier.ProjectSpecifier.ProjectName, fullName);
            return fileTransfer as RemotingFileTransfer;
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
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .RetrievePackageList(projectSpecifier.ProjectName);
            return response.ToArray();
        }

        /// <summary>
        /// List the available packages for a build.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList(IBuildSpecifier buildSpecifier, string sessionToken)
        {
            var logFile = new LogFile(buildSpecifier.BuildName);
            var response = GetCruiseManager(buildSpecifier, sessionToken)
                .RetrievePackageList(buildSpecifier.ProjectSpecifier.ProjectName, logFile.Label);
            return response.ToArray();
        }
        #endregion

        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        public ProjectStatusSnapshot TakeStatusSnapshot(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            var response = GetCruiseManager(projectSpecifier, sessionToken)
                .TakeStatusSnapshot(projectSpecifier.ProjectName);
            return response;
        }

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieves the identifier for a project on a linked site.
        /// </summary>
        /// <param name="projectSpecifier">The project to retrieve the identifier for.</param>
        /// <param name="siteName">The name of the linked site.</param>
        /// <returns>The identifier of the other site.</returns>
        public string GetLinkedSiteId(IProjectSpecifier projectSpecifier, string sessionId, string siteName)
        {
            try
            {
                var response = GetCruiseManager(projectSpecifier, sessionId)
                    .GetLinkedSiteId(projectSpecifier.ProjectName, siteName);
                return response;
            }
            catch (NotImplementedException)
            {
                return string.Empty;
            }
        }
        #endregion
    }
}
