using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using tw.ccnet.core.test;
using tw.ccnet.core.util;

namespace tw.ccnet.core.schedule.test
{
	[TestFixture]
	public class SchedulerTest
	{
		private MockProject _project;
		private Schedule _schedule;
		private Scheduler _scheduler;

		[SetUp]
		protected void SetUp()
		{
			_project = new MockProject("mock");
		}

		[TearDown]
		protected void TearDown()
		{
			if (_scheduler != null)
			{
				_scheduler.Stop();
				_scheduler.WaitForExit();
			}
		}

		[Test]
		public void RunProjectOnce()
		{
			_scheduler = CreateScheduler(1);
			Assertion.AssertEquals(0, _schedule.Iterations);
			_scheduler.Start();
			// verify that the project has not run yet
			_scheduler.WaitForExit();

			// verify that the project has run once after sleeping
			Assertion.AssertEquals(1, _schedule.Iterations);
			Assertion.AssertEquals(SchedulerState.Stopped, _scheduler.State);
		}

		[Test]
		public void RunProjectTwice()
		{
			_scheduler = CreateScheduler(2);
			_schedule.TimeOut = 4;

			DateTime start = DateTime.Now;
			_scheduler.Start();
			// verfiy that the project has not run yet
			Assertion.AssertEquals(0, _schedule.Iterations);
			Thread.Sleep(1);	// block to let thread start
			_scheduler.WaitForExit();
			DateTime stop = DateTime.Now;

			// verify that the project has run twice after sleeping
			Assertion.AssertEquals(2, _schedule.Iterations);
			Assertion.AssertEquals(SchedulerState.Stopped, _scheduler.State);

			// verify that project slept twice
			TimeSpan delta = stop - start;
			TimeSpan expectedDelta = new TimeSpan(_schedule.TimeOut * 2);
			Assertion.Assert("The project did not sleep",  delta >= expectedDelta);
			//Console.WriteLine("expected: " + expectedDelta + " actual: " + delta);
		}

		[Test]
		public void RunProjectUntilStopped()
		{
			_scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Start();
			Thread.Sleep(20);
			Assertion.Assert("_scheduler is not still running?!", _scheduler.IsRunning);
			_scheduler.Stop();
			_scheduler.WaitForExit();
			Assertion.Assert("schedule should be stopped", ! _scheduler.IsRunning);

			// verify that the project has run multiple times
			Assertion.Assert(_project.Runs > 0);
			Assertion.AssertEquals(SchedulerState.Stopped, _scheduler.State);
		}

		[Test]
		public void StartMultipleTimes()
		{
			_scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Start();
			_scheduler.Start();
			Thread.Sleep(0);
			_scheduler.Start();
			Assertion.AssertEquals(SchedulerState.Running, _scheduler.State);
			_scheduler.Stop();
			_scheduler.WaitForExit();
			Assertion.AssertEquals(SchedulerState.Stopped, _scheduler.State);
		}

		[Test]
		public void RestartScheduler()
		{
			_scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Start();
			Thread.Sleep(0);
			_scheduler.Stop();
			_scheduler.WaitForExit();

			_scheduler.Start();
			Thread.Sleep(0);
			_scheduler.Stop();
			_scheduler.WaitForExit();		
		}

		[Test]
		public void StopUnstartedScheduler()
		{
			IScheduler _scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Stop();
			Assertion.AssertEquals(SchedulerState.Stopped, _scheduler.State);
		}

		[Test]
		public void VerifySchedulerStateAfterException()
		{
			TestTraceListener listener = new TestTraceListener();
			Trace.Listeners.Add(listener);
			_project = new ExceptionMockProject("exception");
			_scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Start();
			Thread.Sleep(10);		// block for thread to start

			// verify _scheduler is still running - but is logging exceptions
			Assertion.AssertEquals(SchedulerState.Running, _scheduler.State);
			Assertion.Assert(_schedule.Iterations > 1);
			Assertion.Assert(listener.Traces.Count > 1);
			Assertion.Assert(listener.Traces[0].ToString().IndexOf(ExceptionMockProject.EXCEPTION_MESSAGE) > 0);
			
			// verify _scheduler is restartable
			_scheduler.Stop();
			_scheduler.WaitForExit();

			_scheduler.Project = new MockProject("mock");
			Schedule newSchedule = new Schedule();
			newSchedule.TotalIterations = 1;
			_scheduler.Schedule = newSchedule;
			
			_scheduler.Start();
			_scheduler.WaitForExit();
			Assertion.AssertEquals(1, newSchedule.Iterations);
		}

		[Test]
		public void SleepTest()
		{
			DateTime start = DateTime.Now;
			Thread.Sleep(1000);
			DateTime end = DateTime.Now;
			Assertion.Assert("thread slept for less time than expected: " + (start - end) + " < 1000", end > start.AddMilliseconds(1000));
		}

		private Scheduler CreateScheduler(int iterations)
		{
			return CreateScheduler(iterations, 1);
		}
	
		private Scheduler CreateScheduler(int iterations, int timeout)
		{
			_schedule = new Schedule();
			_schedule.TimeOut = timeout;
			_schedule.TotalIterations = iterations;

			return new Scheduler(_schedule, _project);
		}
	}
}
