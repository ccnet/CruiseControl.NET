using System;
using System.Threading;

using NMock;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class CruiseServerTest : CustomAssertion
	{
		DynamicMock _mockConfig;
		MockProject _project1;
		MockProject _project2;
		IConfiguration _configuration;
		CruiseServer _cc;

		[SetUp]
		protected void SetUp()
		{
			_project1 = new MockProject("project1", new Schedule(1, Schedule.Infinite));
			_project2 = new MockProject("project2", new Schedule(1, Schedule.Infinite));

			_configuration = new Configuration();
			_configuration.AddProject(_project1);
			_configuration.AddProject(_project2);

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
			_configuration.AddProject(projectWithoutSchedule);
			_mockConfig.ExpectAndReturn("Load", _configuration);

			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);

			// verify that projects are loaded
			_mockConfig.Verify();
			AssertEquals(_project1, _cc.GetProject("project1"));
			AssertEquals(_project2, _cc.GetProject("project2"));

			// verify that schedulers have been created
			AssertEquals(2, _cc.ProjectIntegrators.Length);
			AssertEquals(_project1, _cc.ProjectIntegrators[1].Project);
			AssertEquals(_project2, _cc.ProjectIntegrators[0].Project);
		}

		[Test]
		public void RunIntegration()
		{
			_mockConfig.ExpectAndReturn("Load", _configuration);
			((Schedule)_project1.Schedule).TotalIterations = 1;
			((Schedule)_project2.Schedule).TotalIterations = 1;

			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);
			_cc.Start(); // RunIntegration();
			_cc.WaitForExit();

			_mockConfig.Verify();
			AssertEquals(1, _project1.RunIntegration_CallCount);
			AssertEquals(1, _project2.RunIntegration_CallCount);
		}

		[Test]
		[Ignore("MikeRoberts - Failing intermittently - is this thread dependent?")]
		public void HandleChangedConfiguration()
		{
			MockConfigurationLoader config = new MockConfigurationLoader(_configuration);

			_cc = new CruiseServer(config);
			_cc.Start();

			// verify configuration projects and schedulers have been loaded
			AssertEquals(2, _cc.ProjectIntegrators.Length);
			AssertEquals(_project1, _cc.ProjectIntegrators[1].Project);
			AssertEquals(_project2, _cc.ProjectIntegrators[0].Project);

			// create new configuration - change schedule for project1, remove project2 and add project3
			Schedule newSchedule = new Schedule();
			_project1.Schedule = newSchedule;
			MockProject project3 = new MockProject("project3", new Schedule(1, 1));

			IConfiguration newConfig = new Configuration();
			newConfig.AddProject(_project1);
			newConfig.AddProject(project3);
			config.Configuration = newConfig;
			config.RaiseConfigurationChangedEvent();

			// verify configuration projects have been updated
			AssertEquals(_project1, _cc.GetProject(_project1.Name));
			AssertNull(_cc.GetProject(_project2.Name));
			AssertEquals(project3, _cc.GetProject(project3.Name));

			// verify configuration schedulers have been updated
			AssertEquals(2, _cc.ProjectIntegrators.Length);
			
			AssertEquals(_project1, _cc.ProjectIntegrators[0].Project);
			AssertEquals(newSchedule, _cc.ProjectIntegrators[0].Schedule);
			AssertEquals(ProjectIntegratorState.Running, _cc.ProjectIntegrators[0].State);
			
			AssertEquals(project3, _cc.ProjectIntegrators[1].Project);
			AssertEquals("project3 should be automatically started", ProjectIntegratorState.Running, _cc.ProjectIntegrators[1].State);

			_cc.Stop();
		}

		[Test, Ignore("Implement this test")]
		public void HandleChangedConfigurationWhileSchedulersAreRunning()
		{
		}

		[Test]
		public void StartStopAndWaitForExit()
		{
			_mockConfig.ExpectAndReturn("Load", _configuration);

			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);
			_cc.Start();
			foreach (IProjectIntegrator scheduler in _cc.ProjectIntegrators)
			{
				AssertEquals(ProjectIntegratorState.Running, scheduler.State);
			}
			_cc.Stop();
			_cc.WaitForExit();

			foreach (IProjectIntegrator scheduler in _cc.ProjectIntegrators)
			{
				AssertEquals(ProjectIntegratorState.Stopped, scheduler.State);
			}			
		}

		private class MockConfigurationLoader : IConfigurationLoader
		{
			private IConfiguration _configuration;
			private ConfigurationChangedHandler _handler;

			public MockConfigurationLoader(IConfiguration configuration)
			{
				_configuration = configuration;
			}

			public IConfiguration Configuration
			{
				set { _configuration = value; }
			}

			public IConfiguration Load()
			{
				return _configuration;
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
			_mockConfig.ExpectAndReturn("Load", _configuration);
			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);

			_cc.Start();
			Thread.Sleep(50);
			AssertEquals(2, _cc.ProjectIntegrators.Length);
			AssertEquals(ProjectIntegratorState.Running, _cc.ProjectIntegrators[0].State);
			AssertEquals(ProjectIntegratorState.Running, _cc.ProjectIntegrators[1].State);

			// try invoking start again
			_cc.Start();
			Thread.Sleep(1);
			AssertEquals(2, _cc.ProjectIntegrators.Length);
			AssertEquals(ProjectIntegratorState.Running, _cc.ProjectIntegrators[0].State);
			AssertEquals(ProjectIntegratorState.Running, _cc.ProjectIntegrators[1].State);

			_cc.Stop();

			_cc.WaitForExit();
			Thread.Sleep(1);

			AssertEquals(ProjectIntegratorState.Stopped, _cc.ProjectIntegrators[0].State);
			AssertEquals(ProjectIntegratorState.Stopped, _cc.ProjectIntegrators[1].State);

			// try invoking stop again
			_cc.Stop();
			Thread.Sleep(1);
			AssertEquals(ProjectIntegratorState.Stopped, _cc.ProjectIntegrators[0].State);
			AssertEquals(ProjectIntegratorState.Stopped, _cc.ProjectIntegrators[1].State);
		}

		[Test]
		public void IntegrateProjectSpecifiedByName()
		{
			_mockConfig.ExpectAndReturn("Load", _configuration);
			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);

			IntegrationResult result = _cc.RunIntegration(_project1.Name);
			AssertEquals(1, _project1.RunIntegration_CallCount);
		}

		[Test, ExpectedException(typeof(CruiseControlException))]
		public void TryIntegratingUnknownProject()
		{
			_mockConfig.ExpectAndReturn("Load", _configuration);
			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);

			IntegrationResult result = _cc.RunIntegration("does not exist");
		}

		[Test]
		public void ForceBuild_AlreadyBuilding()
		{
			string testProjectName = "TestProjectName";

			ThoughtWorks.CruiseControl.Core.Schedules.Test.MockSchedule schedule = new ThoughtWorks.CruiseControl.Core.Schedules.Test.MockSchedule();
			MockProject mockProject = new MockProject(testProjectName, schedule);
			mockProject.CurrentActivity = ThoughtWorks.CruiseControl.Remote.ProjectActivity.Building; // already building
			AssertEquals(0, schedule.ForceBuild_CallCount);

			_configuration = new Configuration();
			_configuration.AddProject(mockProject);
			_mockConfig.ExpectAndReturn("Load", _configuration);

			// we're testing this method
			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);
			_cc.ForceBuild(testProjectName);

			AssertEquals(1, schedule.ForceBuild_CallCount);
		}

		[Test]
		public void Abort()
		{
			_mockConfig.ExpectAndReturn("Load", _configuration);
			_cc = new CruiseServer((IConfigurationLoader)_mockConfig.MockInstance);
			_cc.Start();
			Thread.Sleep(0);
			AssertEquals(ProjectIntegratorState.Running, _cc.ProjectIntegrators[0].State);
			AssertEquals(ProjectIntegratorState.Running, _cc.ProjectIntegrators[1].State);

			_cc.Abort();
			AssertEquals(ProjectIntegratorState.Stopped, _cc.ProjectIntegrators[0].State);
			AssertEquals(ProjectIntegratorState.Stopped, _cc.ProjectIntegrators[1].State);
		}

		//		public void TestStopSleepingProject()
		//		{
		//			_project1.Sleep = 100000;
		//			_cc.Start(_project1);
		//			AssertEquals(1, _cc.Threads.Count);
		//			Thread.Sleep(1000);
		//
		//			_cc.Stop(_project1);
		//			AssertEquals(0, _cc.Threads.Count);
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
		//			AssertEquals(0, _cc.Threads.Count);
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
