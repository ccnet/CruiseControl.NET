using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Exposes project management functionality (start, stop, status) via remoting.  
	/// The CCTray is one such example of an application that may make use of this remote interface.
	/// </summary>
	public class CruiseManager : MarshalByRefObject, ICruiseManager
	{
		private readonly ICruiseServer cruiseServer;

		public CruiseManager(ICruiseServer cruiseServer)
		{
			this.cruiseServer = cruiseServer;
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return cruiseServer.GetProjectStatus();
		}

        #region Unsecured Build Methods
		public void ForceBuild(string project, string enforcerName)
		{
            cruiseServer.ForceBuild(null, project, enforcerName);
		}

		public void AbortBuild(string project, string enforcerName)
		{
            cruiseServer.AbortBuild(null, project, enforcerName);
		}
		
		public void Request(string projectName, IntegrationRequest integrationRequest)
		{
            cruiseServer.Request(null, projectName, integrationRequest);
		}

		public void Start(string project)
		{
            cruiseServer.Start(null, project);
		}

		public void Stop(string project)
		{
            cruiseServer.Stop(null, project);
		}

		public void SendMessage(string projectName, Message message)
		{
            cruiseServer.SendMessage(null, projectName, message);
        }
        #endregion



        #region Secured Build Methods
        public void ForceBuild(string sessionToken, string projectName, string enforcerName)
        {
            cruiseServer.ForceBuild(sessionToken, projectName, enforcerName);
        }

        public void AbortBuild(string sessionToken, string projectName, string enforcerName)
        {
            cruiseServer.AbortBuild(sessionToken, projectName, enforcerName);
        }

        public void Request(string sessionToken, string projectName, IntegrationRequest integrationRequest)
        {
            cruiseServer.Request(sessionToken, projectName, integrationRequest);
        }

        public void Start(string sessionToken, string project)
        {
            cruiseServer.Start(sessionToken, project);
        }

        public void Stop(string sessionToken, string project)
        {
            cruiseServer.Stop(sessionToken, project);
		}

        public void SendMessage(string sessionToken, string projectName, Message message)
        {
            cruiseServer.SendMessage(sessionToken, projectName, message);
        }

        public void CancelPendingRequest(string sessionToken, string projectName)
        {
            cruiseServer.CancelPendingRequest(sessionToken, projectName);
        }
        #endregion


		public void WaitForExit(string project)
		{
			cruiseServer.WaitForExit(project);
		}

		public void CancelPendingRequest(string projectName)
		{
            cruiseServer.CancelPendingRequest(null, projectName);
		}
		
		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			return cruiseServer.GetCruiseServerSnapshot();
		}

		public string GetLatestBuildName(string projectName)
		{
			return cruiseServer.GetLatestBuildName(projectName);
		}

		public string[] GetBuildNames(string projectName)
		{
			return cruiseServer.GetBuildNames(projectName);
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			try
			{
				return cruiseServer.GetMostRecentBuildNames(projectName, buildCount);
			}
			catch (Exception e)
			{
				Log.Error(e);
				throw new CruiseControlException("Unexpected exception caught on server", e);
			}
		}

		public string GetLog(string projectName, string buildName)
		{
			return cruiseServer.GetLog(projectName, buildName);
		}

		public string GetServerLog()
		{
			return cruiseServer.GetServerLog();
		}

		public string GetServerLog(string projectName)
		{
			return cruiseServer.GetServerLog(projectName);
		}

		public void AddProject(string serializedProject)
		{
			cruiseServer.AddProject(serializedProject);
		}

		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			cruiseServer.DeleteProject(projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

		public string GetProject(string projectName)
		{
			return cruiseServer.GetProject(projectName);
		}

		public void UpdateProject(string projectName, string serializedProject)
		{
			cruiseServer.UpdateProject(projectName, serializedProject);
		}

		public ExternalLink[] GetExternalLinks(string projectName)
		{
			return cruiseServer.GetExternalLinks(projectName);
		}

		public string GetArtifactDirectory(string projectName)
		{
			return cruiseServer.GetArtifactDirectory(projectName);
		}

		public string GetStatisticsDocument(string projectName)
		{
			return cruiseServer.GetStatisticsDocument(projectName);
		}

        public string GetModificationHistoryDocument(string projectName)
        {
            return cruiseServer.GetModificationHistoryDocument(projectName);
        }

        public string GetRSSFeed(string projectName)
        {
            return cruiseServer.GetRSSFeed(projectName);
        }


		public override object InitializeLifetimeService()
		{
			return null;
		}

		public string GetServerVersion()
		{
			return cruiseServer.GetVersion();
		}

        /// <summary>
        /// Retrieves the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public long GetFreeDiskSpace()
        {
            return cruiseServer.GetFreeDiskSpace();
        }

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="source">Where to retrieve the file from.</param>
        public virtual RemotingFileTransfer RetrieveFileTransfer(string project, string fileName, FileTransferSource source)
        {
            return cruiseServer.RetrieveFileTransfer(project, fileName, source);
        }
		#endregion

        #region Security Methods

        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public string Login(ISecurityCredentials credentials)
        {
            string sessionToken = cruiseServer.Login(credentials);
            return sessionToken;
        }

        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="sesionToken"></param>
        public void Logout(string sesionToken)
        {
            cruiseServer.Logout(sesionToken);
        }

        /// <summary>
        /// Validates and stores the session token.
        /// </summary>
        /// <param name="sessionToken">The session token to validate.</param>
        /// <returns>True if the session is valid, false otherwise.</returns>
        public bool ValidateSession(string sessionToken)
        {
            return true;
        }

        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        public virtual string GetSecurityConfiguration(string sessionToken)
        {
            return cruiseServer.GetSecurityConfiguration(sessionToken);
        }

        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListAllUsers(string sessionToken)
        {
            return cruiseServer.ListAllUsers(sessionToken);
        }

        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="projectNames">The names of the projects to check.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string sessionToken, string userName, params string[] projectNames)
        {
            return cruiseServer.DiagnoseSecurityPermissions(sessionToken, userName, projectNames);
        }

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(string sessionToken, int startPosition, int numberOfRecords)
        {
            return cruiseServer.ReadAuditRecords(sessionToken, startPosition, numberOfRecords);
        }

        /// <summary>
        /// Reads all the specified number of filtered audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <param name="filter">The filter to use.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(string sessionToken, int startPosition, int numberOfRecords, IAuditFilter filter)
        {
            return cruiseServer.ReadAuditRecords(sessionToken, startPosition, numberOfRecords, filter);
        }

        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ChangePassword(string sessionToken, string oldPassword, string newPassword)
        {
            cruiseServer.ChangePassword(sessionToken, oldPassword, newPassword);
        }

        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ResetPassword(string sessionToken, string userName, string newPassword)
        {
            cruiseServer.ResetPassword(sessionToken, userName, newPassword);
        }

        #endregion
	}
}
