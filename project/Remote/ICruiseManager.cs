using System;
namespace ThoughtWorks.CruiseControl.Remote
{
	/// <remarks>
	/// Remote Interface to CruiseControl.NET.
	/// </remarks>
    [Obsolete("Use ICruiseServerClient instead")]
	public interface ICruiseManager
	{
		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// for all projects on a cruise server
		/// </summary>
		ProjectStatus [] GetProjectStatus();

		/// <summary>
		/// Forces a build for the named project.
		/// </summary>
		/// <param name="projectName">project to force</param>
        /// <param name="enforcerName">ID of trigger/action forcing the build</param>
		void ForceBuild(string projectName, string enforcerName );

        /// <summary>
        /// Abort a build.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="enforcerName"></param>
		void AbortBuild(string projectName, string enforcerName);

        /// <summary>
        /// Send a build request.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="integrationRequest"></param>
		void Request(string projectName, IntegrationRequest integrationRequest);

        /// <summary>
        /// Start a project.
        /// </summary>
        /// <param name="project"></param>
		void Start(string project);

        /// <summary>
        /// Stop a project.
        /// </summary>
        /// <param name="project"></param>
		void Stop(string project);

        /// <summary>
        /// Send a project message.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="message"></param>
		void SendMessage(string projectName, Message message);

        /// <summary>
        /// Wait for the project to exit.
        /// </summary>
        /// <param name="projectName"></param>
		void WaitForExit(string projectName);

		/// <summary>
		/// Cancel a pending project integration request from the integration queue.
		/// </summary>
		void CancelPendingRequest(string projectName);

        /// <summary>
        /// Gets the projects and integration queues snapshot from this server.
        /// </summary>
        CruiseServerSnapshot GetCruiseServerSnapshot();

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
		/// Returns a log of recent build server activity for a specific project. How much information that is returned is configured on the build server.
		/// </summary>
		string GetServerLog(string projectName);

		/// <summary>
		/// Returns the version of the server
		/// </summary>
		string GetServerVersion();

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
		string GetProject(string projectName);

		/// <summary>
		/// Updates the selected project on the server
		/// </summary>
		void UpdateProject(string projectName, string serializedProject);

        /// <summary>
        /// Get the external links.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
		ExternalLink[] GetExternalLinks(string projectName);

        /// <summary>
        /// get the artefact directory.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
		string GetArtifactDirectory(string projectName);

        /// <summary>
        /// get the statistics document.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
		string GetStatisticsDocument(string projectName);

        /// <summary>
        /// Get the modification history document.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        string GetModificationHistoryDocument(string projectName);

        /// <summary>
        /// Get the RSS feed.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
        string GetRSSFeed(string projectName);

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
        RemotingFileTransfer RetrieveFileTransfer(string project, string fileName);
        #endregion
    }
}
