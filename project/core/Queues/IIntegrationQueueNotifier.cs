
namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Interface for communication from the IIntegrationQueue to the IProjectIntegrator
	/// </summary>
	public interface IIntegrationQueueNotifier
	{
		/// <summary>
		/// This project integration request has reached the top of the queue and can be started.
		/// </summary>
		/// <param name="integrationQueueItem">The integration to be started.</param>
		void NotifyIntegrationToCommence(IIntegrationQueueItem integrationQueueItem);

		/// <summary>
		/// Notification of entering the integration queue.
		/// </summary>
		void NotifyEnteringIntegrationQueue();

		/// <summary>
		/// Notification of exiting the integration queue.
		/// </summary>
		void NotifyExitingIntegrationQueue(bool isPendingItemCancelled);
	}
}
