using System;
using tw.ccnet.core;
using tw.ccnet.core.configuration;
using tw.ccnet.core.schedule;
using tw.ccnet.core.state;
using tw.ccnet.core.util;

namespace integration
{
	public class IntegrationFixture
	{
		public static CruiseControl CreateCruiseControl(string configDirName, params string[] projects)
		{
			// create config
			string configFile = ConfigurationFileFixture.CreateConfigurationFile(configDirName, projects);
			ConfigurationLoader loader = new ConfigurationLoader(configFile);

			return new CruiseControl(loader);
		}

		public static Schedule CreateSchedule(int iterations)
		{
			Schedule schedule = new Schedule();
			schedule.TotalIterations = iterations;
			return schedule;
		}

		public static Schedule CreateSchedule()
		{
			return CreateSchedule(Schedule.Infinite);
		}

		public static IntegrationEventCounter AddIntegrationEventHandler(CruiseControl cc, string projectName)
		{
			IntegrationEventCounter counter = new IntegrationEventCounter();
			((Project)cc.GetProject(projectName)).AddIntegrationEventHandler(counter.IntegrationEventHandler);
			return counter;
		}

		public static IntegrationResult LoadIntegrationResult(string dirName)
		{
			IntegrationStateManager stateManager = new IntegrationStateManager();
			stateManager.Directory = TempFileUtil.GetTempPath(dirName);
			return stateManager.Load();
		}
	}

	public class IntegrationEventCounter : IIntegrationEventHandler
	{
		private int count = 0;

		public IntegrationEventHandler IntegrationEventHandler
		{
			get { return new IntegrationEventHandler(HandleEvent); }
		}

		private void HandleEvent(object sender, IntegrationResult result)
		{
			count++;
		}

		public int EventCount
		{
			get { return count; }
		}
	}
}
