using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using System.Collections.Generic;

namespace ThoughtWorks.CruiseControl.Core
{
	public class RemoteCruiseServer : CruiseServerEventsBase, ICruiseServer
	{
		public const string URI = "CruiseManager.rem";
		public const string DefaultUri = "tcp://localhost:21234/" + URI;

		private ICruiseServer server;
		private bool disposed;
		private IExecutionEnvironment environment = new ExecutionEnvironment();

		public RemoteCruiseServer(ICruiseServer server, string remotingConfigurationFile)
		{
			this.server = server;
			RemotingConfiguration.Configure(remotingConfigurationFile, false);
			RegisterForRemoting();
		}

		public void Start()
		{
			server.Start();
		}

		public void Stop()
		{
			server.Stop();
		}

		public void Abort()
		{
			server.Abort();
		}

		public void WaitForExit()
		{
			server.WaitForExit();
		}

        public void Start(string sessionToken, string project)
		{
			server.Start(sessionToken, project);
		}

        public void Stop(string sessionToken, string project)
		{
			server.Stop(sessionToken, project);
		}

		public ICruiseManager CruiseManager
		{
			get { return server.CruiseManager; }
		}

		public ProjectStatus[] GetProjectStatus()
		{
			return server.GetProjectStatus();
		}

        public void ForceBuild(string sessionToken, string projectName, string enforcerName)
		{
            server.ForceBuild(sessionToken, projectName, enforcerName);
		}

		public void AbortBuild(string sessionToken, string projectName, string enforcerName)
		{
			server.AbortBuild(sessionToken, projectName, enforcerName);
		}
		
        public void Request(string sessionToken, string projectName, IntegrationRequest request)
		{
			server.Request(sessionToken, projectName, request);
		}

		public void WaitForExit(string projectName)
		{
			server.WaitForExit(projectName);
		}

        public void CancelPendingRequest(string sessionToken, string projectName)
		{
			server.CancelPendingRequest(sessionToken, projectName);
		}
		
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			return server.GetCruiseServerSnapshot();
		}

		public string GetLatestBuildName(string projectName)
		{
			return server.GetLatestBuildName(projectName);
		}

		public string[] GetBuildNames(string projectName)
		{
			return server.GetBuildNames(projectName);
		}

		public string GetVersion()
		{
			return server.GetVersion();
		}

		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
			return server.GetMostRecentBuildNames(projectName, buildCount);
		}

		public string GetLog(string projectName, string buildName)
		{
			return server.GetLog(projectName, buildName);
		}

		public string GetServerLog()
		{
			return server.GetServerLog();
		}

		public string GetServerLog(string projectName)
		{
			return server.GetServerLog(projectName);
		}

		public void AddProject(string serializedProject)
		{
			server.AddProject(serializedProject);
		}

		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
			server.DeleteProject(projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
		}

		public string GetProject(string name)
		{
			return server.GetProject(name);
		}

		public void UpdateProject(string projectName, string serializedProject)
		{
			server.UpdateProject(projectName, serializedProject);
		}

		public ExternalLink[] GetExternalLinks(string projectName)
		{
			return server.GetExternalLinks(projectName);
		}

        public void SendMessage(string sessionToken, string projectName, Message message)
		{
			server.SendMessage(sessionToken, projectName, message);
		}

		public string GetArtifactDirectory(string projectName)
		{
			return server.GetArtifactDirectory(projectName);
		}

		public string GetStatisticsDocument(string projectName)
		{
            return server.GetStatisticsDocument(projectName);
		}

        public string GetModificationHistoryDocument(string projectName)
        {
            return server.GetModificationHistoryDocument(projectName);
        }

        public string GetRSSFeed(string projectName)
        {
            return server.GetRSSFeed(projectName);
        }

		private void RegisterForRemoting()
		{
			MarshalByRefObject marshalByRef = (MarshalByRefObject) server.CruiseManager;
			RemotingServices.Marshal(marshalByRef, URI);

			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Registered channel: " + channel.ChannelName);

				// GetUrlsForUri is not support on cross-AppDomain channels on mono (as of 1.1.8.3)
				if (environment.IsRunningOnWindows)
				{
					if (channel is IChannelReceiver)
					{
						foreach (string url in ((IChannelReceiver) channel).GetUrlsForUri(URI))
						{
							Log.Info("CruiseManager: Listening on url: " + url);
						}
					}
				}
			}
		}

		void IDisposable.Dispose()
		{
			lock (this)
			{
				if (disposed) return;
				disposed = true;
			}
			Log.Info("Disconnecting remote server: ");
			RemotingServices.Disconnect((MarshalByRefObject) server.CruiseManager);
			foreach (IChannel channel in ChannelServices.RegisteredChannels)
			{
				Log.Info("Unregistering channel: " + channel.ChannelName);
				ChannelServices.UnregisterChannel(channel);
			}
			server.Dispose();
		}

        /// <summary>
        /// Retrieves the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public long GetFreeDiskSpace()
        {
            return server.GetFreeDiskSpace();
        }

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a status snapshot of a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The snapshot of the current status.</returns>
        public virtual ProjectStatusSnapshot TakeStatusSnapshot(string projectName)
        {
            return server.TakeStatusSnapshot(projectName);
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the latest list of packages for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList(string projectName)
        {
            return server.RetrievePackageList(projectName);
        }

        /// <summary>
        /// Retrieves the list of packages for a build for a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="buildLabel"></param>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList(string projectName, string buildLabel)
        {
            return server.RetrievePackageList(projectName, buildLabel);
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        public virtual RemotingFileTransfer RetrieveFileTransfer(string project, string fileName)
        {
            return server.RetrieveFileTransfer(project, fileName);
        }
        #endregion

        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public string Login(ISecurityCredentials credentials)
        {
            return server.Login(credentials);
	}

        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        /// <param name="sesionToken"></param>
        public void Logout(string sesionToken)
        {
            server.Logout(sesionToken);
        }

        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        public virtual string GetSecurityConfiguration(string sessionToken)
        {
            return server.GetSecurityConfiguration(sessionToken);
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
            return server.ListAllUsers(sessionToken);
        }

        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <param name="userName">The name of the user.</param>
        /// <param name="projectNames">The names of the projects to check.</param>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string sessionToken, string userName, params string[] projectNames)
        {
            return server.DiagnoseSecurityPermissions(sessionToken, userName, projectNames);
        }

        /// <summary>
        /// Reads all the specified number of audit events.
        /// </summary>
        /// <param name="startPosition">The starting position.</param>
        /// <param name="numberOfRecords">The number of records to read.</param>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(string sessionToken, int startPosition, int numberOfRecords)
        {
            return server.ReadAuditRecords(sessionToken, startPosition, numberOfRecords);
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
            return server.ReadAuditRecords(sessionToken, startPosition, numberOfRecords, filter);
        }

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="oldPassword">The person's old password.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ChangePassword(string sessionToken, string oldPassword, string newPassword)
        {
            server.ChangePassword(sessionToken, oldPassword, newPassword);
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        /// <param name="sessionToken">The session token for the current user.</param>
        /// <param name="userName">The user name to reset the password for.</param>
        /// <param name="newPassword">The person's new password.</param>
        public virtual void ResetPassword(string sessionToken, string userName, string newPassword)
        {
            server.ResetPassword(sessionToken, userName, newPassword);
        }
        #endregion
    }
}
