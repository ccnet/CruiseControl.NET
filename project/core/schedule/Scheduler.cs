using System;
using System.Threading;
using tw.ccnet.remote;
using tw.ccnet.core.util;

namespace tw.ccnet.core.schedule
{
	public class Scheduler : IScheduler
	{
		private ISchedule _schedule;
		private IProject _project;
		private Thread _thread;
		private SchedulerState _state = SchedulerState.Stopped;

		public Scheduler(ISchedule schedule, IProject project)
		{
			_schedule = schedule;
			_project = project;
		}

		public IProject Project 
		{ 
			get { return _project; }
			set { _project = value; } 
		}

		public ISchedule Schedule
		{
			get { return _schedule; }
			set { _schedule = value; }
		}

		public SchedulerState State
		{
			get { return _state; }
		}

		public void Start()
		{
			if (IsRunning)
			{
				LogUtil.Log("scheduler", "Thread cannot be started.  It is already running.");
				return;
			}
			// TODO: need verification that multiple thread instances cannot be created.
			_thread =  new Thread(new ThreadStart(Run));
			_state = SchedulerState.Running;
			_thread.Start();
		}

		private void Run()
		{
			while (IsRunning && _schedule.ShouldRun())
			{
				try
				{
					_project.Run(_schedule.ForceBuild);
				}
				catch (Exception ex) 
				{ 
					LogUtil.Log(_project, "Project threw an exception: " + ex);
				}
				_schedule.Update();
				Thread.Sleep(_schedule.CalculateTimeToNextIntegration());
			}
			_state = SchedulerState.Stopped;
		}

		public bool IsRunning
		{
			get { return _state == SchedulerState.Running; }
		}

		public void Stop()
		{
			if (IsRunning)
			{
				_state = SchedulerState.Stopping;
//				if (_thread != null && _thread.ThreadState == ThreadState.WaitSleepJoin)
//				{
//					_thread.Resume();
//				}
			}
		}

		public void WaitForExit()
		{
			if (_thread != null && _thread.IsAlive)
			{
				_thread.Join();
			}
		}

		public void Dispose()
		{
			Stop();
		}
	}
}
