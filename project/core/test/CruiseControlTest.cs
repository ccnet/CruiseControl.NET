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

		[SetUp]
		protected void SetUp()
		{
			_project1 = new MockProject("project1");
			_project2 = new MockProject("project2");

			_projects = new Hashtable();
			_projects.Add("project1", _project1);
			_projects.Add("project2", _project2);

			_mockConfig = new DynamicMock(typeof(IConfigurationLoader));
		}

		[Test]
		public void LoadConfigurationAtConstruction()
		{
			MockProject projectWithoutSchedule = new MockProject("project3");
			projectWithoutSchedule.Schedule = null;
			_projects.Add(projectWithoutSchedule.Name, projectWithoutSchedule);
			_mockConfig.ExpectAndReturn("LoadProjects", _projects);

			CruiseControl cc = new CruiseControl((IConfigurationLoader)_mockConfig.MockInstance);

			// verify that projects are loaded
			_mockConfig.Verify();
			Assertion.AssertEquals(_project1, cc.GetProject("project1"));
			Assertion.AssertEquals(_project2, cc.GetProject("project2"));

			// verify that schedulers have been created
			Assertion.AssertEquals(2, cc.Schedulers.Count);
			Assertion.AssertEquals(_project1, ((IScheduler)cc.Schedulers[1]).Project);
			Assertion.AssertEquals(_project2, ((IScheduler)cc.Schedulers[0]).Project);
		}

		[Test]
		public void RunIntegration()
		{
			_mockConfig.ExpectAndReturn("LoadProjects", _projects);

			CruiseControl cc = new CruiseControl((IConfigurationLoader)_mockConfig.MockInstance);
			cc.RunIntegration();

			_mockConfig.Verify();
			Assertion.AssertEquals(1, _project1.Runs);
			Assertion.AssertEquals(1, _project2.Runs);
		}

		[Test]
		public void HandleChangedConfiguration()
		{
			MockConfigurationLoader config = new MockConfigurationLoader();
			config.Projects = _projects;

			CruiseControl cc = new CruiseControl(config);
			// verify configuration projects and schedulers have been loaded
			Assertion.AssertEquals(2, cc.Schedulers.Count);
			Assertion.AssertEquals(_project1, ((IScheduler)cc.Schedulers[1]).Project);
			Assertion.AssertEquals(_project2, ((IScheduler)cc.Schedulers[0]).Project);

			// create new configuration - change schedule for project1, remove project2 and add project3
			Schedule newSchedule = new Schedule();
			_project1.Schedule = newSchedule;
			MockProject project3 = new MockProject("project3");

			Hashtable projects = new Hashtable();
			projects.Add(_project1.Name, _project1);
			projects.Add(project3.Name, project3);
			config.Projects = projects;
			config.RaiseConfigurationChangedEvent();

			// verify configuration projects have been updated
			Assertion.AssertEquals(_project1, cc.GetProject(_project1.Name));
			Assertion.AssertNull(cc.GetProject(_project2.Name));
			Assertion.AssertEquals(project3, cc.GetProject(project3.Name));

			// verify configuration schedulers have been updated
			Assertion.AssertEquals(2, cc.Schedulers.Count);
			Assertion.AssertEquals(_project1, ((Scheduler)cc.Schedulers[0]).Project);
			Assertion.AssertEquals(newSchedule, ((Scheduler)cc.Schedulers[0]).Schedule);
			//TODO: ensure that schedulers are appropriately restarted after reload
//			Assertion.AssertEquals(SchedulerState.Running, ((Scheduler)cc.Schedulers[0]).State);
			Assertion.AssertEquals(project3, ((Scheduler)cc.Schedulers[1]).Project);
//			Assertion.AssertEquals(SchedulerState.Stopped, ((Scheduler)cc.Schedulers[1]).State);
		}

		[Test, Ignore("Implement this test")]
		public void HandleChangedConfigurationWhileSchedulersAreRunning()
		{
		}

		[Test]
		public void StartStopAndWaitForExit()
		{
			_mockConfig.ExpectAndReturn("LoadProjects", _projects);

			CruiseControl cc = new CruiseControl((IConfigurationLoader)_mockConfig.MockInstance);
			cc.Start();
			foreach (IScheduler scheduler in cc.Schedulers)
			{
				Assertion.AssertEquals(SchedulerState.Running, scheduler.State);
			}
			cc.Stop();
			cc.WaitForExit();

			foreach (IScheduler scheduler in cc.Schedulers)
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
//
//            // configuration has not changed
//			_mockConfig.ExpectAndReturn("HasConfigurationChanged", false);
//			// _mockConfig.ExpectNoCall("LoadProjects");		// fix NMock to get this to work
//			_cc.RunIntegration();
//
//			_mockConfig.Verify();
//			Assertion.AssertEquals(2, _project1.Runs);
//			Assertion.AssertEquals(2, _project2.Runs);
//		}

////		public void TestStartAndStopSingleProject()
////		{
////			_cc.Start(_project1);
////			Assertion.AssertEquals(1, _cc.Threads.Count);
////			Thread.Sleep(1000);
////			_cc.Stop(_project1);
////
////			Assertion.AssertEquals(0, _cc.Threads.Count);
////			Assertion.AssertEquals(2, _project1.Runs);
////			Assertion.AssertEquals(1, _project1.Starts);
////			Assertion.AssertEquals(1, _project1.Stops);
////		}
////
////		public void TestStartAndStopProjectTwice()
////		{
////			_cc.Start(_project1);		// 0 stops, 1 start
////			Assertion.AssertEquals(1, _cc.Threads.Count);
////			Thread.Sleep(1000);
////
////			_cc.Start(_project1);		// 1 stop, 2 start
////			Assertion.AssertEquals(1, _cc.Threads.Count);
////			Thread.Sleep(1000);
////			_cc.Stop(_project1);		// 2 stop, 2 start
////			Assertion.AssertEquals(0, _cc.Threads.Count);
////			_cc.Stop(_project1);		// 2 stop, 2 start
////			Assertion.AssertEquals(0, _cc.Threads.Count);
////
////			Assertion.AssertEquals(2, _project1.Starts);
////			Assertion.AssertEquals(2, _project1.Stops);
////		}
////
////		public void TestStartAndStopMultipleProjects()
////		{
////			_mockConfig.ExpectAndReturn("HasConfigurationChanged", true);
////			_mockConfig.ExpectAndReturn("LoadProjects", _projects);
////
////			_cc.Start();
////			Thread.Sleep(1000);
////			Assertion.AssertEquals(2, _cc.Threads.Count);
////			_cc.Stop();
////			Assertion.AssertEquals(0, _cc.Threads.Count);
////		}
////
////		public void TestStopSleepingProject()
////		{
////			_project1.Sleep = 100000;
////			_cc.Start(_project1);
////			Assertion.AssertEquals(1, _cc.Threads.Count);
////			Thread.Sleep(1000);
////
////			_cc.Stop(_project1);
////			Assertion.AssertEquals(0, _cc.Threads.Count);
////		}
////
////		// test stop if thread has already stopped
////		public void TestStopAlreadyStoppedProject()
////		{
////			_cc.Start(_project1);
////			Thread.Sleep(1000);
////			_project1.Stop();
////			Thread.Sleep(1000);
////			_cc.Stop(_project1);
////			Assertion.AssertEquals(0, _cc.Threads.Count);
////		}
////
////		public void TestDisposeIfThreadsAreNotStopped()
////		{
////			_mockConfig.ExpectAndReturn("HasConfigurationChanged", true);
////			_mockConfig.ExpectAndReturn("LoadProjects", _projects);
////
////			
////			_cc.Start();
////			Thread.Sleep(2);
////			// do not shut down threads -- need to verify dispose called
////			_cc.Dispose();
////		}
	}
}
