using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class MonitorBuildOccurredEventArgs : EventArgs
	{
		public readonly IProjectMonitor ProjectMonitor;
		public readonly BuildTransition BuildTransition;

		public MonitorBuildOccurredEventArgs( IProjectMonitor projectMonitor, BuildTransition buildTransition )
		{
			ProjectMonitor = projectMonitor;
			BuildTransition = buildTransition;
		}
	}

	public delegate void MonitorBuildOccurredEventHandler( object sender, MonitorBuildOccurredEventArgs e );

}