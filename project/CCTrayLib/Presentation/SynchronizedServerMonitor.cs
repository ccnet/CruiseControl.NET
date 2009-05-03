using System;
using System.ComponentModel;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using System.Windows.Forms;

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

        public string SessionToken
        {
            get { return serverMonitor.SessionToken; }
        }

		public BuildServerTransport Transport
		{
			get { return serverMonitor.Transport; }
		}

		public void CancelPendingRequest(string projectName)
		{
			serverMonitor.CancelPendingRequest(projectName);
		}

        public CruiseServerSnapshot CruiseServerSnapshot
		{
            get { return serverMonitor.CruiseServerSnapshot; }
		}

        public ProjectStatus GetProjectStatus(string projectName)
        {
            return serverMonitor.GetProjectStatus(projectName);
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
            if (Polled != null)
            {
                var canInvoke = true;
                if (synchronizeInvoke is Control) canInvoke = !(synchronizeInvoke as Control).IsDisposed;

                if (canInvoke) synchronizeInvoke.BeginInvoke(Polled, new object[] { sender, args });
            }
		}

		private void ServerMonitor_QueueChanged(object sender, MonitorServerQueueChangedEventArgs args)
		{
            if (QueueChanged != null)
            {
                var canInvoke = true;
                if (synchronizeInvoke is Control) canInvoke = !(synchronizeInvoke as Control).IsDisposed;

                if (canInvoke) synchronizeInvoke.BeginInvoke(QueueChanged, new object[] { sender, args });
            }
		}

        public void Start()
        {
            serverMonitor.Start();
        }

        public void Stop()
        {
            serverMonitor.Stop();
        }

        public bool RefreshSession()
        {
            return serverMonitor.RefreshSession();
        }
	}
}
