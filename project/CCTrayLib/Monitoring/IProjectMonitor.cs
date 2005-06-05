using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Represents the current status of a project, updated only when 
	/// requested by calling Poll.
	/// 
	/// Tracks build transitions and fires events when significant changes occur.
	/// </summary>
	public interface IProjectMonitor : IPollable
	{
		string ProjectName { get; }
		ProjectStatus ProjectStatus { get; }
		ProjectState ProjectState { get; }
		Exception ConnectException { get; }

		event MonitorBuildOccurredEventHandler BuildOccurred;
		event MonitorPolledEventHandler Polled;
		void ForceBuild();
	}

}