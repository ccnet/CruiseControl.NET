using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
	/// <summary>
	/// Container class
	/// </summary>
	public class IntegrationQueueItem : IIntegrationQueueItem
	{
		private IProject project;
		private IntegrationRequest integrationRequest;
		private IIntegrationQueueNotifier integrationQueueNotifier;

		/// <summary>
		/// Initializes a new instance of the <see cref="IntegrationQueueItem"/> class.
		/// </summary>
		public IntegrationQueueItem(IProject project, IntegrationRequest integrationRequest, IIntegrationQueueNotifier integrationQueueNotifier)
		{
			this.project = project;
			this.integrationRequest = integrationRequest;
			this.integrationQueueNotifier = integrationQueueNotifier;
		}

		/// <summary>
		/// Gets the project to be added to the build queue.
		/// </summary>
		/// <value></value>
		public IProject Project
		{
			get { return project; }
		}

		/// <summary>
		/// Gets the integration request which was responsible for requesting the integration.
		/// </summary>
		/// <value></value>
		public IntegrationRequest IntegrationRequest
		{
			get { return integrationRequest; }
		}

		/// <summary>
		/// Gets the integration queue callback for the associated project.
		/// </summary>
		/// <value>The integration queue callback.</value>
		public IIntegrationQueueNotifier IntegrationQueueNotifier
		{
			get { return integrationQueueNotifier; }
		}
	}
}
