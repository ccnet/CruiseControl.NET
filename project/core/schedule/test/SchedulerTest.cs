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
	public class SchedulerTest : CustomAssertion
	{
		private MockProject _project;
		private Schedule _schedule;
		private Scheduler _scheduler;

		[SetUp]
		protected void SetUp()
		{
			_project = new MockProject("mock", null);
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
			AssertEquals(0, _schedule.Iterations);
			_scheduler.Start();
			// verify that the project has not run yet
			_scheduler.WaitForExit();

			// verify that the project has run once after sleeping
			AssertEquals(1, _schedule.Iterations);
			AssertEquals(SchedulerState.Stopped, _scheduler.State);
		}

		[Test]
		public void RunProjectTwice()
		{
			_scheduler = CreateScheduler(2);
			_schedule.TimeOut = 20;

			DateTime start = DateTime.Now;
			_scheduler.Start();
			// verfiy that the project has not run yet
			AssertEquals(0, _schedule.Iterations);
			Thread.Sleep(1);	// block to let thread start
			_scheduler.WaitForExit();
			DateTime stop = DateTime.Now;

			// verify that the project has run twice after sleeping
			AssertEquals(2, _schedule.Iterations);
			AssertEquals(SchedulerState.Stopped, _scheduler.State);

			// verify that project slept twice
			TimeSpan delta = stop - start;
			TimeSpan expectedDelta = new TimeSpan(0, 0, 0, 0, (int) _schedule.TimeOut * 2);
			Assertion.Assert("The project did not sleep",  delta >= expectedDelta);

			//			TimeSpan expectedDelta = new TimeSpan(_schedule.TimeOut * 2);
			//			// Assert("The project did not sleep",  delta >= expectedDelta);
			//			//Console.WriteLine("expected: " + expectedDelta + " actual: " + delta);
		}

		[Test]
		public void RunProjectUntilStopped()
		{
			_scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Start();
			Thread.Sleep(20);
			Assert("_scheduler is not still running?!", _scheduler.IsRunning);
			_scheduler.Stop();
			_scheduler.WaitForExit();
			Assert("schedule should be stopped", ! _scheduler.IsRunning);

			// verify that the project has run multiple times
			Assert(_project.RunIntegration_CallCount > 0);
			AssertEquals(SchedulerState.Stopped, _scheduler.State);
		}

		[Test]
		public void StartMultipleTimes()
		{
			_scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Start();
			_scheduler.Start();
			Thread.Sleep(0);
			_scheduler.Start();
			AssertEquals(SchedulerState.Running, _scheduler.State);
			_scheduler.Stop();
			_scheduler.WaitForExit();
			AssertEquals(SchedulerState.Stopped, _scheduler.State);
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
			AssertEquals(SchedulerState.Stopped, _scheduler.State);
		}

		[Test]
		public void VerifySchedulerStateAfterException()
		{
			TestTraceListener listener = new TestTraceListener();
			Trace.Listeners.Add(listener);
			_project = new ExceptionMockProject("exception", new Schedule());
			_scheduler = CreateScheduler(Schedule.Infinite);
			_scheduler.Start();
			Thread.Sleep(1000);		// block for thread to start

			// verify scheduler is still running - but is logging exceptions
			AssertEquals(SchedulerState.Running, _scheduler.State);
			Assert(_schedule.Iterations > 1);
			Assert(listener.Traces.Count > 1);
			Assert(listener.Traces[0].ToString().IndexOf(ExceptionMockProject.EXCEPTION_MESSAGE) > 0);
			
			// verify scheduler is restartable
			_scheduler.Stop();
			_scheduler.WaitForExit();

			//<<<<<<< SchedulerTest.cs
			//			_scheduler.Project = new MockProject("mock");
			//			Schedule newSchedule = new Schedule();
			//			newSchedule.TotalIterations = 1;
			//			newSchedule.TimeOut = 1;
			//=======
			Schedule newSchedule = new Schedule(1, 1);
			_scheduler.Project = new MockProject("mock", newSchedule);
			//>>>>>>> 1.4
			_scheduler.Schedule = newSchedule;
			
			_scheduler.Start();
			Thread.Sleep(1);
			_scheduler.WaitForExit();
			AssertEquals(1, newSchedule.Iterations);
		}

		[Test][Ignore("doesn't run dependably")]
		public void SleepTest()
		{
			DateTime start = DateTime.Now;
			Thread.Sleep(1000);
			DateTime end = DateTime.Now;
			Assert("thread slept for less time than expected: " + (start - end) + " < 1000", end > start.AddMilliseconds(1000));
		}

		[Test]
		public void ForceBuild()
		{
			_project = new MockProject("test", new Schedule());
			Schedule schedule = new Schedule();
			schedule.TotalIterations = 1;
			schedule.TimeOut = 1;
			schedule.ForceBuild = true;
			_scheduler = new Scheduler(schedule, _project);

			_scheduler.Start();
			_scheduler.WaitForExit();

			Assert(_project.RunIntegration_forceBuild);
		}

		private Scheduler CreateScheduler(int iterations)
		{
			return CreateScheduler(iterations, 1);
		}
	
		private Scheduler CreateScheduler(int iterations, int timeout)
		{
			_schedule = new Schedule(timeout, iterations);
			return new Scheduler(_schedule, _project);
		}
	}
}
