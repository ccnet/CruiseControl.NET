using System;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using tw.ccnet.core.test;

namespace tw.ccnet.core.schedule.test
{
	[TestFixture]
	public class SchedulerTest
	{
		private MockProject _project;
		private Schedule _schedule;

		[SetUp]
		protected void SetUp()
		{
			_project = new MockProject("mock");
		}

		[Test]
		public void RunProjectOnce()
		{
			Scheduler scheduler = CreateScheduler(1);
			Assertion.AssertEquals(0, _schedule.Iterations);
			scheduler.Start();
			// verify that the project has not run yet
			scheduler.WaitForExit();

			// verify that the project has run once after sleeping
			Assertion.AssertEquals(1, _schedule.Iterations);
			Assertion.AssertEquals(SchedulerState.Stopped, scheduler.State);
		}

		[Test]
		public void RunProjectTwice()
		{
			Scheduler scheduler = CreateScheduler(2);
			_schedule.TimeOut = 4;

			DateTime start = DateTime.Now;
			scheduler.Start();
			// verfiy that the project has not run yet
			Assertion.AssertEquals(0, _schedule.Iterations);
			scheduler.WaitForExit();
			DateTime stop = DateTime.Now;

			// verify that the project has run twice after sleeping
			Assertion.AssertEquals(2, _schedule.Iterations);
			Assertion.AssertEquals(SchedulerState.Stopped, scheduler.State);

			// verify that project slept twice
			// TODO: determine why this fails sometimes.  Is the thread really sleeping?
			// TimeSpan delta = stop - start;
			// TimeSpan expectedDelta = new TimeSpan(_schedule.TimeOut * 2);
			// Console.WriteLine("expected: " + expectedDelta + " actual: " + delta);
			// Assertion.Assert("The project did not sleep",  delta >= expectedDelta);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void StartWhileStartedThrowsException()
		{
			Scheduler scheduler = CreateScheduler(3);
			scheduler.Start();
			scheduler.Start();			
		}

		[Test]
		public void RunProjectUntilStopped()
		{
			Scheduler scheduler = CreateScheduler(Schedule.Infinite);
			scheduler.Start();
			Thread.Sleep(20);
			Assertion.Assert("scheduler is not still running?!", scheduler.IsRunning);
			scheduler.Stop();
			scheduler.WaitForExit();
			Assertion.Assert("schedule should be stopped", ! scheduler.IsRunning);

			// verify that the project has run multiple times
			Assertion.Assert(_project.Runs > 0);
			Assertion.AssertEquals(SchedulerState.Stopped, scheduler.State);
		}

		[Test]
		public void StopUnstartedScheduler()
		{
			IScheduler scheduler = CreateScheduler(Schedule.Infinite);
			scheduler.Stop();
			Assertion.AssertEquals(SchedulerState.Stopped, scheduler.State);
		}

		[Test]
		public void VerifySchedulerStateAfterException()
		{
			_project = new ExceptionMockProject("exception");
			Scheduler scheduler = CreateScheduler(Schedule.Infinite);
			try 
			{ 
				scheduler.Start();
				Assertion.Fail("expected exception was not thrown");
			}
			catch (Exception) {	}
			scheduler.WaitForExit();

			// verify scheduler is reset and can be restarted
			Assertion.AssertEquals(SchedulerState.Stopped, scheduler.State);
			Assertion.AssertEquals(0, _schedule.Iterations);

			scheduler.Project = new MockProject("mock");
			_schedule.TotalIterations = 1;
			scheduler.Start();
			scheduler.WaitForExit();
			Assertion.AssertEquals(1, _schedule.Iterations);
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
