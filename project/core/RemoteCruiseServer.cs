
using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// Connects to a remote CruiseControl.NET server.
    /// </summary>
    public class RemoteCruiseServer 
        : CruiseServerEventsBase, ICruiseServer, IDisposable
    {
        public const string ManagerUri = "CruiseManager.rem";
        public const string DefaultManagerUri = "tcp://localhost:21234/" + ManagerUri;
        public const string ServerClientUri = "CruiseServerClient.rem";
        public const string DefaultServerClientUri = "tcp://localhost:21234/" + ServerClientUri;

        private readonly bool disableRemoting;
        private ICruiseServer server;
        private bool disposed;
        private IExecutionEnvironment environment = new ExecutionEnvironment();

        public RemoteCruiseServer(ICruiseServer server, string remotingConfigurationFile)
            : this(server, remotingConfigurationFile, false)
        {
        }

        public RemoteCruiseServer(ICruiseServer server, string remotingConfigurationFile, bool disableRemoting)
        {
            // Store the server instance and wire up the events so they are passed on
            this.server = server;
            this.server.AbortBuildProcessed += (o, e) => { this.FireAbortBuildProcessed(e.ProjectName, e.Data); };
            this.server.AbortBuildReceived += (o, e) => { this.FireAbortBuildReceived(e.ProjectName, e.Data); };
            this.server.ForceBuildProcessed += (o, e) => { this.FireForceBuildProcessed(e.ProjectName, e.Data); };
            this.server.ForceBuildReceived += (o, e) => { this.FireForceBuildReceived(e.ProjectName, e.Data); };
            this.server.IntegrationCompleted += (o, e) => { this.FireIntegrationCompleted(e.Request, e.ProjectName, e.Status); };
            this.server.IntegrationStarted += (o, e) => { this.FireIntegrationStarted(e.Request, e.ProjectName); };
            this.server.ProjectStarted += (o, e) => { this.FireProjectStarted(e.ProjectName); };
            this.server.ProjectStarting += (o, e) => { this.FireProjectStarting(e.ProjectName); };
            this.server.ProjectStopped += (o, e) => { this.FireProjectStopped(e.ProjectName); };
            this.server.ProjectStopping += (o, e) => { this.FireProjectStopping(e.ProjectName); };
            this.server.SendMessageProcessed += (o, e) => { this.FireSendMessageProcessed(e.ProjectName, e.Data); };
            this.server.SendMessageReceived += (o, e) => { this.FireSendMessageReceived(e.ProjectName, e.Data); };

            this.disableRemoting = disableRemoting;
            if (!disableRemoting)
            {
                RemotingConfiguration.Configure(remotingConfigurationFile, false);
                RegisterManagerForRemoting();
                RegisterServerClientForRemoting();
            }
        }

        private void RegisterManagerForRemoting()
        {
            MarshalByRefObject marshalByRef = (MarshalByRefObject)server.CruiseManager;
            RemotingServices.Marshal(marshalByRef, ManagerUri);

            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                Log.Info("Registered channel: " + channel.ChannelName);

                // GetUrlsForUri is not support on cross-AppDomain channels on mono (as of 1.1.8.3)
                if (environment.IsRunningOnWindows)
                {
                    var dummy = channel as IChannelReceiver;
                    if (dummy != null)
                    {
                        foreach (string url in dummy.GetUrlsForUri(ManagerUri))
                        {
                            Log.Info("CruiseManager: Listening on url: " + url);
                        }
                    }
                }
            }
        }

        private void RegisterServerClientForRemoting()
        {
            MarshalByRefObject marshalByRef = (MarshalByRefObject)server.CruiseServerClient;
            RemotingServices.Marshal(marshalByRef, ServerClientUri);

            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                Log.Info("Registered channel: " + channel.ChannelName);

                // GetUrlsForUri is not support on cross-AppDomain channels on mono (as of 1.1.8.3)
                if (environment.IsRunningOnWindows)
                {
                    var dummy = channel as IChannelReceiver;
                    if (dummy != null)
                    {
                        foreach (string url in dummy.GetUrlsForUri(ServerClientUri))
                        {
                            Log.Info("CruiseServerClient: Listening on url: " + url);
                        }
                    }
                }
            }
        }

        ~RemoteCruiseServer()
        {
            // If for some reason CC.NET crashes and Dispose() is not called, this will hopefully clean up the port
            // (eventually). Since this relies on GC, we cannot guarentee when it will be called!
            try
            {
                this.Dispose();
            }
            catch
            {
                // Ignore any exceptions here - the application will be well and truly finished
            }
        }

        public void Dispose()
        {
            lock (this)
            {
                if (disposed) return;
                disposed = true;
            }

            if (!disableRemoting)
            {
                Log.Info("Disconnecting remote server: ");
                RemotingServices.Disconnect((MarshalByRefObject)server.CruiseManager);
                RemotingServices.Disconnect((MarshalByRefObject)server.CruiseServerClient);
                foreach (IChannel channel in ChannelServices.RegisteredChannels)
                {
                    Log.Info("Unregistering channel: " + channel.ChannelName);
                    ChannelServices.UnregisterChannel(channel);
                }
            }

            server.Dispose();
        }

        #region Abort()
        /// <summary>
        /// Terminates the CruiseControl.NET server immediately, stopping all started projects
        /// </summary>
        public virtual void Abort()
        {
            server.Abort();
        }
        #endregion

        #region Start()
        /// <summary>
        /// Launches the CruiseControl.NET server and starts all project schedules it contains
        /// </summary>
        public virtual void Start()
        {
            server.Start();
        }

        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public virtual Response Start(ProjectRequest request)
        {
            return server.Start(request);
        }
        #endregion

        #region Stop()
        /// <summary>
        /// Requests all started projects within the CruiseControl.NET server to stop
        /// </summary>
        public virtual void Stop()
        {
            server.Stop();
        }

        /// <summary>
        /// Attempts to stop a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public virtual Response Stop(ProjectRequest request)
        {
            return server.Stop(request);
        }
        #endregion

        #region CancelPendingRequest()
        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        public virtual Response CancelPendingRequest(ProjectRequest request)
        {
            return server.CancelPendingRequest(request);
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Send a text message to the server.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual Response SendMessage(MessageRequest request)
        {
            return server.SendMessage(request);
        }
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        public virtual SnapshotResponse GetCruiseServerSnapshot(ServerRequest request)
        {
            return server.GetCruiseServerSnapshot(request);
        }
        #endregion

        #region CruiseManager
        /// <summary>
        /// Retrieve CruiseManager interface for the server
        /// </summary>
        [Obsolete("Use CruiseServerClient instead")]
        public virtual ICruiseManager CruiseManager
        {
            get { return server.CruiseManager; }
        }
        #endregion

        #region CruiseServerClient
        /// <summary>
        /// Client for communicating with the server.
        /// </summary>
        public virtual ICruiseServerClient CruiseServerClient
        {
            get { return server.CruiseServerClient; }
        }
        #endregion

        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public virtual ProjectStatusResponse GetProjectStatus(ServerRequest request)
        {
            return server.GetProjectStatus(request);
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public virtual Response ForceBuild(ProjectRequest request)
        {
            return server.ForceBuild(request);
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Aborts the build of the selected project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        public virtual Response AbortBuild(ProjectRequest request)
        {
            return server.AbortBuild(request);
        }
        #endregion

        #region WaitForExit()
        /// <summary>
        /// Wait for CruiseControl server to finish executing
        /// </summary>
        public virtual void WaitForExit()
        {
            server.WaitForExit();
        }

        /// <summary>
        /// Waits for the project to exit.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual Response WaitForExit(ProjectRequest request)
        {
            return server.WaitForExit(request);
        }
        #endregion

        #region GetLatestBuildName()
        /// <summary>
        /// Returns the name of the most recent build for the specified project
        /// </summary>
        public virtual DataResponse GetLatestBuildName(ProjectRequest request)
        {
            return server.GetLatestBuildName(request);
        }
        #endregion

        #region GetBuildNames()
        /// <summary>
        /// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public virtual DataListResponse GetBuildNames(ProjectRequest request)
        {
            return server.GetBuildNames(request);
        }
        #endregion

        #region GetMostRecentBuildNames()
        /// <summary>
        /// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public virtual DataListResponse GetMostRecentBuildNames(BuildListRequest request)
        {
            return server.GetMostRecentBuildNames(request);
        }
        #endregion

        #region GetLog()
        /// <summary>
        /// Returns the build log contents for requested project and build name
        /// </summary>
        public virtual DataResponse GetLog(BuildRequest request)
        {
            return server.GetLog(request);
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
        /// </summary>
        public virtual DataResponse GetServerLog(ServerRequest request)
        {
            return server.GetServerLog(request);
        }
        #endregion

        #region GetServerVersion()
        /// <summary>
        /// Returns the version of the server
        /// </summary>
        public virtual DataResponse GetServerVersion(ServerRequest request)
        {
            return server.GetServerVersion(request);
        }
        #endregion

        #region AddProject()
        /// <summary>
        /// Adds a project to the server
        /// </summary>
        public virtual Response AddProject(ChangeConfigurationRequest request)
        {
            return server.AddProject(request);
        }
        #endregion

        #region DeleteProject()
        /// <summary>
        /// Deletes the specified project from the server
        /// </summary>
        public virtual Response DeleteProject(ChangeConfigurationRequest request)
        {
            return server.DeleteProject(request);
        }
        #endregion

        #region UpdateProject()
        /// <summary>
        /// Updates the selected project on the server
        /// </summary>
        public virtual Response UpdateProject(ChangeConfigurationRequest request)
        {
            return server.UpdateProject(request);
        }
        #endregion

        #region GetProject()
        /// <summary>
        /// Returns the serialized form of the requested project from the server
        /// </summary>
        public virtual DataResponse GetProject(ProjectRequest request)
        {
            return server.GetProject(request);
        }
        #endregion

        #region GetExternalLinks()
        /// <summary>
        /// Retrieve the list of external links for the project.
        /// </summary>
        public virtual ExternalLinksListResponse GetExternalLinks(ProjectRequest request)
        {
            return server.GetExternalLinks(request);
        }
        #endregion

        #region GetArtifactDirectory()
        /// <summary>
        /// Retrieves the name of directory used for storing artefacts for a project.
        /// </summary>
        public virtual DataResponse GetArtifactDirectory(ProjectRequest request)
        {
            return server.GetArtifactDirectory(request);
        }
        #endregion

        #region GetStatisticsDocument()
        /// <summary>
        /// Retrieve the statistics document for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual DataResponse GetStatisticsDocument(ProjectRequest request)
        {
            return server.GetStatisticsDocument(request);
        }
        #endregion

        #region GetModificationHistoryDocument()
        /// <summary>
        /// Retrieve the modification history document for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual DataResponse GetModificationHistoryDocument(ProjectRequest request)
        {
            return server.GetModificationHistoryDocument(request);
        }
        #endregion

        #region GetRSSFeed()
        /// <summary>
        /// Retrieve the RSS feed for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual DataResponse GetRSSFeed(ProjectRequest request)
        {
            return server.GetRSSFeed(request);
        }
        #endregion

        #region Login()
        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual LoginResponse Login(LoginRequest request)
        {
            return server.Login(request);
        }
        #endregion

        #region Logout()
        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="request"></param>
        public virtual Response Logout(ServerRequest request)
        {
            return server.Logout(request);
        }
        #endregion

        #region GetSecurityConfiguration()
        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        /// <param name="request"></param>
        public virtual DataResponse GetSecurityConfiguration(ServerRequest request)
        {
            return server.GetSecurityConfiguration(request);
        }
        #endregion

        #region ListUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>
        /// A list of <see cref="ListUsersResponse"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual ListUsersResponse ListUsers(ServerRequest request)
        {
            return server.ListUsers(request);
        }
        #endregion

        #region DiagnoseSecurityPermissions()
        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual DiagnoseSecurityResponse DiagnoseSecurityPermissions(DiagnoseSecurityRequest request)
        {
            return server.DiagnoseSecurityPermissions(request);
        }
        #endregion

        #region ReadAuditRecords()
        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual ReadAuditResponse ReadAuditRecords(ReadAuditRequest request)
        {
            return server.ReadAuditRecords(request);
        }
        #endregion

        #region ListBuildParameters()
        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="request">The project to retrieve the parameters for.</param>
        /// <returns>The list of parameters (if any).</returns>
        public virtual BuildParametersResponse ListBuildParameters(ProjectRequest request)
        {
            return server.ListBuildParameters(request);
        }
        #endregion

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="request"></param>
        public virtual Response ChangePassword(ChangePasswordRequest request)
        {
            return server.ChangePassword(request);
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="request"></param>
        public virtual Response ResetPassword(ChangePasswordRequest request)
        {
            return server.ResetPassword(request);
        }
        #endregion

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public virtual DataResponse GetFreeDiskSpace(ServerRequest request)
        {
            return server.GetFreeDiskSpace(request);
        }
        #endregion

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        public virtual StatusSnapshotResponse TakeStatusSnapshot(ProjectRequest request)
        {
            return server.TakeStatusSnapshot(request);
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves a list of packages for a project.
        /// </summary>
        public virtual ListPackagesResponse RetrievePackageList(ProjectRequest request)
        {
            return server.RetrievePackageList(request);
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        public virtual FileTransferResponse RetrieveFileTransfer(FileTransferRequest request)
        {
            return server.RetrieveFileTransfer(request);
        }
        #endregion

        #region RetrieveService()
        /// <summary>
        /// Retrieves a service.
        /// </summary>
        /// <param name="serviceType">The type of service to retrieve.</param>
        /// <returns>A valid service, if found, null otherwise.</returns>
        public virtual object RetrieveService(Type serviceType)
        {
            return server.RetrieveService(serviceType);
        }
        #endregion

        #region AddService()
        /// <summary>
        /// Adds a service.
        /// </summary>
        /// <param name="serviceType">The type of service.</param>
        /// <param name="service">The service to add.</param>
        public virtual void AddService(Type serviceType, object service)
        {
            server.AddService(serviceType, service);
        }
        #endregion

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieve the identifer for this project on a linked site.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public virtual DataResponse GetLinkedSiteId(ProjectItemRequest request)
        {
            return server.GetLinkedSiteId(request);
        }
        #endregion
    }
}