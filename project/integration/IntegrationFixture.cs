using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Schedules;

namespace integration
{
	public class IntegrationFixture
	{
		public static CruiseServer CreateCruiseControl(string configDirName, params string[] projects)
		{
			// create config
			string configFile = ConfigurationFileFixture.CreateConfigurationFile(configDirName, projects);
			ConfigurationLoader loader = new ConfigurationLoader(configFile);

			return new CruiseServer(loader);
		}

		public static Schedule CreateSchedule(int iterations)
		{
			Schedule schedule = new Schedule();
			schedule.SleepSeconds = 4;
			schedule.TotalIterations = iterations;
			return schedule;
		}

		public static Schedule CreateSchedule()
		{
			return CreateSchedule(Schedule.Infinite);
		}

		public static IntegrationEventCounter AddIntegrationEventHandler(CruiseServer cc, string projectName)
		{
			IntegrationEventCounter counter = new IntegrationEventCounter();
			((Project)cc.GetProject(projectName)).IntegrationCompleted += counter.IntegrationCompletedEventHandler;
			return counter;
		}

		public static IntegrationResult LoadIntegrationResult(string dirName)
		{
			IntegrationStateManager stateManager = new IntegrationStateManager();
			stateManager.Directory = TempFileUtil.GetTempPath(dirName);
			return stateManager.LoadState();
		}
	}

	public class IntegrationEventCounter : IIntegrationCompletedEventHandler
	{
		private int count = 0;

		public IntegrationCompletedEventHandler IntegrationCompletedEventHandler
		{
			get { return new IntegrationCompletedEventHandler(HandleEvent); }
		}

		private void HandleEvent(object sender, IntegrationCompletedEventArgs result)
		{
			count++;
		}

		public int EventCount
		{
			get { return count; }
		}
	}
}
