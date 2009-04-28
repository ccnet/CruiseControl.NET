using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Events;

namespace ThoughtWorks.CruiseControl.Remote
{
	public interface ICruiseServer : IDisposable
	{
		/// <summary>
		/// Launches the CruiseControl.NET server and starts all project schedules it contains
		/// </summary>
		void Start();

		/// <summary>
		/// Requests all started projects within the CruiseControl.NET server to stop
		/// </summary>
		void Stop();

		/// <summary>
		/// Terminates the CruiseControl.NET server immediately, stopping all started projects
		/// </summary>
		void Abort();

		/// <summary>
		/// Wait for CruiseControl server to finish executing
		/// </summary>
		void WaitForExit();

        void Start(string sessionToken, string project);
        void Stop(string sessionToken, string project);

		/// <summary>
		/// Cancel a pending project integration request from the integration queue.
		/// </summary>
        void CancelPendingRequest(string sessionToken, string projectName);
		
		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        CruiseServerSnapshot GetCruiseServerSnapshot();

		/// <summary>
		/// Retrieve CruiseManager interface for the server
		/// </summary>
		ICruiseManager CruiseManager { get; }

		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
		ProjectStatus [] GetProjectStatus();

		/// <summary>
		/// Forces a build for the named project.
		/// </summary>
		/// <param name="projectName">name of the project to force a build</param>
        /// <param name="enforcerName">name or id of the person, program that forces the build</param>
        void ForceBuild(string sessionToken, string projectName, string enforcerName);


		
		/// <summary>
		/// Aborts the build of the selected project.
		/// </summary>
        /// <param name="sessionToken"></param>
        /// <param name="projectName"></param>
        /// <param name="enforcerName"></param>
		void AbortBuild(string sessionToken,string projectName, string enforcerName);
		

        void Request(string sessionToken, string projectName, IntegrationRequest request);

		void WaitForExit(string projectName);
		
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
		/// Returns a log of recent build server activity for the specified project. How much information that is returned is configured on the build server.
		/// </summary>
		string GetServerLog(string projectName);

		/// <summary>
		/// Returns the version number of the server
		/// </summary>
		string GetVersion();

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
		string GetProject(string name);

		/// <summary>
		/// Updates the specified project configuration on the server
		/// </summary>
		void UpdateProject(string projectName, string serializedProject);

		ExternalLink[] GetExternalLinks(string projectName);
        void SendMessage(string sessionToken, string projectName, Message message);

		string GetArtifactDirectory(string projectName);

		string GetStatisticsDocument(string projectName);

        string GetModificationHistoryDocument(string projectName);

        string GetRSSFeed(string projectName);

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
        /// Retrieves the security configuration for the server.
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
        /// Reads all the specified number of filtered audit events.
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
    }
}
