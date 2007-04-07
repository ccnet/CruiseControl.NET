using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ISingleServerMonitor : IServerMonitor
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
		/// Gets the integration queue snapshot for the monitored server (single).
		/// </summary>
		/// <value>The integration queue snapshot.</value>
		IntegrationQueueSnapshot IntegrationQueueSnapshot { get; }

		bool IsConnected { get; }
		
		Exception ConnectException { get; }
	}
}
