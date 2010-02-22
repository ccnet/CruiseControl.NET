using System.Collections;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Remote;
using System;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Interface for the project integrators to communicate with for adding their integration
	/// requests to a queue.
	/// </summary>
	public interface IIntegrationQueue : IList
	{
		string Name { get; }

        bool IsBlocked { get; }

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
        /// Try to block this queue, based upon a request from another queue.
        /// While blocked, no projects in this queue can integrate.
        /// </summary>
        /// <param name="requestingQueue">Queue requesting that a lock be taken out</param>
        /// <returns>True if the queue is now blocked, false if the queue could
        /// not be blocked due to being in-use.</returns>
        bool BlockQueue(IIntegrationQueue requestingQueue);

        /// <summary>
        /// Unblock this queue.
        /// </summary>
        /// <param name="requestingQueue">Queue requesting that a lock be released</param>
        void UnblockQueue(IIntegrationQueue requestingQueue);

        /// <summary>
        /// Attempt to acquire a lock on the queue to mark it as in-use.
        /// </summary>
        /// <param name="queueLock">If locking the queue for use was
        /// successful (returned true), lockObject is an IDisposable that
        /// will discard the lock when disposed.</param>
        /// <returns>True if the queue is now marked as in-use, false if the
        /// queue could not be marked as in-use due to being blocked (or
        /// one of its lockqueues was in-use).</returns>
        bool TryLock(out IDisposable queueLock);
	}
}
