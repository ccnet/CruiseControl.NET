using System;
using tw.ccnet.remote;

namespace tw.ccnet.core
{
	public interface IScheduler : IDisposable
	{
		ISchedule Schedule { get; set; }
		IProject Project { get; set; }
		void Start();
		void Stop();
		void WaitForExit();
		bool IsRunning { get; }
		SchedulerState State { get; }
	}
}
