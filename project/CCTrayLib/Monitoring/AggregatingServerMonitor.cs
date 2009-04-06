
namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// This aggregator is responsible for iterating across the list of known build servers
	/// and firing their poll events.
	/// </summary>
	public class AggregatingServerMonitor : IServerMonitor
	{
		public event MonitorServerPolledEventHandler Polled;
		public event MonitorServerQueueChangedEventHandler QueueChanged;

		private readonly IServerMonitor[] monitors;

		public AggregatingServerMonitor(params IServerMonitor[] monitors)
		{
			this.monitors = monitors;

			foreach (IServerMonitor monitor in this.monitors)
			{
				monitor.Polled += new MonitorServerPolledEventHandler(Monitor_Polled);
				monitor.QueueChanged += new MonitorServerQueueChangedEventHandler(Monitor_QueueChanged);
			}
		}

		/// <summary>
		/// Polls all the known build servers used by the current project list.
		/// </summary>
		public void Poll()
		{
			foreach (IServerMonitor monitor in monitors)
			{
				monitor.Poll();
			}
		}

		/// <summary>
		/// Prepares all the server monitors for a poll.
		/// </summary>
		public void OnPollStarting()
		{
			foreach (IServerMonitor monitor in monitors)
			{
				monitor.OnPollStarting();
			}
		}

		private void Monitor_Polled(object sender, MonitorServerPolledEventArgs args)
		{
			if (Polled != null) Polled(this, args);
		}

		private void Monitor_QueueChanged(object sender, MonitorServerQueueChangedEventArgs args)
		{
			if (QueueChanged != null) QueueChanged(this, args);
		}
	}
}
