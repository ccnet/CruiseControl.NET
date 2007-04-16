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

        private CruiseServerSnapshot lastCruiseServerSnapshot;
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
        /// Gets the cruise server snapshot of project and queue status for the monitored server (single).
        /// </summary>
        public CruiseServerSnapshot CruiseServerSnapshot
		{
            get { return lastCruiseServerSnapshot; }
		}

        /// <summary>
        /// Lookup the last project status retrieved for this project.
        /// </summary>
        public ProjectStatus GetProjectStatus(string projectName)
        {
            if (lastCruiseServerSnapshot == null || lastCruiseServerSnapshot.ProjectStatuses == null)
            {
                return null;
            }
            foreach (ProjectStatus status in lastCruiseServerSnapshot.ProjectStatuses)
            {
                if (status.Name == projectName)
                    return status;
            }
            throw new ApplicationException("Project '" + projectName + "' not found on server");
        }

	    /// <summary>
		/// Polls this server for the latest cruise control server project statuses and queues.
		/// </summary>
		public void Poll()
		{
			try
			{
			    CruiseServerSnapshot cruiseServerSnapshot = cruiseServerManager.GetCruiseServerSnapshot();
                if ((lastCruiseServerSnapshot == null) 
                    || (cruiseServerSnapshot == null)
                    || lastCruiseServerSnapshot.IsQueueSetSnapshotChanged(cruiseServerSnapshot.QueueSetSnapshot))
                {
                    OnQueueChanged(new MonitorServerQueueChangedEventArgs(this));
                }
                lastCruiseServerSnapshot = cruiseServerSnapshot;
			}
			catch (Exception ex)
			{
				Trace.WriteLine("ServerMonitorPoll Exception: " + ex);
                lastCruiseServerSnapshot = null;
				connectException = ex;
                OnQueueChanged(new MonitorServerQueueChangedEventArgs(this));
            }

			OnPolled(new MonitorServerPolledEventArgs(this));
		}

		public void OnPollStarting()
		{
            lastCruiseServerSnapshot = null; // Force an OnQueueChanged event to fire when poll restarted
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
			get { return lastCruiseServerSnapshot != null; }
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
