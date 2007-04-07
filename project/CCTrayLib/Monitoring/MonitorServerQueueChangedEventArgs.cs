using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class MonitorServerQueueChangedEventArgs : EventArgs
	{
		public readonly ISingleServerMonitor ServerMonitor;

		public MonitorServerQueueChangedEventArgs(ISingleServerMonitor serverMonitor)
		{
			ServerMonitor = serverMonitor;
		}
	}

	public delegate void MonitorServerQueueChangedEventHandler(object sender, MonitorServerQueueChangedEventArgs e);
}
