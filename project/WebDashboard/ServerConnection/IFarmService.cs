using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.WebDashboard.ServerConnection
{
	public interface IFarmService
	{
        IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount, string sessionToken);
        IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier serverSpecifier, string sessionToken);
        void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment, string sessionToken);
        string GetServerLog(IServerSpecifier serverSpecifier, string sessionToken);
        string GetServerLog(IProjectSpecifier specifier, string sessionToken);
        void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken);
        void AbortBuild(IProjectSpecifier projectSpecifier, string sessionToken);
        void Start(IProjectSpecifier projectSpecifier, string sessionToken);
        void Stop(IProjectSpecifier projectSpecifier, string sessionToken);
        void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, Dictionary<string, string> parameters);
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(string sessionToken);
        ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier, string sessionToken);
        ExternalLink[] GetExternalLinks(IProjectSpecifier projectSpecifier, string sessionToken);
		IServerSpecifier[] GetServerSpecifiers();
        IServerSpecifier GetServerConfiguration(string serverName);
		string GetServerVersion(IServerSpecifier serverSpecifier);
        string GetArtifactDirectory(IProjectSpecifier projectSpecifier, string sessionToken);
        string GetStatisticsDocument(IProjectSpecifier projectSpecifier, string sessionToken);
        CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(string sessionToken);
        CruiseServerSnapshotListAndExceptions GetCruiseServerSnapshotListAndExceptions(IServerSpecifier serverSpecifier, string sessionToken);
        string GetModificationHistoryDocument(IProjectSpecifier projectSpecifier, string sessionToken);
        string GetRSSFeed(IProjectSpecifier projectSpecifier);
        string Login(string server, LoginRequest credentials);
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
        /// A list of <see cref="UserNameCredentials"/> containing the details on all the users
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
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="projectSpecifier">The project to check.</param>
        /// <returns>The list of parameters (if any).</returns>
        List<ParameterBase> ListBuildParameters(IProjectSpecifier projectSpecifier, string sessionToken);

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
        List<AuditRecord> ReadAuditRecords(IServerSpecifier serverSpecifier, string sessionToken, int startPosition, int numberOfRecords, AuditFilterBase filter);

        /// <summary>
        /// Processes a message for a server.
        /// </summary>
        /// <param name="serverSpecifer">The server.</param>
        /// <param name="action">The action.</param>
        /// <param name="message">The message.</param>
        /// <returns>The response.</returns>
        string ProcessMessage(IServerSpecifier serverSpecifer, string action, string message);

        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        long GetFreeDiskSpace(IServerSpecifier serverSpecifier);

        #region RetrieveFileTransfer()
        RemotingFileTransfer RetrieveFileTransfer(IProjectSpecifier projectSpecifier, string fileName, string sessionToken);

        RemotingFileTransfer RetrieveFileTransfer(IBuildSpecifier buildSpecifier, string fileName, string sessionToken);
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// List the available packages for a project.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        PackageDetails[] RetrievePackageList(IProjectSpecifier projectSpecifier, string sessionToken);

        /// <summary>
        /// List the available packages for a build.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        PackageDetails[] RetrievePackageList(IBuildSpecifier buildSpecifier, string sessionToken);
        #endregion

        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        ProjectStatusSnapshot TakeStatusSnapshot(IProjectSpecifier projectSpecifier, string sessionToken);

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieves the identifier for a project on a linked site.
        /// </summary>
        /// <param name="projectSpecifier">The project to retrieve the identifier for.</param>
        /// <param name="siteName">The name of the linked site.</param>
        /// <returns>The identifier of the other site.</returns>
        string GetLinkedSiteId(IProjectSpecifier projectSpecifier, string sessionId, string siteName);
        #endregion
    }
}
