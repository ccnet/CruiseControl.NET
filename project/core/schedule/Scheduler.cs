using System;
using System.Threading;

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
				throw new CruiseControlException("Schedule is already started.  Stop before starting again.");
			}
			_state = SchedulerState.Running;
			_thread = new Thread(new ThreadStart(Run));
			_thread.Start();
		}

		private void Run()
		{
			try
			{
				while (IsRunning && _schedule.ShouldRun())
				{
					_project.RunIntegration();
					_schedule.Update();
					Thread.Sleep(_schedule.CalculateTimeToNextIntegration());
					// Console.WriteLine("sleep " + _schedule.CalculateTimeToNextIntegration().Ticks);
				}
			}
			catch (Exception ex) 
			{ 
				_thread.Abort();
				Console.WriteLine("Exception: " + ex);
			}
			finally
			{
				_state = SchedulerState.Stopped;
			}
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
