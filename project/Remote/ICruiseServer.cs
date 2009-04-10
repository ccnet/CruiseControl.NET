using System;
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

		void Start(string project);
		void Stop(string project);

		/// <summary>
		/// Cancel a pending project integration request from the integration queue.
		/// </summary>
		void CancelPendingRequest(string projectName);
		
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
		void ForceBuild(string projectName, string enforcerName);

		
		/// <summary>
		/// Aborts the build of the selected project.
		/// </summary>
		void AbortBuild(string projectName, string enforcerName);
		
		void Request(string projectName, IntegrationRequest request);

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
		void SendMessage(string projectName, Message message);

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

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="source">Where to retrieve the file from.</param>
        RemotingFileTransfer RetrieveFileTransfer(string project, string fileName, FileTransferSource source);
        #endregion
    }
}
