using NUnit.Framework;
using System;
using System.Diagnostics;
using System.Threading;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class ProjectIntegratorTest : CustomAssertion
	{
		private MockProject _project;
		private Schedule _schedule;
		private ProjectIntegrator _integrator;

		[SetUp]
		public void SetUp()
		{
			_project = new MockProject("mock", null);
		}

		[TearDown]
		public void TearDown()
		{
			if (_integrator != null)
			{
				_integrator.Stop();
				_integrator.WaitForExit();
			}
		}

		[Test]
		public void RunProjectOnce()
		{
			_integrator = CreateProjectIntegrator(1);
			Assert.AreEqual(0, _schedule.IterationsSoFar);
			_integrator.Start();
			// verify that the project has not run yet
			_integrator.WaitForExit();

			// verify that the project has run once after sleeping
			Assert.AreEqual(1, _schedule.IterationsSoFar);
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void RunProjectTwice()
		{
			// create a project integrator that runs for two integrations
			_integrator = CreateProjectIntegrator(3);
			_schedule.SleepSeconds = 1;

			DateTime startTime = DateTime.Now;

			// verfiy that the project has not run yet
			Assert.AreEqual(0, _schedule.IterationsSoFar);

			// start integration
			_integrator.Start();

			// block for 1 millisecond to let thread start
			Thread.Sleep(1);

			// let all integration cycles finish
			_integrator.WaitForExit();
			
			DateTime stopTime = DateTime.Now;

			// verify that the project has run three times after sleeping
			Assert.AreEqual(3, _schedule.IterationsSoFar);
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);

			// verify that project slept twice over 3 integrations
			TimeSpan delta = stopTime - startTime;
			TimeSpan expectedDelta = TimeSpan.FromSeconds(_schedule.SleepSeconds * 2);
			Assert.IsTrue(delta >= expectedDelta, "The project did not sleep: " + delta);

			//			TimeSpan expectedDelta = new TimeSpan(_schedule.TimeOut * 2);
			//			// Assert("The project did not sleep",  delta >= expectedDelta);
			//			//Console.WriteLine("expected: " + expectedDelta + " actual: " + delta);
		}

		[Test]
		public void RunProjectUntilStopped()
		{
			_integrator = CreateProjectIntegrator(Schedule.Infinite);
			_integrator.Start();
			Thread.Sleep(200);
			Assert.IsTrue(_integrator.IsRunning);
			_integrator.Stop();
			_integrator.WaitForExit();
			Assert.IsTrue(! _integrator.IsRunning);

			// verify that the project has run multiple times
			Assert.IsTrue(_project.RunIntegration_CallCount > 0);
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void StartMultipleTimes()
		{
			_integrator = CreateProjectIntegrator(Schedule.Infinite);
			_integrator.Start();
			_integrator.Start();
			Thread.Sleep(0);
			_integrator.Start();
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			_integrator.Stop();
			_integrator.WaitForExit();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void RestartScheduler()
		{
			_integrator = CreateProjectIntegrator(Schedule.Infinite);
			_integrator.Start();
			Thread.Sleep(0);
			_integrator.Stop();
			_integrator.WaitForExit();

			_integrator.Start();
			Thread.Sleep(0);
			_integrator.Stop();
			_integrator.WaitForExit();		
		}

		[Test]
		public void StopUnstartedScheduler()
		{
			IProjectIntegrator _scheduler = CreateProjectIntegrator(Schedule.Infinite);
			_scheduler.Stop();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _scheduler.State);
		}

		[Test]
		public void VerifySchedulerStateAfterException()
		{
			TestTraceListener listener = new TestTraceListener();
			Trace.Listeners.Add(listener);

			_project = new ExceptionMockProject("exception", new Schedule());
			_integrator = CreateProjectIntegrator();
			_integrator.Start();

			// block for thread to start
			Thread.Sleep(2500);

			// verify scheduler is still running - but is logging exceptions
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			Assert.IsTrue(_schedule.IterationsSoFar > 1);
			Assert.IsTrue(listener.Traces.Count > 0);
			Assert.IsTrue(listener.Traces[0].ToString().IndexOf(ExceptionMockProject.EXCEPTION_MESSAGE) > 0);
			
			// verify scheduler is restartable
			_integrator.Stop();
			_integrator.WaitForExit();

			//<<<<<<< SchedulerTest.cs
			//			_scheduler.Project = new MockProject("mock");
			//			Schedule newSchedule = new Schedule();
			//			newSchedule.TotalIterations = 1;
			//			newSchedule.TimeOut = 1;
			//=======
//			Schedule newSchedule = new Schedule(1, 1);
//			_integrator.Project = new MockProject("mock", newSchedule);
//			//>>>>>>> 1.4
//			_integrator.Schedule = newSchedule;
			
//			_integrator.Start();
//			Thread.Sleep(1);
//			_integrator.WaitForExit();
//			Assert.AreEqual(1, newSchedule.IterationsSoFar);
		}

		[Test]
		public void SleepTest()
		{
			DateTime start = DateTime.Now;
			Thread.Sleep(1000);
			DateTime end = DateTime.Now;
			AssertApproximatelyEqual("thread sleep time", 1000, (end - start).TotalMilliseconds, 150);
		}

		[Test]
		public void StartTwice()
		{
			_integrator = CreateProjectIntegrator();
			_integrator.Start();
			Thread.Sleep(50);
			_integrator.Start();
		}

		[Test]
		public void Abort()
		{
			_schedule = new Schedule(new DateTimeProvider(), 1, Schedule.Infinite);
			_integrator = new ProjectIntegrator(_schedule, _project);
			_integrator.Start();
			Thread.Sleep(0);
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			_integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void TerminateWhenProjectIsntStarted()
		{
			_schedule = new Schedule(new DateTimeProvider(),1, Schedule.Infinite);
			_integrator = new ProjectIntegrator(_schedule, _project);
			_integrator.Abort();
			Assert.AreEqual(ProjectIntegratorState.Stopped, _integrator.State);
		}

		[Test]
		public void TerminateCalledTwice()
		{
			_schedule = new Schedule(new DateTimeProvider(),1, Schedule.Infinite);
			_integrator = new ProjectIntegrator(_schedule, _project);
			_integrator.Start();
			Thread.Sleep(0);
			Assert.AreEqual(ProjectIntegratorState.Running, _integrator.State);
			_integrator.Abort();
			_integrator.Abort();
		}

		[Test]
		public void ForceBuild()
		{
			_integrator.ForceBuild();
		}

		private ProjectIntegrator CreateProjectIntegrator()
		{
			return CreateProjectIntegrator(Schedule.Infinite, 1);
		}

		private ProjectIntegrator CreateProjectIntegrator(int iterations)
		{
			return CreateProjectIntegrator(iterations, 1);
		}
	
		private ProjectIntegrator CreateProjectIntegrator(int iterations, int timeout)
		{
			_schedule = new Schedule(new DateTimeProvider(), timeout, iterations);
			return new ProjectIntegrator(_schedule, _project);
		}
	}
}
