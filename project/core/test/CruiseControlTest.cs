using System;
using System.Collections;
using System.Threading;
using System.Xml;
using NUnit.Framework;
using NMock;
using tw.ccnet.core.util;
using tw.ccnet.core.schedule;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class CruiseControlTest
	{
		private DynamicMock _mockConfig;
		private MockProject _project1;
		private MockProject _project2;
		private IDictionary _projects;
		private CruiseControl _cc;

		[SetUp]
		protected void SetUp()
		{
			_project1 = new MockProject("project1", new Schedule(1, Schedule.Infinite));
			_project2 = new MockProject("project2", new Schedule(1, Schedule.Infinite));

			_projects = new Hashtable();
			_projects.Add("project1", _project1);
			_projects.Add("project2", _project2);

			_mockConfig = new DynamicMock(typeof(IConfigurationLoader));
		}

		[TearDown]
		protected void TearDown()
		{
			if (_cc != null) _cc.Stop();
			_cc = null;
		}

		[Test]
		public void LoadConfigurationAtConstruction()
		{
			MockProject projectWithoutSchedule = new MockProject("project3", null);
			_projects.Add(projectWithoutSchedule.Name, projectWithoutSchedule);
			_mockConfig.ExpectAndReturn("LoadProjects", _projects);

			_cc = new CruiseControl((IConfigurationLoader)_mockConfig.MockInstance);

			// verify that projects are loaded
			_mockConfig.Verify();
			Assertion.AssertEquals(_project1, _cc.GetProject("project1"));
			Assertion.AssertEquals(_project2, _cc.GetProject("project2"));

			// verify that schedulers have been created
			Assertion.AssertEquals(2, _cc.Schedulers.Count);
			Assertion.AssertEquals(_project1, ((IScheduler)_cc.Schedulers[1]).Project);
			Assertion.AssertEquals(_project2, ((IScheduler)_cc.Schedulers[0]).Project);
		}

		[Test]
		public void RunIntegration()
		{
			_mockConfig.ExpectAndReturn("LoadProjects", _projects);
			((Schedule)_project1.Schedule).TotalIterations = 1;
			((Schedule)_project2.Schedule).TotalIterations = 1;

			_cc = new CruiseControl((IConfigurationLoader)_mockConfig.MockInstance);
			_cc.Start(); // RunIntegration();
			_cc.WaitForExit();

			_mockConfig.Verify();
			Assertion.AssertEquals(1, _project1.Runs);
			Assertion.AssertEquals(1, _project2.Runs);
		}

		[Test]
		public void HandleChangedConfiguration()
		{
			MockConfigurationLoader config = new MockConfigurationLoader();
			config.Projects = _projects;

			_cc = new CruiseControl(config);
			_cc.Start();
			// verify configuration projects and schedulers have been loaded
			Assertion.AssertEquals(2, _cc.Schedulers.Count);
			Assertion.AssertEquals(_project1, ((IScheduler)_cc.Schedulers[1]).Project);
			Assertion.AssertEquals(_project2, ((IScheduler)_cc.Schedulers[0]).Project);

			// create new configuration - change schedule for project1, remove project2 and add project3
			Schedule newSchedule = new Schedule();
			_project1.Schedule = newSchedule;
			MockProject project3 = new MockProject("project3", new Schedule(1, 1));

			Hashtable projects = new Hashtable();
			projects.Add(_project1.Name, _project1);
			projects.Add(project3.Name, project3);
			config.Projects = projects;
			config.RaiseConfigurationChangedEvent();

			// verify configuration projects have been updated
			Assertion.AssertEquals(_project1, _cc.GetProject(_project1.Name));
			Assertion.AssertNull(_cc.GetProject(_project2.Name));
			Assertion.AssertEquals(project3, _cc.GetProject(project3.Name));

			// verify configuration schedulers have been updated
			Assertion.AssertEquals(2, _cc.Schedulers.Count);
			Assertion.AssertEquals(_project1, ((Scheduler)_cc.Schedulers[0]).Project);
			Assertion.AssertEquals(newSchedule, ((Scheduler)_cc.Schedulers[0]).Schedule);
			Assertion.AssertEquals(SchedulerState.Running, ((Scheduler)_cc.Schedulers[0]).State);
			Assertion.AssertEquals(project3, ((Scheduler)_cc.Schedulers[1]).Project);
			// project3 is automatically started
			Assertion.AssertEquals(SchedulerState.Running, ((Scheduler)_cc.Schedulers[1]).State);

			_cc.Stop();
		}

		[Test, Ignore("Implement this test")]
		public void HandleChangedConfigurationWhileSchedulersAreRunning()
		{
		}

		[Test]
		public void StartStopAndWaitForExit()
		{
			_mockConfig.ExpectAndReturn("LoadProjects", _projects);

			_cc = new CruiseControl((IConfigurationLoader)_mockConfig.MockInstance);
			_cc.Start();
			foreach (IScheduler scheduler in _cc.Schedulers)
			{
				Assertion.AssertEquals(SchedulerState.Running, scheduler.State);
			}
			_cc.Stop();
			_cc.WaitForExit();

			foreach (IScheduler scheduler in _cc.Schedulers)
			{
				Assertion.AssertEquals(SchedulerState.Stopped, scheduler.State);
			}			
		}

		private class MockConfigurationLoader : IConfigurationLoader
		{
			private IDictionary _projects;
			private ConfigurationChangedHandler _handler;

			public IDictionary Projects
			{
				set { _projects = value; }
			}

			public IDictionary LoadProjects()
			{
				return _projects;
			}

			public void AddConfigurationChangedHandler(ConfigurationChangedHandler handler)
			{
				_handler += handler;
			}

			public void RaiseConfigurationChangedEvent()
			{
				_handler();
			}
		}

		[Test]
		public void StartAndStopTwice()
		{
			_mockConfig.ExpectAndReturn("LoadProjects", _projects);
			_cc = new CruiseControl((IConfigurationLoader)_mockConfig.MockInstance);

			_cc.Start();
			Thread.Sleep(1);
			Assertion.AssertEquals(2, _cc.Schedulers.Count);
			Assertion.AssertEquals(SchedulerState.Running, ((IScheduler)_cc.Schedulers[0]).State);
			Assertion.AssertEquals(SchedulerState.Running, ((IScheduler)_cc.Schedulers[1]).State);

			// try invoking start again
			_cc.Start();
			Thread.Sleep(1);
			Assertion.AssertEquals(2, _cc.Schedulers.Count);
			Assertion.AssertEquals(SchedulerState.Running, ((IScheduler)_cc.Schedulers[0]).State);
			Assertion.AssertEquals(SchedulerState.Running, ((IScheduler)_cc.Schedulers[1]).State);

			_cc.Stop();
			_cc.WaitForExit();
			Thread.Sleep(1);
			Assertion.AssertEquals(SchedulerState.Stopped, ((IScheduler)_cc.Schedulers[0]).State);
			Assertion.AssertEquals(SchedulerState.Stopped, ((IScheduler)_cc.Schedulers[1]).State);

			// try invoking stop again
			_cc.Stop();
			Thread.Sleep(1);
			Assertion.AssertEquals(SchedulerState.Stopped, ((IScheduler)_cc.Schedulers[0]).State);
			Assertion.AssertEquals(SchedulerState.Stopped, ((IScheduler)_cc.Schedulers[1]).State);
		}

//		public void TestStopSleepingProject()
//		{
//			_project1.Sleep = 100000;
//			_cc.Start(_project1);
//			Assertion.AssertEquals(1, _cc.Threads.Count);
//			Thread.Sleep(1000);
//
//			_cc.Stop(_project1);
//			Assertion.AssertEquals(0, _cc.Threads.Count);
//		}
//
//		// test stop if thread has already stopped
//		public void TestStopAlreadyStoppedProject()
//		{
//			_cc.Start(_project1);
//			Thread.Sleep(1000);
//			_project1.Stop();
//			Thread.Sleep(1000);
//			_cc.Stop(_project1);
//			Assertion.AssertEquals(0, _cc.Threads.Count);
//		}
//
//		public void TestDisposeIfThreadsAreNotStopped()
//		{
//			_mockConfig.ExpectAndReturn("HasConfigurationChanged", true);
//			_mockConfig.ExpectAndReturn("LoadProjects", _projects);
//
//			
//			_cc.Start();
//			Thread.Sleep(2);
//			// do not shut down threads -- need to verify dispose called
//			_cc.Dispose();
//		}
	}
}
