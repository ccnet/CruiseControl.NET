using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Track the state of a single CruiseControl server.
	/// </summary>
	public class ServerMonitor : ISingleServerMonitor
	{
		public event MonitorServerPolledEventHandler Polled;
		public event MonitorServerQueueChangedEventHandler QueueChanged;

		private IntegrationQueueSnapshot lastIntegrationQueueSnapshot;
		private ICruiseServerManager cruiseServerManager;
		private Exception connectException;

		public ServerMonitor(ICruiseServerManager cruiseServerManager)
		{
			this.cruiseServerManager = cruiseServerManager;
		}

		/// <summary>
		/// Cancel the pending request on the integration queue for the specified project on this server.
		/// </summary>
		/// <param name="projectName">Name of the project to cancel.</param>
		public void CancelPendingRequest(string projectName)
		{
			this.cruiseServerManager.CancelPendingRequest(projectName);
		}

		/// <summary>
		/// Gets the integration queue snapshot for this server.
		/// </summary>
		/// <value>The integration queue snapshot.</value>
		public IntegrationQueueSnapshot IntegrationQueueSnapshot
		{
			get { return lastIntegrationQueueSnapshot; }
		}

		/// <summary>
		/// Polls this server for the latest integration queue snapshot.
		/// </summary>
		public void Poll()
		{
			try
			{
				IntegrationQueueSnapshot newIntegrationQueueSnapshot = cruiseServerManager.GetIntegrationQueueSnapshot();
				if (newIntegrationQueueSnapshot != null)
				{
					if ((lastIntegrationQueueSnapshot == null) 
						|| (lastIntegrationQueueSnapshot.TimeStamp != newIntegrationQueueSnapshot.TimeStamp))
					{
						OnQueueChanged(new MonitorServerQueueChangedEventArgs(this));
					}
				}
				lastIntegrationQueueSnapshot = newIntegrationQueueSnapshot;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("ServerMonitorPoll Exception: " + ex.ToString());
				lastIntegrationQueueSnapshot = null;
				connectException = ex;
			}

			OnPolled(new MonitorServerPolledEventArgs(this));
		}

		public void OnPollStarting()
		{
			lastIntegrationQueueSnapshot = null; // Force an OnQueueChanged event to fire when poll restarted
		}

		public string ServerUrl
		{
			get { return cruiseServerManager.ServerUrl; }
		}

		public string DisplayName
		{
			get { return cruiseServerManager.DisplayName; }
		}

		public BuildServerTransport Transport
		{
			get { return cruiseServerManager.Transport; }
		}

		public bool IsConnected
		{
			get { return lastIntegrationQueueSnapshot != null; }
		}

		public Exception ConnectException
		{
			get { return connectException; }
		}

		protected void OnPolled(MonitorServerPolledEventArgs args)
		{
			if (Polled != null) Polled(this, args);
		}

		protected void OnQueueChanged(MonitorServerQueueChangedEventArgs args)
		{
			if (QueueChanged != null) QueueChanged(this, args);
		}
	}
}
