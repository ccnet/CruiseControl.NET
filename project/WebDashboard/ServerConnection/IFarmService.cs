using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;

using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface IFarmService
	{
		IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount);
		IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier serverSpecifier);
		void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);
		string GetServerLog(IServerSpecifier serverSpecifier);
		string GetServerLog(IProjectSpecifier specifier);


        #region Unsecure Project Methods
        /// <summary>
        /// Unsecure start project, use for projects without security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
		void Start(IProjectSpecifier projectSpecifier);

        /// <summary>
        /// Unsecure stop project, use for projects without security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
		void Stop(IProjectSpecifier projectSpecifier);

        /// <summary>
        /// Unsecure force build project, use for projects without security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <param name="enforcerName"></param>
		void ForceBuild(IProjectSpecifier projectSpecifier, string enforcerName);

        /// <summary>
        /// Unsecure abort build project, use for projects without security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <param name="enforcerName"></param>
		void AbortBuild(IProjectSpecifier projectSpecifier, string enforcerName);
        #endregion


        #region Secure Project Methods
        /// <summary>
        /// Secure start project,  use for projects with security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <param name="sessionToken"></param>
        void Start(IProjectSpecifier projectSpecifier, string sessionToken);

        /// <summary>
        /// Secure start project,  use for projects with security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <param name="sessionToken"></param>
        void Stop(IProjectSpecifier projectSpecifier, string sessionToken);

        /// <summary>
        /// Secure force build project,  use for projects with security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <param name="sessionToken"></param>
        /// <param name="enforcerName"></param>
        void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName);

        /// <summary>
        /// Secure abort build  project,  use for projects with security setup
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <param name="sessionToken"></param>
        /// <param name="enforcerName"></param>
        void AbortBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName);
        #endregion


		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions();
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier);
		ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier);
		IServerSpecifier[] GetServerSpecifiers();
		IServerSpecifier GetServerConfiguration(string serverName);
		
		string GetServerVersion(IServerSpecifier serverSpecifier);
		string GetArtifactDirectory(IProjectSpecifier projectSpecifier);
		string GetStatisticsDocument(IProjectSpecifier projectSpecifier);
        CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions();
        CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier serverSpecifier);
        string GetModificationHistoryDocument(IProjectSpecifier projectSpecifier);
        string GetRSSFeed(IProjectSpecifier projectSpecifier);

        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        long GetFreeDiskSpace(IServerSpecifier serverSpecifier);

        #region RetrieveFileTransfer()
        RemotingFileTransfer RetrieveFileTransfer(IProjectSpecifier projectSpecifier, string fileName, FileTransferSource source);
        #endregion

        string Login(string server, ISecurityCredentials credentials);
        void Logout(string server, string sessionToken);
        void ChangePassword(string server, string sessionToken, string oldPassword, string newPassword);
        void ResetPassword(string server, string sessionToken, string userName, string newPassword);

        /// <summary>
        /// Retrieves the configuration for security on a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to get the configuration from.</param>
        /// <returns>An XML fragment containing the security configuration.</returns>
        string GetServerSecurity(IServerSpecifier serverSpecifier, string sessionToken);

        /// <summary>
        /// Lists all the users who have been defined on a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to get the users from.</param>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        List<UserDetails> ListAllUsers(IServerSpecifier serverSpecifier, string sessionToken);

        /// <summary>
        /// Checks the security permissions for a user for a project.
        /// </summary>
        /// <param name="projectSpecifier">The project to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IProjectSpecifier projectSpecifier, string sessionToken, string userName);

        /// <summary>
        /// Checks the security permissions for a user for a server.
        /// </summary>
        /// <param name="serverSpecifier">The server to check.</param>
        /// <param name="userName">The name of the user.</param>
        /// <returns>A set of diagnostics information.</returns>
        List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(IServerSpecifier serverSpecifier, string sessionToken, string userName);

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords);
       
 		/// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords, IAuditFilter filter);

        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        ProjectStatusSnapshot TakeStatusSnapshot(IProjectSpecifier projectSpecifier);
    }
}
