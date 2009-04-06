using System.Collections;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Interface for the project integrators to communicate with for adding their integration
	/// requests to a queue.
	/// </summary>
	public interface IIntegrationQueue : IList
	{
		string Name { get; }

        bool IsLocked { get; }

        /// <summary>
        /// The configuration settings for this queue.
        /// </summary>
        IQueueConfiguration Configuration { get; }

		/// <summary>
		/// Add a project integration request be added to the integration queue.
		/// If no requests are on that queue already the integration is just kicked off immediately.
		/// If the request is a force build and an integration is already on the queue for that project
		/// then the queue request is ignored as it is redundant.
		/// </summary>
		/// <param name="integrationQueueItem">The integration queue item.</param>
		void Enqueue(IIntegrationQueueItem integrationQueueItem);

		/// <summary>
		/// Releases the next integration request on the queue to start it's integration.
		/// </summary>
		void Dequeue();

		/// <summary>
		/// Removes a pending integration request (i.e. one that has not yet started) for this
		/// project from the queue if it is available.
		/// </summary>
		/// <param name="project">The project to have pending items removed from the queue.</param>
		void RemovePendingRequest(IProject project);

		/// <summary>
		/// Removes all queued integrations for this project. To be invoked when "stopping"
		/// a project.
		/// </summary>
		/// <param name="project">The project to be removed.</param>
		void RemoveProject(IProject project);

		/// <summary>
		/// Returns an array of the current queued integrations on the queue.
		/// </summary>
		/// <returns>Array of current queued integrations on the queue.</returns>
		IIntegrationQueueItem[] GetQueuedIntegrations();

		IntegrationRequest GetNextRequest(IProject project);
		
		bool HasItemOnQueue(IProject project);
		bool HasItemPendingOnQueue(IProject project);

        /// <summary>
        /// Toggle Locks. This instructs the queue that it should acquire (or release) locks upon the other queues which it is configured 
        /// to lock when integrating.
        /// </summary>
        /// <param name="acquire">Should the queue acquire locks or release them?</param>
        void ToggleQueueLocks(bool acquire);

        /// <summary>
        /// Lock this queue, based upon a request from another queue.
        /// Acquires a fresh lock for the queue making the request (assuming none exists).
        /// </summary>
        /// <param name="requestingQueue">Queue requesting that a lock be taken out</param>
        void LockQueue(IIntegrationQueue requestingQueue);

        /// <summary>
        /// Unlock this queue, based upon a request from another queue.
        /// Releases any locks currently held by the queue making the request.
        /// </summary>
        /// <param name="requestingQueue">Queue requesting that a lock be released</param>
        void UnlockQueue(IIntegrationQueue requestingQueue);
	}
}
