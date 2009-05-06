using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <remarks>
	/// Remote Interface to CruiseControl.NET.
	/// </remarks>
	public interface ICruiseServerClient
        : IMessageProcessor
    {
        #region GetProjectStatus()
        /// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
		ProjectStatusResponse GetProjectStatus(ServerRequest request);
        #endregion

        #region Start()
        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        Response Start(ProjectRequest request);
        #endregion

        #region Stop()
        /// <summary>
        /// Attempts to stop a project.
        /// </summary>
        /// <param name="request">A <see cref="ProjectRequest"/> containing the request details.</param>
        /// <returns>A <see cref="Response"/> containing the results of the request.</returns>
        Response Stop(ProjectRequest request);
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

        #region WaitForExit()
        /// <summary>
        /// Waits for the project to exit.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Response WaitForExit(ProjectRequest request);
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        SnapshotResponse GetCruiseServerSnapshot(ServerRequest request);
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
        ExternalLinksListResponse GetExternalLinks(ProjectRequest request);
        #endregion

        #region GetArtifactDirectory()
        DataResponse GetArtifactDirectory(ProjectRequest request);
        #endregion

        #region GetStatisticsDocument()
        DataResponse GetStatisticsDocument(ProjectRequest request);
        #endregion

        #region GetModificationHistoryDocument()
        DataResponse GetModificationHistoryDocument(ProjectRequest request);
        #endregion

        #region GetRSSFeed()
        DataResponse GetRSSFeed(ProjectRequest request);
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
        /// A list of <see cref="UserNameCredentials"/> containing the details on all the users
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
        /// <param name="projectName">The name of the project to retrieve the parameters for.</param>
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
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        RemotingFileTransfer RetrieveFileTransfer(string project, string fileName);
        #endregion
    }
}
