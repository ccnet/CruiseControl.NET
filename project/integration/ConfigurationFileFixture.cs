using System;
using System.Xml;
using tw.ccnet.core.configuration.test;
using tw.ccnet.core;
using tw.ccnet.core.util;
using tw.ccnet.core.schedule;

namespace integration
{
	public class ConfigurationFileFixture
	{
		public static string CreateConfigurationFile(string configDirName, params string[] projects)
		{
			string xml = ConfigurationFixture.GenerateConfigXml(String.Concat(projects));
			return TempFileUtil.CreateTempFile(configDirName, "ccnet.config", xml); 
		}

		public static string GenerateSimpleProjectXml(string projectName, Schedule schedule)
		{
			string historyXml = ConfigurationFixture.GenerateStateManagerXml(TempFileUtil.CreateTempDir(projectName));
			return GenerateSimpleProjectXml(projectName, schedule, historyXml);
		}

		public static string GenerateSimpleProjectXml(string projectName, Schedule schedule, string historyXml)
		{
			string buildXml = ConfigurationFixture.GenerateMockBuildXml();
			string sourceControlXml = ConfigurationFixture.GenerateDefaultSourceControlXml();
			string scheduleXml = (schedule == null) ? null : 
				ConfigurationFixture.GenerateScheduleXml(schedule.TotalIterations);
			return ConfigurationFixture.GenerateProjectXml(projectName, buildXml, sourceControlXml, null, scheduleXml, historyXml);
		}
	}
}
