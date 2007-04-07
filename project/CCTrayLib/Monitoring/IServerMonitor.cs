
namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Represents the current status of a build server, updated only when 
	/// requested by calling Poll.
	/// 
	/// Tracks integration queue activity and fires events when significant changes occur.
	/// </summary>
	public interface IServerMonitor : IPollable
	{
		event MonitorServerPolledEventHandler Polled;
		event MonitorServerQueueChangedEventHandler QueueChanged;
	}
}
