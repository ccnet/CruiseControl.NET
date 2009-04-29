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

		public IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier)
		{
			return new DefaultBuildSpecifier(projectSpecifier, GetCruiseManager(projectSpecifier.ServerSpecifier).GetLatestBuildName(projectSpecifier.ProjectName));
		}

		public string GetLog(IBuildSpecifier buildSpecifier)
		{
			return GetCruiseManager(buildSpecifier).GetLog(buildSpecifier.ProjectSpecifier.ProjectName, buildSpecifier.BuildName);
		}

		public IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier projectSpecifier)
		{
			return CreateBuildSpecifiers(projectSpecifier, GetCruiseManager(projectSpecifier).GetBuildNames(projectSpecifier.ProjectName));
		}

		public IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount)
		{
			return CreateBuildSpecifiers(projectSpecifier, GetCruiseManager(projectSpecifier).GetMostRecentBuildNames(projectSpecifier.ProjectName, buildCount));
		}

		public void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			GetCruiseManager(projectSpecifier).DeleteProject(projectSpecifier.ProjectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
        }

        #region Secure Project Methods
        public void Start(IProjectSpecifier projectSpecifier, string sessionToken)
			{
            GetCruiseManager(projectSpecifier.ServerSpecifier).Start(sessionToken, projectSpecifier.ProjectName);
			}

        public void Stop(IProjectSpecifier projectSpecifier, string sessionToken)
        {
            GetCruiseManager(projectSpecifier.ServerSpecifier).Stop(projectSpecifier.ProjectName);
		}

        public void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName)
        {
            ForceBuild(projectSpecifier, sessionToken, enforcerName, new Dictionary<string, string>());
        }

        public void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName, Dictionary<string, string> parameters)
        {
            GetCruiseManager(projectSpecifier.ServerSpecifier).ForceBuild(sessionToken, projectSpecifier.ProjectName, enforcerName, parameters);
        }

        public void AbortBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName)
		{
            GetCruiseManager(projectSpecifier.ServerSpecifier).AbortBuild(sessionToken, projectSpecifier.ProjectName, enforcerName);
		}
        #endregion


        #region Unsecure Project Methods
        public void Start(IProjectSpecifier projectSpecifier)
		{
            Start(projectSpecifier, null);
		}

        public void Stop(IProjectSpecifier projectSpecifier)
		{
            Stop(projectSpecifier, null);
        }

        public void ForceBuild(IProjectSpecifier projectSpecifier, string enforcerName)
        {
            ForceBuild(projectSpecifier, null, enforcerName, new Dictionary<string, string>());
        }

        public void ForceBuild(IProjectSpecifier projectSpecifier, string enforcerName, Dictionary<string, string> parameters)
        {
            ForceBuild(projectSpecifier, null, enforcerName, parameters);
        }

        public void AbortBuild(IProjectSpecifier projectSpecifier, string enforcerName)
				{
            AbortBuild(projectSpecifier, null, enforcerName);
			}

        #endregion


		public ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier).GetExternalLinks(projectSpecifier.ProjectName);
		}

		public ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions()
		{
			return GetProjectStatusListAndCaptureExceptions(GetServerSpecifiers());
		}

		public ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier)
		{
			return GetProjectStatusListAndCaptureExceptions(new IServerSpecifier[] {serverSpecifier});
		}


		public string GetServerLog(IServerSpecifier serverSpecifier)
		{
			return GetCruiseManager(serverSpecifier).GetServerLog();
		}

		public string GetServerLog(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier.ServerSpecifier).GetServerLog(projectSpecifier.ProjectName);
		}


		public string GetServerVersion(IServerSpecifier serverSpecifier)
		{
			return GetCruiseManager(serverSpecifier).GetServerVersion();
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

		public void AddProject(IServerSpecifier serverSpecifier, string serializedProject)
		{
			GetCruiseManager(serverSpecifier).AddProject(serializedProject);
		}

		public string GetProject(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier.ServerSpecifier).GetProject(projectSpecifier.ProjectName);
		}

		public void UpdateProject(IProjectSpecifier projectSpecifier, string serializedProject)
		{
			GetCruiseManager(projectSpecifier.ServerSpecifier).UpdateProject(projectSpecifier.ProjectName, serializedProject);
		}

		public string GetArtifactDirectory(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier).GetArtifactDirectory(projectSpecifier.ProjectName);
		}

		public string GetStatisticsDocument(IProjectSpecifier projectSpecifier)
		{
			return GetCruiseManager(projectSpecifier).GetStatisticsDocument(projectSpecifier.ProjectName);
		}

        public string GetModificationHistoryDocument(IProjectSpecifier projectSpecifier)
        {
            return GetCruiseManager(projectSpecifier).GetModificationHistoryDocument(projectSpecifier.ProjectName);
        }

        public string GetRSSFeed(IProjectSpecifier projectSpecifier)
        {
            return GetCruiseManager(projectSpecifier).GetRSSFeed(projectSpecifier.ProjectName);
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

        public CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions()
		{
            return GetCruiseServerSnapshotListAndExceptions(GetServerSpecifiers());
		}

        public CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier serverSpecifier)
		{
            return GetCruiseServerSnapshotListAndExceptions(new IServerSpecifier[] { serverSpecifier });
		}

        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public long GetFreeDiskSpace(IServerSpecifier serverSpecifier)
        {
            long space = GetCruiseManager(serverSpecifier).GetFreeDiskSpace();
            return space;
        }

        #region RetrieveFileTransfer()
        public RemotingFileTransfer RetrieveFileTransfer(IProjectSpecifier projectSpecifier, string fileName)
        {
            RemotingFileTransfer fileTransfer = GetCruiseManager(projectSpecifier)
                .RetrieveFileTransfer(projectSpecifier.ProjectName, fileName);
            return fileTransfer;
        }

        public RemotingFileTransfer RetrieveFileTransfer(IBuildSpecifier buildSpecifier, string fileName)
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
        public PackageDetails[] RetrievePackageList(IProjectSpecifier projectSpecifier)
        {
            var packages = GetCruiseManager(projectSpecifier).RetrievePackageList(projectSpecifier.ProjectName);
            return packages;
        }

        /// <summary>
        /// List the available packages for a build.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        public PackageDetails[] RetrievePackageList(IBuildSpecifier buildSpecifier)
        {
            var logFile = new LogFile(buildSpecifier.BuildName);
            var packages = GetCruiseManager(buildSpecifier)
                .RetrievePackageList(buildSpecifier.ProjectSpecifier.ProjectName, logFile.Label);
            return packages;
        }
        #endregion

        public string Login(string server, ISecurityCredentials credentials)
		{
            return GetCruiseManager(GetServerConfiguration(server)).Login(credentials);
		}

        public void Logout(string server, string sessionToken)
			{
            GetCruiseManager(GetServerConfiguration(server)).Logout(sessionToken);
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
            GetCruiseManager(GetServerConfiguration(server)).ChangePassword(sessionToken, oldPassword, newPassword);
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
            GetCruiseManager(GetServerConfiguration(server)).ResetPassword(sessionToken, userName, newPassword);
        }

        private CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier[] serverSpecifiers)
        {
            ArrayList cruiseServerSnapshotsOnServers = new ArrayList();
            ArrayList exceptions = new ArrayList();

            foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
            {
                try
                {
                    CruiseServerSnapshot cruiseServerSnapshot = GetCruiseManager(serverSpecifier).GetCruiseServerSnapshot();
                    cruiseServerSnapshotsOnServers.Add(new CruiseServerSnapshotOnServer(cruiseServerSnapshot, serverSpecifier));
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
            return GetCruiseManager(serverSpecifier).GetSecurityConfiguration(sessionToken);
        }

        /// <summary>
        /// Lists all the users who have been defined on a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to get the users from.</param>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListAllUsers(IServerSpecifier serverSpecifier, string sessionToken)
        {
            return GetCruiseManager(serverSpecifier).ListAllUsers(sessionToken);
        }

        /// <summary>
        /// Checks the security permissions for a user for a project.
        /// </summary>
        /// <param name="projectSpecifier">The project to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IProjectSpecifier projectSpecifier, string sessionToken, string userName)
        {
            return GetCruiseManager(projectSpecifier).DiagnoseSecurityPermissions(sessionToken, userName, projectSpecifier.ProjectName);
        }

        /// <summary>
        /// Checks the security permissions for a user for a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IServerSpecifier serverSpecifier, string sessionToken, string userName)
        {
            return GetCruiseManager(serverSpecifier).DiagnoseSecurityPermissions(sessionToken, userName, string.Empty);
        }

  
        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords)
        {
            return GetCruiseManager(serverSpecifier).ReadAuditRecords(sessionToken, startPosition, numberOfRecords);
        }

        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords, IAuditFilter filter)
        {
            return GetCruiseManager(serverSpecifier).ReadAuditRecords(sessionToken, startPosition, numberOfRecords, filter);
        }

        private ICruiseManager GetCruiseManager(IBuildSpecifier buildSpecifier)
        {
            return GetCruiseManager(buildSpecifier.ProjectSpecifier);
        }

        private ICruiseManager GetCruiseManager(IProjectSpecifier projectSpecifier)
        {
            return GetCruiseManager(projectSpecifier.ServerSpecifier);
        }

        private ICruiseManager GetCruiseManager(IServerSpecifier serverSpecifier)
        {
            return managerFactory.GetCruiseManager(GetServerUrl(serverSpecifier));
        }

        private ServerLocation[] ServerLocations
        {
            get { return configuration.Servers; }
        }

        private ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier[] serverSpecifiers)
        {
            ArrayList projectStatusOnServers = new ArrayList();
            ArrayList exceptions = new ArrayList();

            foreach (IServerSpecifier serverSpecifier in serverSpecifiers)
            {
                try
                {
                    foreach (ProjectStatus projectStatus in GetCruiseManager(serverSpecifier).GetProjectStatus())
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

        private IBuildSpecifier[] CreateBuildSpecifiers(IProjectSpecifier projectSpecifier, string[] buildNames)
        {
            ArrayList buildSpecifiers = new ArrayList();
            foreach (string buildName in buildNames)
            {
                buildSpecifiers.Add(new DefaultBuildSpecifier(projectSpecifier, buildName));
            }
            return (IBuildSpecifier[])buildSpecifiers.ToArray(typeof(IBuildSpecifier));
        }

        private string GetServerUrl(IServerSpecifier serverSpecifier)
        {
            foreach (ServerLocation serverLocation in ServerLocations)
            {
                if (StringUtil.EqualsIgnoreCase(serverLocation.Name, serverSpecifier.ServerName))
                {
                    return serverLocation.Url;
                }
            }

            throw new UnknownServerException(serverSpecifier.ServerName);
        }

        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        public ProjectStatusSnapshot TakeStatusSnapshot(IProjectSpecifier projectSpecifier)
        {
            return GetCruiseManager(projectSpecifier).TakeStatusSnapshot(projectSpecifier.ProjectName);
        }

        /// <summary>
        /// Lists all the parameters for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public List<ParameterBase> ListBuildParameters(IProjectSpecifier projectSpecifier)
        {
            var parameters = GetCruiseManager(projectSpecifier).ListBuildParameters(projectSpecifier.ProjectName);
            return parameters;
        }
    }
}
