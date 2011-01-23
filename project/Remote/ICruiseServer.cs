using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Events;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// The main server for running Continuous Integration.
    /// </summary>
	public interface ICruiseServer : IDisposable
	{
        #region Abort()
        /// <summary>
        /// Terminates the CruiseControl.NET server immediately, stopping all started projects
        /// </summary>
        void Abort();
        #endregion

        #region Start()
		/// <summary>
		/// Launches the CruiseControl.NET server and starts all project schedules it contains
		/// </summary>
		void Start();

		/// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        Response Start(ProjectRequest request);
        #endregion

        #region Stop()
        /// <summary>
		/// Requests all started projects within the CruiseControl.NET server to stop
		/// </summary>
		void Stop();

		/// <summary>
        /// Attempts to stop a project.
		/// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        Response Stop(ProjectRequest request);
        #endregion

        #region CancelPendingRequest()
		/// <summary>
        /// Cancel a pending project integration request from the integration queue.
		/// </summary>
        Response CancelPendingRequest(ProjectRequest request);
        #endregion

        #region SendMessage()
		/// <summary>
        /// Send a text message to the server.
		/// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Response SendMessage(MessageRequest request);
        #endregion
		
        #region GetCruiseServerSnapshot()
		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        SnapshotResponse GetCruiseServerSnapshot(ServerRequest request);
        #endregion

        #region CruiseManager
		/// <summary>
		/// Retrieve CruiseManager interface for the server
		/// </summary>
        [Obsolete("Use CruiseServerClient instead")]
		ICruiseManager CruiseManager { get; }
        #endregion

        #region CruiseServerClient
        /// <summary>
        /// Client for communicating with the server.
        /// </summary>
        ICruiseServerClient CruiseServerClient { get; }
        #endregion

        #region GetProjectStatus()
		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
        ProjectStatusResponse GetProjectStatus(ServerRequest request);
        #endregion

        #region ForceBuild()
		/// <summary>
		/// Forces a build for the named project.
		/// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        Response ForceBuild(ProjectRequest request);
        #endregion
		
        #region AbortBuild()
		/// <summary>
		/// Aborts the build of the selected project.
		/// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        Response AbortBuild(ProjectRequest request);
        #endregion
		
        #region WaitForExit()
        /// <summary>
        /// Wait for CruiseControl server to finish executing
        /// </summary>
        void WaitForExit();

        /// <summary>
        /// Waits for the project to exit.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Response WaitForExit(ProjectRequest request);
        #endregion
		
        #region GetLatestBuildName()
		/// <summary>
		/// Returns the name of the most recent build for the specified project
		/// </summary>
        DataResponse GetLatestBuildName(ProjectRequest request);
        #endregion

        #region GetBuildNames()
		/// <summary>
		/// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
		/// </summary>
        DataListResponse GetBuildNames(ProjectRequest request);
        #endregion

        #region GetMostRecentBuildNames()
		/// <summary>
		/// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
		/// </summary>
        DataListResponse GetMostRecentBuildNames(BuildListRequest request);
        #endregion

        #region GetLog()
		/// <summary>
		/// Returns the build log contents for requested project and build name
		/// </summary>
        DataResponse GetLog(BuildRequest request);
        #endregion

        #region GetFinalBuildStatus()
        /// <summary>
        /// Gets the final status for a build.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>The <see cref="StatusSnapshotResponse"/> for the build.</returns>
        StatusSnapshotResponse GetFinalBuildStatus(BuildRequest request);
        #endregion

        #region GetServerLog()
        /// <summary>
		/// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
		/// </summary>
        DataResponse GetServerLog(ServerRequest request);
        #endregion

        #region GetServerVersion()
		/// <summary>
        /// Returns the version of the server
		/// </summary>
        DataResponse GetServerVersion(ServerRequest request);
        #endregion

        #region AddProject()
		/// <summary>
        /// Adds a project to the server
		/// </summary>
        Response AddProject(ChangeConfigurationRequest request);
        #endregion

        #region DeleteProject()
		/// <summary>
        /// Deletes the specified project from the server
		/// </summary>
        Response DeleteProject(ChangeConfigurationRequest request);
        #endregion

        #region UpdateProject()
		/// <summary>
        /// Updates the selected project on the server
		/// </summary>
        Response UpdateProject(ChangeConfigurationRequest request);
        #endregion

        #region GetProject()
		/// <summary>
		/// Returns the serialized form of the requested project from the server
		/// </summary>
        DataResponse GetProject(ProjectRequest request);
        #endregion

        #region GetExternalLinks()
		/// <summary>
        /// Retrieve the list of external links for the project.
		/// </summary>
        ExternalLinksListResponse GetExternalLinks(ProjectRequest request);
        #endregion

        #region GetArtifactDirectory()
        /// <summary>
        /// Retrieves the name of directory used for storing artefacts for a project.
        /// </summary>
        DataResponse GetArtifactDirectory(ProjectRequest request);
        #endregion

        #region GetStatisticsDocument()
        /// <summary>
        /// Retrieve the statistics document for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DataResponse GetStatisticsDocument(ProjectRequest request);
        #endregion

        #region GetModificationHistoryDocument()
        /// <summary>
        /// Retrieve the modification history document for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DataResponse GetModificationHistoryDocument(ProjectRequest request);
        #endregion

        #region GetRSSFeed()
        /// <summary>
        /// Retrieve the RSS feed for a project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DataResponse GetRSSFeed(ProjectRequest request);
        #endregion

        #region Events
        /// <summary>
        /// A project is starting.
        /// </summary>
        event EventHandler<CancelProjectEventArgs> ProjectStarting;

        /// <summary>
        /// A project has started.
        /// </summary>
        event EventHandler<ProjectEventArgs> ProjectStarted;

        /// <summary>
        /// A project is stopping.
        /// </summary>
        event EventHandler<CancelProjectEventArgs> ProjectStopping;

        /// <summary>
        /// A project has stopped.
        /// </summary>
        event EventHandler<ProjectEventArgs> ProjectStopped;

        /// <summary>
        /// A force build has been received.
        /// </summary>
        event EventHandler<CancelProjectEventArgs<string>> ForceBuildReceived;

        /// <summary>
        /// A force build has been processed.
        /// </summary>
        event EventHandler<ProjectEventArgs<string>> ForceBuildProcessed;

        /// <summary>
        /// An abort build has been received.
        /// </summary>
        event EventHandler<CancelProjectEventArgs<string>> AbortBuildReceived;

        /// <summary>
        /// An abort build has been processed.
        /// </summary>
        event EventHandler<ProjectEventArgs<string>> AbortBuildProcessed;

        /// <summary>
        /// A send message has been received.
        /// </summary>
        event EventHandler<CancelProjectEventArgs<Message>> SendMessageReceived;

        /// <summary>
        /// A send message has been processed.
        /// </summary>
        event EventHandler<ProjectEventArgs<Message>> SendMessageProcessed;

        /// <summary>
        /// A project integrator is starting an integration.
        /// </summary>
        event EventHandler<IntegrationStartedEventArgs> IntegrationStarted;

        /// <summary>
        /// A project integrator has completed an integration.
        /// </summary>
        event EventHandler<IntegrationCompletedEventArgs> IntegrationCompleted;
        #endregion

        #region Login()
        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        LoginResponse Login(LoginRequest request);
        #endregion

        #region Logout()
        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="request"></param>
        Response Logout(ServerRequest request);
        #endregion

        #region GetSecurityConfiguration()
        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        /// <param name="request"></param>
        DataResponse GetSecurityConfiguration(ServerRequest request);
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
        ListUsersResponse ListUsers(ServerRequest request);
        #endregion

        #region DiagnoseSecurityPermissions()
        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A set of diagnostics information.</returns>
        DiagnoseSecurityResponse DiagnoseSecurityPermissions(DiagnoseSecurityRequest request);
        #endregion

        #region ReadAuditRecords()
        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        ReadAuditResponse ReadAuditRecords(ReadAuditRequest request);
        #endregion

        #region ListBuildParameters()
        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        ///<param name="request"></param>
        /// <returns>The list of parameters (if any).</returns>
        BuildParametersResponse ListBuildParameters(ProjectRequest request);
        #endregion
        
        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="request"></param>
        Response ChangePassword(ChangePasswordRequest request);
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="request"></param>
        Response ResetPassword(ChangePasswordRequest request);
        #endregion

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        DataResponse GetFreeDiskSpace(ServerRequest request);
        #endregion

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        StatusSnapshotResponse TakeStatusSnapshot(ProjectRequest request);
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves a list of packages for a project.
        /// </summary>
        ListPackagesResponse RetrievePackageList(ProjectRequest request);
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        FileTransferResponse RetrieveFileTransfer(FileTransferRequest request);
        #endregion

        #region RetrieveService()
        /// <summary>
        /// Retrieves a service.
        /// </summary>
        /// <param name="serviceType">The type of service to add.</param>
        /// <returns>A valid service, if found, null otherwise.</returns>
        object RetrieveService(Type serviceType);
        #endregion

        #region AddService()
        /// <summary>
        /// Adds a service.
        /// </summary>
        /// <param name="serviceType">The type of service.</param>
        /// <param name="service">The service to add.</param>
        void AddService(Type serviceType, object service);
        #endregion

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieve the identifer for this project on a linked site.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        DataResponse GetLinkedSiteId(ProjectItemRequest request);
        #endregion
    }
}
