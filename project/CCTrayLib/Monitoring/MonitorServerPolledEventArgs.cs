using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class MonitorServerPolledEventArgs : EventArgs
	{
		public readonly IServerMonitor ServerMonitor;

		public MonitorServerPolledEventArgs( IServerMonitor serverMonitor )
		{
			ServerMonitor = serverMonitor;
		}
	}

	public delegate void MonitorServerPolledEventHandler( object sender, MonitorServerPolledEventArgs args );

}
