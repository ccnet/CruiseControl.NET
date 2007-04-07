using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// This is like ICruiseManager, but relates to an individual server.
	/// </summary>
	public interface ICruiseServerManager
	{
		string ServerUrl { get; }
		string DisplayName { get; }
		BuildServerTransport Transport { get; }

		/// <summary>
		/// Cancel the pending request on the integration queue for the specified project on this server.
		/// </summary>
		/// <param name="projectName">Name of the project to cancel.</param>
		void CancelPendingRequest(string projectName);
		/// <summary>
		/// Gets the integration queue snapshot from this server.
		/// </summary>
		/// <value>The integration queue snapshot.</value>
		IntegrationQueueSnapshot GetIntegrationQueueSnapshot();
	}
}
