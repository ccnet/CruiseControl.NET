using System;
using ThoughtWorks.CruiseControl.Remote;
using System.Collections.Generic;
using ThoughtWorks.CruiseControl.Remote.Events;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
    /// <summary>
    /// Managers the integration queues.
    /// </summary>
    public interface IQueueManager
    {
        /// <summary>
        /// Starts all the projects.
        /// </summary>
        void StartAllProjects();

        /// <summary>
        /// Stops all the projects.
        /// </summary>
        void StopAllProjects();

        /// <summary>
        /// Aborts all running projects and stops queue processing.
        /// </summary>
        void Abort();

        /// <summary>
        /// Stops all running projects and regenerates the queues.
        /// </summary>
        /// <param name="configuration">The configuration to use.</param>
        void Restart(IConfiguration configuration);

        /// <summary>
        /// Starts a specific project.
        /// </summary>
        /// <param name="project">The name of the project to start.</param>
        void Start(string project);

        /// <summary>
        /// Stops a specific project.
        /// </summary>
        /// <param name="project">The name of the project to stop.</param>
        void Stop(string project);

        /// <summary>
        /// Starts a forced build for a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <param name="enforcerName">The person forcing the build.</param>
        /// <param name="buildValues"></param>
        void ForceBuild(string projectName, string enforcerName, Dictionary<string, string> buildValues);

        /// <summary>
        /// Adds a request for a project.
        /// </summary>
        /// <param name="project">The name of the project.</param>
        /// <param name="request">The request to add.</param>
        void Request(string project, IntegrationRequest request);

        /// <summary>
        /// Cancels a request to start a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        void CancelPendingRequest(string projectName);

        /// <summary>
        /// Waits for a project to exit.
        /// </summary>
        /// <param name="projectName">The name of the project to wait for.</param>
        void WaitForExit(string projectName);

        /// <summary>
        /// Gets a snapshot of the current server status.
        /// </summary>
        /// <returns>A snapshot of the server status.</returns>
        CruiseServerSnapshot GetCruiseServerSnapshot();

        /// <summary>
        /// Gets the statuses of the projects.
        /// </summary>
        /// <returns>A list of the current statuses for the projects.</returns>
        ProjectStatus[] GetProjectStatuses();

        /// <summary>
        /// Retrieves the integrator for a project.
        /// </summary>
        /// <param name="projectName">The name of the project.</param>
        /// <returns>The integrator for the project.</returns>
        IProjectIntegrator GetIntegrator(string projectName);

        /// <summary>
        /// Associates the integration events.
        /// </summary>
        /// <param name="integrationStarted"></param>
        /// <param name="integrationCompleted"></param>
        void AssociateIntegrationEvents(EventHandler<IntegrationStartedEventArgs> integrationStarted,
            EventHandler<IntegrationCompletedEventArgs> integrationCompleted);
    }
}
