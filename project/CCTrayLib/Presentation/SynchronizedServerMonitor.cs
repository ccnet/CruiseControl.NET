using System;
using System.ComponentModel;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// This is a decorator for an IServerMonitor that ensures that events are fired
	/// via an ISynchronizeInvoke interface.  The only reason to do this normally is to 
	/// ensure that the events get processed on a WinForms thread.
	/// </summary>
	public class SynchronizedServerMonitor : ISingleServerMonitor
	{
		public event MonitorServerQueueChangedEventHandler QueueChanged;
		public event MonitorServerPolledEventHandler Polled;

		private readonly ISingleServerMonitor serverMonitor;
		private readonly ISynchronizeInvoke synchronizeInvoke;

		public SynchronizedServerMonitor(ISingleServerMonitor serverMonitor, ISynchronizeInvoke synchronizeInvoke)
		{
			this.serverMonitor = serverMonitor;
			this.synchronizeInvoke = synchronizeInvoke;

			serverMonitor.Polled += new MonitorServerPolledEventHandler(ServerMonitor_Polled);
			serverMonitor.QueueChanged += new MonitorServerQueueChangedEventHandler(ServerMonitor_QueueChanged);
		}

		public string ServerUrl
		{
			get { return serverMonitor.ServerUrl; }
		}

		public string DisplayName
		{
			get { return serverMonitor.DisplayName; }
		}

		public BuildServerTransport Transport
		{
			get { return serverMonitor.Transport; }
		}

		public void CancelPendingRequest(string projectName)
		{
			serverMonitor.CancelPendingRequest(projectName);
		}

		public IntegrationQueueSnapshot IntegrationQueueSnapshot
		{
			get { return serverMonitor.IntegrationQueueSnapshot; }
		}

		public bool IsConnected
		{
			get { return serverMonitor.IsConnected; }
		}

		public Exception ConnectException
		{
			get { return serverMonitor.ConnectException; }
		}

		public void Poll()
		{
			serverMonitor.Poll();
		}

		public void OnPollStarting()
		{
			serverMonitor.OnPollStarting();
		}

		private void ServerMonitor_Polled(object sender, MonitorServerPolledEventArgs args)
		{
			if (Polled != null) synchronizeInvoke.BeginInvoke(Polled, new object[] {sender, args});
		}

		private void ServerMonitor_QueueChanged(object sender, MonitorServerQueueChangedEventArgs args)
		{
			if (QueueChanged != null) synchronizeInvoke.BeginInvoke(QueueChanged, new object[] {sender, args});
		}
	}
}
