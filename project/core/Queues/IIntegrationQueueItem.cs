using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Interface for the items placed in a named integration queue.
	/// </summary>
	public interface IIntegrationQueueItem
	{
		/// <summary>
		/// Gets the project to be added to the build queue.
		/// </summary>
		IProject Project { get; }

		/// <summary>
		/// Gets the integration request which was responsible for requesting the integration.
		/// </summary>
		/// <value></value>
		IntegrationRequest IntegrationRequest { get; }

		/// <summary>
		/// Gets the integration queue callback for the associated project.
		/// </summary>
		/// <value>The integration queue callback.</value>
		IIntegrationQueueNotifier IntegrationQueueNotifier { get; }
	}
}
