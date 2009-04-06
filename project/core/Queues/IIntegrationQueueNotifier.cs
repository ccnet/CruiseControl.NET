
namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Interface for communication from the IIntegrationQueue to the IProjectIntegrator
	/// </summary>
	public interface IIntegrationQueueNotifier
	{
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
