using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class MonitorPolledEventArgs : EventArgs
	{
		public readonly IProjectMonitor ProjectMonitor;

		public MonitorPolledEventArgs( IProjectMonitor projectMonitor )
		{
			ProjectMonitor = projectMonitor;
		}
	}

	public delegate void MonitorPolledEventHandler( object sender, MonitorPolledEventArgs args );

}