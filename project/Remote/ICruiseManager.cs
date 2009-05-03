using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Parameters;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <remarks>
	/// Remote Interface to CruiseControl.NET.
	/// </remarks>
	public interface ICruiseManager
	{
		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
        ProjectStatus[] GetProjectStatus();


        #region Unsecured Build Methods
		/// <summary>
		/// Forces a build for the named project.
		/// </summary>
		/// <param name="projectName">project to force</param>
        /// <param name="enforcerName">ID of trigger/action forcing the build</param>
        void ForceBuild(string projectName, string enforcerName);

        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="projectName">project to force</param>
        /// <param name="enforcerName">ID of trigger/action forcing the build</param>
        /// <param name="parameters">The parameters to use.</param>
        void ForceBuild(string projectName, string enforcerName, Dictionary<string, string> parameters);
        
        /// <summary>
        /// Aborts the build for the named project
        /// </summary>
        /// <param name="projectName">project to abort build from</param>
        /// <param name="enforcerName">ID of trigger/action requesting the abort</param>
		void AbortBuild(string projectName, string enforcerName);
		void Request(string projectName, IntegrationRequest integrationRequest);

        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        void CancelPendingRequest(string projectName);

        /// <summary>
        /// Starts a project, so it can be integrated 
        /// </summary>
        /// <param name="project">project to start</param>
		void Start(string project);
        /// <summary>
        /// Stops a project, the project will not be integrated anymore
        /// </summary>
        /// <param name="project">project to stop</param>
		void Stop(string project);

        /// <summary>
        /// Adds a message to the message list of the project
        /// </summary>
        /// <param name="projectName">project to add the message to</param>
        /// <param name="message">the message</param>
		void SendMessage(string projectName, Message message);
        #endregion


        #region Secured Build Methods
        void ForceBuild(string sessionToken, string projectName, string enforcerName);
        void ForceBuild(string sessionToken, string projectName, string enforcerName, Dictionary<string, string> parameters);
        void AbortBuild(string sessionToken, string projectName, string enforcerName);
        void Request(string sessionToken, string projectName, IntegrationRequest integrationRequest);

        void Start(string sessionToken, string project);
        void Stop(string sessionToken, string project);
        void SendMessage(string sessionToken, string projectName, Message message);
        void CancelPendingRequest(string sessionToken, string projectName);
        #endregion

		/// <summary>
        /// waits for a project to end its integration cycle
		/// </summary>
        /// <param name="projectName"></param>
        void WaitForExit(string projectName);

        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        CruiseServerSnapshot GetCruiseServerSnapshot();

		/// <summary>
		/// Returns the name of the most recent build for the specified project
		/// </summary>
		string GetLatestBuildName(string projectName);

		/// <summary>
		/// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
		/// </summary>
		string[] GetBuildNames(string projectName);

		/// <summary>
		/// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
		/// </summary>
		string[] GetMostRecentBuildNames(string projectName, int buildCount);

		/// <summary>
		/// Returns the build log contents for requested project and build name
		/// </summary>
		string GetLog(string projectName, string buildName);

		/// <summary>
		/// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
		/// </summary>
		string GetServerLog();

		/// <summary>
		/// Returns a log of recent build server activity for a specific project. How much information that is returned is configured on the build server.
		/// </summary>
		string GetServerLog(string projectName);

		/// <summary>
		/// Returns the version of the server
		/// </summary>
		string GetServerVersion();

		/// <summary>
		/// Adds a project to the server
		/// </summary>
		void AddProject(string serializedProject);

		/// <summary>
		/// Deletes the specified project from the server
		/// </summary>
		void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);

		/// <summary>
		/// Returns the serialized form of the requested project from the server
		/// </summary>
		string GetProject(string projectName);

		/// <summary>
		/// Updates the selected project on the server
		/// </summary>
		void UpdateProject(string projectName, string serializedProject);

		ExternalLink[] GetExternalLinks(string projectName);

		string GetArtifactDirectory(string projectName);

		string GetStatisticsDocument(string projectName);

        string GetModificationHistoryDocument(string projectName);

        /// <summary>
        /// returns the xmlfeed of the given project
        /// </summary>
        /// <param name="projectName">project to get the rss feed from</param>
        /// <returns></returns>
        string GetRSSFeed(string projectName);

        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        long GetFreeDiskSpace();

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        ProjectStatusSnapshot TakeStatusSnapshot(string projectName);
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the latest list of packages for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        PackageDetails[] RetrievePackageList(string projectName);

        /// <summary>
        /// Retrieves the list of packages for a build for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="buildLabel"></param>
        /// <returns></returns>
        PackageDetails[] RetrievePackageList(string projectName, string buildLabel);
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        RemotingFileTransfer RetrieveFileTransfer(string project, string fileName);
        #endregion

        #region Security Methods
        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        string Login(ISecurityCredentials credentials);

        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="sesionToken"></param>
        void Logout(string sesionToken);

        /// <summary>
        /// Validates and stores the session token.
        /// </summary>
        /// <param name="sessionToken">The session token to validate.</param>
        /// <returns>True if the session is valid, false otherwise.</returns>
        bool ValidateSession(string sessionToken);

        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        string GetSecurityConfiguration(string sessionToken);

        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        List<UserDetails> ListAllUsers(string sessionToken);

        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="projectNames">The names of the projects to check.</param>
        /// <returns>A set of diagnostics information.</returns>
        List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string sessionToken, string userName, params string[] projectNames);

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        List<AuditRecord> ReadAuditRecords(string sessionToken, int startPosition, int numberOfRecords);

        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        List<AuditRecord> ReadAuditRecords(string sessionToken, int startPosition, int numberOfRecords, IAuditFilter filter);

        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        void ChangePassword(string sessionToken, string oldPassword, string newPassword);

        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        void ResetPassword(string sessionToken, string userName, string newPassword);
        #endregion

        /// <summary>
        /// Lists all the parameters for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        List<ParameterBase> ListBuildParameters(string projectName);
    }
}
