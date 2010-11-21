

using System;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;
using ThoughtWorks.CruiseControl.Remote.Parameters;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.Core
{
	/// <summary>
	/// Exposes project management functionality (start, stop, status) via remoting.  
	/// The CCTray is one such example of an application that may make use of this remote interface.
	/// </summary>
    [Obsolete("Use ICruiseServerClient instead")]
	public class CruiseManager : MarshalByRefObject, ICruiseManager
	{
		private readonly ICruiseServer cruiseServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="CruiseManager" /> class.	
        /// </summary>
        /// <param name="cruiseServer">The cruise server.</param>
        /// <remarks></remarks>
        public CruiseManager(ICruiseServer cruiseServer)
		{
			this.cruiseServer = cruiseServer;
		}

        /// <summary>
        /// Gets the actual server.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
        public ICruiseServer ActualServer
        {
            get { return this.cruiseServer; }
        }

        /// <summary>
        /// Initializes the lifetime service.	
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        public override object InitializeLifetimeService()
		{
            return null;
		}

        /// <summary>
        /// Gets information about the last build status, current activity and project name.
        /// for all projects on a cruise server
        /// </summary>
        public ProjectStatus[] GetProjectStatus()
        {
            ProjectStatusResponse resp = cruiseServer.GetProjectStatus(GenerateServerRequest());
            ValidateResponse(resp);
            return resp.Projects.ToArray();
        }

        /// <summary>
        /// Forces a build for the named project.
        /// </summary>
        /// <param name="projectName">project to force</param>
        /// <param name="enforcerName">ID of trigger/action forcing the build</param>
        public void ForceBuild(string projectName, string enforcerName)
        {
            Response resp = cruiseServer.ForceBuild(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
        }

        /// <summary>
        /// Aborts the build.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="enforcerName">Name of the enforcer.</param>
        /// <remarks></remarks>
        public void AbortBuild(string projectName, string enforcerName)
		{
            Response resp = cruiseServer.AbortBuild(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
		}

        /// <summary>
        /// Requests the specified project name.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="integrationRequest">The integration request.</param>
        /// <remarks></remarks>
		public void Request(string projectName, IntegrationRequest integrationRequest)
		{
            BuildIntegrationRequest request = new BuildIntegrationRequest(null, projectName);
            request.BuildCondition = integrationRequest.BuildCondition;
            Response resp = cruiseServer.ForceBuild(request);
            ValidateResponse(resp);
		}

        /// <summary>
        /// Starts the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
		public void Start(string project)
		{
            Response resp = cruiseServer.Start(GenerateProjectRequest(project));
            ValidateResponse(resp);
		}

        /// <summary>
        /// Stops the specified project.	
        /// </summary>
        /// <param name="project">The project.</param>
        /// <remarks></remarks>
		public void Stop(string project)
		{
            Response resp = cruiseServer.Stop(GenerateProjectRequest(project));
            ValidateResponse(resp);
		}

        /// <summary>
        /// Sends the message.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="message">The message.</param>
        /// <remarks></remarks>
		public void SendMessage(string projectName, Message message)
		{
            MessageRequest request = new MessageRequest();
            request.ProjectName = projectName;
            request.Message = message.Text;
            request.Kind = message.Kind;
            Response resp = cruiseServer.SendMessage(request);
            ValidateResponse(resp);
        }

        /// <summary>
        /// Waits for exit.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <remarks></remarks>
        public void WaitForExit(string projectName)
        {
            Response resp = cruiseServer.WaitForExit(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
		}

        /// <summary>
        /// Cancel a pending project integration request from the integration queue.
        /// </summary>
		public void CancelPendingRequest(string projectName)
		{
            Response resp = cruiseServer.CancelPendingRequest(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
		}
		
		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
            SnapshotResponse resp = cruiseServer.GetCruiseServerSnapshot(GenerateServerRequest());
            ValidateResponse(resp);
            return resp.Snapshot;
		}

        /// <summary>
        /// Returns the name of the most recent build for the specified project
        /// </summary>
		public string GetLatestBuildName(string projectName)
		{
            DataResponse resp = cruiseServer.GetLatestBuildName(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Returns the names of all builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
		public string[] GetBuildNames(string projectName)
		{
            DataListResponse resp = cruiseServer.GetBuildNames(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data.ToArray();
		}

        /// <summary>
        /// Returns the names of the buildCount most recent builds for the specified project, sorted s.t. the newest build is first in the array
        /// </summary>
		public string[] GetMostRecentBuildNames(string projectName, int buildCount)
		{
            BuildListRequest request = new BuildListRequest(null, projectName);
            request.NumberOfBuilds = buildCount;
            DataListResponse resp = cruiseServer.GetMostRecentBuildNames(request);
            ValidateResponse(resp);
            return resp.Data.ToArray();
		}

        /// <summary>
        /// Returns the build log contents for requested project and build name
        /// </summary>
		public string GetLog(string projectName, string buildName)
		{
            BuildRequest request = new BuildRequest(null, projectName);
            request.BuildName = buildName;
            DataResponse resp = cruiseServer.GetLog(request);
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Returns a log of recent build server activity. How much information that is returned is configured on the build server.
        /// </summary>
		public string GetServerLog()
		{
            DataResponse resp = cruiseServer.GetServerLog(GenerateServerRequest());
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Returns a log of recent build server activity for a specific project. How much information that is returned is configured on the build server.
        /// </summary>
		public string GetServerLog(string projectName)
		{
            DataResponse resp = cruiseServer.GetServerLog(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data;
        }

        /// <summary>
        /// Returns the version of the server
        /// </summary>
        public string GetServerVersion()
        {
            DataResponse resp = cruiseServer.GetServerVersion(GenerateServerRequest());
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Adds a project to the server
        /// </summary>
		public void AddProject(string serializedProject)
		{
            ChangeConfigurationRequest request = new ChangeConfigurationRequest();
            request.ProjectDefinition = serializedProject;
            Response resp = cruiseServer.AddProject(request);
            ValidateResponse(resp);
		}

        /// <summary>
        /// Deletes the specified project from the server
        /// </summary>
		public void DeleteProject(string projectName, bool purgeWorkingDirectory, bool purgeArtifactDirectory, bool purgeSourceControlEnvironment)
		{
            ChangeConfigurationRequest request = new ChangeConfigurationRequest(null, projectName);
            request.PurgeWorkingDirectory = purgeWorkingDirectory;
            request.PurgeArtifactDirectory = purgeArtifactDirectory;
            request.PurgeSourceControlEnvironment = purgeSourceControlEnvironment;
            Response resp = cruiseServer.DeleteProject(request);
            ValidateResponse(resp);
		}

        /// <summary>
        /// Returns the serialized form of the requested project from the server
        /// </summary>
		public string GetProject(string projectName)
		{
            DataResponse resp = cruiseServer.GetProject(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Updates the selected project on the server
        /// </summary>
		public void UpdateProject(string projectName, string serializedProject)
		{
            ChangeConfigurationRequest request = new ChangeConfigurationRequest(null, projectName);
            request.ProjectDefinition = serializedProject;
            Response resp = cruiseServer.UpdateProject(request);
            ValidateResponse(resp);
		}

        /// <summary>
        /// Gets the external links.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public ExternalLink[] GetExternalLinks(string projectName)
		{
            ExternalLinksListResponse resp = cruiseServer.GetExternalLinks(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.ExternalLinks.ToArray();
		}

        /// <summary>
        /// Gets the artifact directory.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string GetArtifactDirectory(string projectName)
		{
            DataResponse resp = cruiseServer.GetArtifactDirectory(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Gets the statistics document.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		public string GetStatisticsDocument(string projectName)
		{
            DataResponse resp = cruiseServer.GetStatisticsDocument(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Gets the modification history document.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetModificationHistoryDocument(string projectName)
        {
            DataResponse resp = cruiseServer.GetModificationHistoryDocument(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data;
        }

        /// <summary>
        /// Gets the RSS feed.	
        /// </summary>
        /// <param name="projectName">Name of the project.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        public string GetRSSFeed(string projectName)
        {
            DataResponse resp = cruiseServer.GetRSSFeed(GenerateProjectRequest(projectName));
            ValidateResponse(resp);
            return resp.Data;
		}

        /// <summary>
        /// Retrieves the amount of free disk space.
        /// </summary>
        /// <returns></returns>
        public long GetFreeDiskSpace()
        {
            DataResponse resp = cruiseServer.GetFreeDiskSpace(GenerateServerRequest());
            ValidateResponse(resp);
            return Convert.ToInt64(resp.Data);
        }

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        public virtual RemotingFileTransfer RetrieveFileTransfer(string project, string fileName)
        {
            var request = new FileTransferRequest();
            request.ProjectName = project;
            request.FileName = fileName;
            var response = cruiseServer.RetrieveFileTransfer(request);
            ValidateResponse(response);
            return response.FileTransfer as RemotingFileTransfer;
        }
		#endregion

        #region Helper methods - conversion from old to new
        #region GenerateServerRequest()
        /// <summary>
        /// Generate a server request.
        /// </summary>
        /// <returns></returns>
        private ServerRequest GenerateServerRequest()
        {
            ServerRequest request = new ServerRequest();
            return request;
        }
        #endregion

        #region GenerateProjectRequest()
        /// <summary>
        /// Generate a project request.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        private ProjectRequest GenerateProjectRequest(string projectName)
        {
            ProjectRequest request = new ProjectRequest();
            request.ProjectName = projectName;
            return request;
        }
        #endregion

        #region ValidateResponse()
        /// <summary>
        /// Validate the response from the server.
        /// </summary>
        /// <param name="response"></param>
        private void ValidateResponse(Response response)
        {
            if (response.Result == ResponseResult.Failure)
        {
                string message = "Request request has failed on the remote server:" + Environment.NewLine +
                    response.ConcatenateErrors();
                throw new CruiseControlException(message);
        }
        }
        #endregion
        #endregion
    }
}
