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
		IBuildSpecifier[] GetMostRecentBuildSpecifiers(IProjectSpecifier projectSpecifier, int buildCount);
		IBuildSpecifier[] GetBuildSpecifiers(IProjectSpecifier serverSpecifier);
		void DeleteProject(IProjectSpecifier projectSpecifier, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment);
		string GetServerLog(IServerSpecifier serverSpecifier);
		string GetServerLog(IProjectSpecifier specifier);
		void Start(IProjectSpecifier projectSpecifier);
		void Stop(IProjectSpecifier projectSpecifier);
        void ForceBuild(IProjectSpecifier projectSpecifier, string enforcerName);
        void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName);
        void ForceBuild(IProjectSpecifier projectSpecifier, string enforcerName, Dictionary<string, string> parameters);
		void AbortBuild(IProjectSpecifier projectSpecifier, string enforcerName);
        void Start(IProjectSpecifier projectSpecifier, string sessionToken);
        void Stop(IProjectSpecifier projectSpecifier, string sessionToken);
        void ForceBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName, Dictionary<string, string> parameters);
        void AbortBuild(IProjectSpecifier projectSpecifier, string sessionToken, string enforcerName);
		ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(string sessionToken);
        ProjectStatusListAndExceptions GetProjectStatusListAndCaptureExceptions(IServerSpecifier serverSpecifier, string sessionToken);
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
        List<ParameterBase> ListBuildParameters(IProjectSpecifier projectSpecifier);

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
        RemotingFileTransfer RetrieveFileTransfer(IProjectSpecifier projectSpecifier, string fileName);

        RemotingFileTransfer RetrieveFileTransfer(IBuildSpecifier buildSpecifier, string fileName);
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// List the available packages for a project.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        PackageDetails[] RetrievePackageList(IProjectSpecifier projectSpecifier);

        /// <summary>
        /// List the available packages for a build.
        /// </summary>
        /// <param name="projectSpecifier"></param>
        /// <returns></returns>
        PackageDetails[] RetrievePackageList(IBuildSpecifier buildSpecifier);
        #endregion

        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        ProjectStatusSnapshot TakeStatusSnapshot(IProjectSpecifier projectSpecifier);
    }
}
