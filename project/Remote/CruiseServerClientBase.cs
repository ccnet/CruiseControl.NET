using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A base class to implement client-side communications with a CruiseControl.NET server.
    /// </summary>
    public abstract class CruiseServerClientBase
    {
        #region Private fields
        private string sessionToken;
        private object lockObject = new object();
        #endregion

        #region Public properties
        #region TargetServer
        /// <summary>
        /// The server that will be targeted by all messages.
        /// </summary>
        public abstract string TargetServer { get; set; }
        #endregion

        #region SessionToken
        /// <summary>
        /// The session token to use.
        /// </summary>
        public virtual string SessionToken
        {
            get { return sessionToken; }
            set { sessionToken = value; }
        }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this client busy performing an operation.
        /// </summary>
        public abstract bool IsBusy { get; }
        #endregion

        #region IsLoggedIn
        /// <summary>
        /// Is this client currently logged in.
        /// </summary>
        public virtual bool IsLoggedIn
        {
            get { return !string.IsNullOrEmpty(SessionToken); }
        }
        #endregion

        #region Address
        /// <summary>
        /// The address of the client.
        /// </summary>
        public abstract string Address { get; }
        #endregion
        #endregion

        #region Public methods
        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public virtual ProjectStatus[] GetProjectStatus()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="projectName">project to force</param>
        public virtual void ForceBuild(string projectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Forces a build for the named project with some parameters.
        /// </summary>
        /// <param name="projectName">project to force</param>
        /// <param name="parameters"></param>
        public virtual void ForceBuild(string projectName, List<NameValuePair> parameters)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Attempts to abort a current project build.
        /// </summary>
        /// <param name="projectName">The name of the project to abort.</param>
        public virtual void AbortBuild(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Request()
        /// <summary>
        /// Sends a build request to the server.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="integrationRequest"></param>
        public virtual void Request(string projectName, IntegrationRequest integrationRequest)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region StartProject()
        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="project"></param>
        public virtual void StartProject(string project)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region StopProject()
        /// <summary>
        /// Stop project.
        /// </summary>
        /// <param name="project"></param>
        public virtual void StopProject(string project)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Sends a message for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="message"></param>
        public virtual void SendMessage(string projectName, Message message)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region WaitForExit()
        /// <summary>
        /// Waits for a project to stop.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        public virtual void WaitForExit(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region CancelPendingRequest()
        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        public virtual void CancelPendingRequest(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        public virtual CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetLatestBuildName()
        /// <summary>
        /// Returns the name of the most recent build for the specified project
        /// </summary>
        public virtual string GetLatestBuildName(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetBuildNames()
        /// <summary>
        /// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public virtual string[] GetBuildNames(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetMostRecentBuildNames()
        /// <summary>
        /// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public virtual string[] GetMostRecentBuildNames(string projectName, int buildCount)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetLog()
        /// <summary>
        /// Returns the build log contents for requested project and build name
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="buildName">Name of the build.</param>
        /// <returns>The log file for the build.</returns>
        public string GetLog(string projectName, string buildName)
        {
            return this.GetLog(projectName, buildName, false);
        }

        /// <summary>
        /// Returns the build log contents for requested project and build name optionally using compression.
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="buildName">Name of the build.</param>
        /// <param name="compress">If set to <c>true</c> the log will be compressed.</param>
        /// <returns>The log file for the build.</returns>
        public virtual string GetLog(string projectName, string buildName, bool compress)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
        /// </summary>
        public virtual string GetServerLog()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Returns a log of recent build server activity for a specific project. How much information that is returned is configured on the build server.
        /// </summary>
        public virtual string GetServerLog(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetServerVersion()
        /// <summary>
        /// Returns the version of the server
        /// </summary>
        public virtual string GetServerVersion()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region AddProject()
        /// <summary>
        /// Adds a project to the server
        /// </summary>
        public virtual void AddProject(string serializedProject)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region DeleteProject()
        /// <summary>
        /// Deletes the specified project from the server
        /// </summary>
        public virtual void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetProject()
        /// <summary>
        /// Returns the serialized form of the requested project from the server
        /// </summary>
        public virtual string GetProject(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region UpdateProject()
        /// <summary>
        /// Updates the selected project on the server
        /// </summary>
        public virtual void UpdateProject(string projectName, string serializedProject)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetExternalLinks()
        /// <summary>
        /// Retrieves any external links.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public virtual ExternalLink[] GetExternalLinks(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetArtifactDirectory()
        /// <summary>
        /// Retrieves the name of the artifact directory.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public virtual string GetArtifactDirectory(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetStatisticsDocument()
        /// <summary>
        /// Retrieves the statistics document.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public virtual string GetStatisticsDocument(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetModificationHistoryDocument()
        /// <summary>
        /// Retrieves a document containing the history of all the changes.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public virtual string GetModificationHistoryDocument(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetRSSFeed()
        /// <summary>
        /// Retrieves the RSS feed URL.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public virtual string GetRSSFeed(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Login()
        /// <summary>
        /// Logs a user into the session and generates a session.
        /// </summary>
        /// <returns>True if the request is successful, false otherwise.</returns>
        public virtual bool Login(List<NameValuePair> Credentials)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Logout()
        /// <summary>
        /// Logs a user out of the system and removes their session.
        /// </summary>
        public virtual void Logout()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetSecurityConfiguration()
        /// <summary>
        /// Retrieves the security configuration.
        /// </summary>
        public virtual string GetSecurityConfiguration()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ListUsers()
        /// <summary>
        /// Lists all the users who have been defined in the system.
        /// </summary>
        /// <returns>
        /// A list of <see cref="UserDetails"/> containing the details on all the users
        /// who have been defined.
        /// </returns>
        public virtual List<UserDetails> ListUsers()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region DiagnoseSecurityPermissions()
        /// <summary>
        /// Checks the security permissions for a user against one or more projects.
        /// </summary>
        /// <returns>A set of diagnostics information.</returns>
        public virtual List<SecurityCheckDiagnostics> DiagnoseSecurityPermissions(string userName, params string[] projects)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ReadAuditRecords()
        /// <summary>
        /// Reads the specified number of audit events.
        /// </summary>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startRecord, int numberOfRecords)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the specified number of filtered audit events.
        /// </summary>
        /// <returns>A list of <see cref="AuditRecord"/>s containing the audit details that match the filter.</returns>
        public virtual List<AuditRecord> ReadAuditRecords(int startRecord, int numberOfRecords, AuditFilterBase filter)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ListBuildParameters()
        /// <summary>
        /// Lists the build parameters for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to retrieve the parameters for.</param>
        /// <returns>The list of parameters (if any).</returns>
        public virtual List<ParameterBase> ListBuildParameters(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ChangePassword()
        /// <summary>
        /// Changes the password of the user.
        /// </summary>
        public virtual void ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ResetPassword()
        /// <summary>
        /// Resets the password for a user.
        /// </summary>
        public virtual void ResetPassword(string userName, string newPassword)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region TakeStatusSnapshot()
        /// <summary>
        /// Takes a snapshot of the current project status.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The current status snapshot.</returns>
        public virtual ProjectStatusSnapshot TakeStatusSnapshot(string projectName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of packages for a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The currently available packages.</returns>
        public virtual List<PackageDetails> RetrievePackageList(string projectName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves the current list of packages for a build within a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="buildLabel">The label of the build.</param>
        /// <returns>The currently available packages.</returns>
        public virtual List<PackageDetails> RetrievePackageList(string projectName, string buildLabel)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieves a file transfer instance.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file transfer instance.</returns>
        public virtual IFileTransfer RetrieveFileTransfer(string projectName, string fileName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetFreeDiskSpace()
        /// <summary>
        /// Retrieve the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public virtual long GetFreeDiskSpace()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region GetLinkedSiteId()
        /// <summary>
        /// Retrieve the identifer for this project on a linked site.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="siteName"></param>
        /// <returns></returns>
        public virtual string GetLinkedSiteId(string projectName, string siteName)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ProcessMessage()
        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message in an XML format.</param>
        /// <returns>The response message in an XML format.</returns>
        public virtual string ProcessMessage(string action, string message)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Processes a message.
        /// </summary>
        /// <param name="action">The action to use.</param>
        /// <param name="message">The request message.</param>
        /// <returns>The response message.</returns>
        public virtual Response ProcessMessage(string action, ServerRequest message)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region ProcessSingleAction()
        /// <summary>
        /// Process a single action within a monitor lock.
        /// </summary>
        /// <typeparam name="T">The type of parameter to pass.</typeparam>
        /// <param name="action">The action to process.</param>
        /// <param name="value">The value to pass to the action.</param>
        public void ProcessSingleAction<T>(Action<T> action, T value)
        {
            lock (lockObject)
            {
                action(value);
            }
        }
        #endregion

        #region ListServers()
        /// <summary>
        /// Lists the servers that this client connection exposes.
        /// </summary>
        /// <returns>The list of available servers.</returns>
        public virtual IEnumerable<string> ListServers()
        {
            throw new NotImplementedException();
        }
        #endregion
        #endregion

        #region Public events
        #region RequestSending
        /// <summary>
        /// A request message is being sent.
        /// </summary>
        public event EventHandler<CommunicationsEventArgs> RequestSending;
        #endregion

        #region ResponseReceived
        /// <summary>
        /// A response message has been received.
        /// </summary>
        public event EventHandler<CommunicationsEventArgs> ResponseReceived;
        #endregion
        #endregion

        #region Protected methods
        #region FireRequestSending
        /// <summary>
        /// Fires the <see cref="RequestSending"/> event.
        /// </summary>
        /// <param name="action">The action that is being sent.</param>
        /// <param name="request">The request that is being sent.</param>
        protected virtual void FireRequestSending(string action, ServerRequest request)
        {
            if (RequestSending != null)
            {
                RequestSending(this, new CommunicationsEventArgs(action, request));
            }
        }
        #endregion

        #region FireResponseReceived
        /// <summary>
        /// Fires the <see cref="ResponseReceived"/> event.
        /// </summary>
        /// <param name="action">The action that the response is for.</param>
        /// <param name="response">The response that was received.</param>
        protected virtual void FireResponseReceived(string action, Response response)
        {
            if (ResponseReceived != null)
            {
                ResponseReceived(this, new CommunicationsEventArgs(action, response));
            }
        }
        #endregion
        #endregion
    }
}
