#pragma warning disable 0618
using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// A client connection to an old (pre-1.5) version of CruiseControl.NET via .NET Remoting.
    /// </summary>
    public class CruiseServerRemotingClient
        : CruiseServerClientBase
    {
        #region Private fields
        private readonly string serverUri;
        private string targetServer;
        private ICruiseManager manager;
        private string userName = Environment.UserName;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialises a new <see cref="CruiseServerRemotingClient"/>.
        /// </summary>
        /// <param name="serverAddress">The address of the server.</param>
        public CruiseServerRemotingClient(string serverAddress)
        {
            UriBuilder builder = new UriBuilder(serverAddress);
            if (builder.Port == -1) builder.Port = 21234;
            var uri = new Uri(builder.Uri, "/CruiseManager.rem");
            this.serverUri = uri.AbsoluteUri;
            this.manager = (ICruiseManager)RemotingServices.Connect(typeof(ICruiseManager), serverUri);
        }
        #endregion

        #region Public properties
        #region TargetServer
        /// <summary>
        /// The server that will be targeted by all messages.
        /// </summary>
        public override string TargetServer
        {
            get
            {
                if (string.IsNullOrEmpty(targetServer))
                {
                    var targetUri = new Uri(serverUri);
                    return targetUri.Host;
                }
                else
                {
                    return targetServer;
                }
            }
            set { targetServer = value; }
        }
        #endregion

        #region IsBusy
        /// <summary>
        /// Is this client busy performing an operation.
        /// </summary>
        public override bool IsBusy
        {
            get { return false; }
        }
        #endregion

        #region Address
        /// <summary>
        /// The address of the client.
        /// </summary>
        public override string Address
        {
            get { return serverUri; }
        }
        #endregion
        #endregion

        #region Public methods
        #region GetProjectStatus()
        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public override ProjectStatus[] GetProjectStatus()
        {
            var response = manager.GetProjectStatus();
            return response;
        }
        #endregion

        #region ForceBuild()
        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="projectName">project to force</param>
        public override void ForceBuild(string projectName)
        {
            manager.ForceBuild(projectName, userName);
        }

        /// <summary>
        /// Forces a build for the named project with some parameters.
        /// </summary>
        /// <param name="projectName">project to force</param>
        /// <param name="parameters"></param>
        public override void ForceBuild(string projectName, List<NameValuePair> parameters)
        {
            ForceBuild(projectName);
        }
        #endregion

        #region AbortBuild()
        /// <summary>
        /// Attempts to abort a current project build.
        /// </summary>
        /// <param name="projectName">The name of the project to abort.</param>
        public override void AbortBuild(string projectName)
        {
            manager.AbortBuild(projectName, userName);
        }
        #endregion

        #region Request()
        /// <summary>
        /// Sends a build request to the server.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="integrationRequest"></param>
        public override void Request(string projectName, IntegrationRequest integrationRequest)
        {
            manager.Request(projectName, integrationRequest);
        }
        #endregion

        #region StartProject()
        /// <summary>
        /// Attempts to start a project.
        /// </summary>
        /// <param name="project"></param>
        public override void StartProject(string project)
        {
            manager.Start(project);
        }
        #endregion

        #region StopProject()
        /// <summary>
        /// Stop project.
        /// </summary>
        /// <param name="project"></param>
        public override void StopProject(string project)
        {
            manager.Stop(project);
        }
        #endregion

        #region SendMessage()
        /// <summary>
        /// Sends a message for a project.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <param name="message"></param>
        public override void SendMessage(string projectName, Message message)
        {
            manager.SendMessage(projectName, message);
        }
        #endregion

        #region WaitForExit()
        /// <summary>
        /// Waits for a project to stop.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        public override void WaitForExit(string projectName)
        {
            manager.WaitForExit(projectName);
        }
        #endregion

        #region CancelPendingRequest()
        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
        public override void CancelPendingRequest(string projectName)
        {
            manager.CancelPendingRequest(projectName);
        }
        #endregion

        #region GetCruiseServerSnapshot()
        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        public override CruiseServerSnapshot GetCruiseServerSnapshot()
        {
            var response = manager.GetCruiseServerSnapshot();
            return response;
        }
        #endregion

        #region GetLatestBuildName()
        /// <summary>
        /// Returns the name of the most recent build for the specified project
        /// </summary>
        public override string GetLatestBuildName(string projectName)
        {
            var response = manager.GetLatestBuildName(projectName);
            return response;
        }
        #endregion

        #region GetBuildNames()
        /// <summary>
        /// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public override string[] GetBuildNames(string projectName)
        {
            var response = manager.GetBuildNames(projectName);
            return response;
        }
        #endregion

        #region GetMostRecentBuildNames()
        /// <summary>
        /// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
        public override string[] GetMostRecentBuildNames(string projectName, int buildCount)
        {
            var response = manager.GetMostRecentBuildNames(projectName, buildCount);
            return response;
        }
        #endregion

        #region GetLog()
        /// <summary>
        /// Returns the build log contents for requested project and build name
        /// </summary>
        public override string GetLog(string projectName, string buildName)
        {
            var response = manager.GetLog(projectName, buildName);
            return response;
        }
        #endregion

        #region GetServerLog()
        /// <summary>
        /// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
        /// </summary>
        public override string GetServerLog()
        {
            var response = manager.GetServerLog();
            return response;
        }

        /// <summary>
        /// Returns a log of recent build server activity for a specific project. How much information that is returned is configured on the build server.
        /// </summary>
        public override string GetServerLog(string projectName)
        {
            var response = manager.GetServerLog(projectName);
            return response;
        }
        #endregion

        #region GetServerVersion()
        /// <summary>
        /// Returns the version of the server
        /// </summary>
        public override string GetServerVersion()
        {
            var response = manager.GetServerVersion();
            return response;
        }
        #endregion

        #region AddProject()
        /// <summary>
        /// Adds a project to the server
        /// </summary>
        public override void AddProject(string serializedProject)
        {
            manager.AddProject(serializedProject);
        }
        #endregion

        #region DeleteProject()
        /// <summary>
        /// Deletes the specified project from the server
        /// </summary>
        public override void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
        {
            manager.DeleteProject(projectName, purgeWorkingDirectory, purgeArtifactDirectory, purgeSourceControlEnvironment);
        }
        #endregion

        #region GetProject()
        /// <summary>
        /// Returns the serialized form of the requested project from the server
        /// </summary>
        public override string GetProject(string projectName)
        {
            var response = manager.GetProject(projectName);
            return response;
        }
        #endregion

        #region UpdateProject()
        /// <summary>
        /// Updates the selected project on the server
        /// </summary>
        public override void UpdateProject(string projectName, string serializedProject)
        {
            manager.UpdateProject(projectName, serializedProject);
        }
        #endregion

        #region GetExternalLinks()
        /// <summary>
        /// Retrieves any external links.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public override ExternalLink[] GetExternalLinks(string projectName)
        {
            var response = manager.GetExternalLinks(projectName);
            return response;
        }
        #endregion

        #region GetArtifactDirectory()
        /// <summary>
        /// Retrieves the name of the artifact directory.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public override string GetArtifactDirectory(string projectName)
        {
            var response = manager.GetArtifactDirectory(projectName);
            return response;
        }
        #endregion

        #region GetStatisticsDocument()
        /// <summary>
        /// Retrieves the statistics document.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public override string GetStatisticsDocument(string projectName)
        {
            var response = manager.GetStatisticsDocument(projectName);
            return response;
        }
        #endregion

        #region GetModificationHistoryDocument()
        /// <summary>
        /// Retrieves a document containing the history of all the changes.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public override string GetModificationHistoryDocument(string projectName)
        {
            var response = manager.GetModificationHistoryDocument(projectName);
            return response;
        }
        #endregion

        #region GetRSSFeed()
        /// <summary>
        /// Retrieves the RSS feed URL.
        /// </summary>
        /// <param name="projectName">The name of the project to use.</param>
        /// <returns></returns>
        public override string GetRSSFeed(string projectName)
        {
            var response = manager.GetRSSFeed(projectName);
            return response;
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieves a file transfer instance.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <returns>The file transfer instance.</returns>
        public override IFileTransfer RetrieveFileTransfer(string projectName, string fileName)
        {
            var response = manager.RetrieveFileTransfer(projectName, fileName);
            return response;
        }
        #endregion
        #endregion
    }
}
