using System;
using System.Xml;
using tw.ccnet.core.util;

namespace tw.ccnet.core.configuration.test
{
	public class ConfigurationFixture
	{
		public const int SleepTime = 100;

		public static XmlDocument GenerateConfig(string projectXml)
		{
			return XmlUtil.CreateDocument(GenerateConfigXml(projectXml));
		}
		
		public static string GenerateConfigXml()
		{
			return GenerateConfigXml(GenerateProjectXml("test"));
		}

		public static string GenerateConfigXml(string projectXml)
		{
			return String.Format("<cruisecontrol>{0}</cruisecontrol>", projectXml);
		}

		public static string GenerateProjectXml(string name, string buildXml, string sourceControlXml, string publishersXml, string scheduleXml, string historyXml)
		{
			return String.Format(@"<project name=""{0}"" sleepTime=""{3}"">{1}{2}{4}{5}{6}</project>", 
				name, buildXml, sourceControlXml, SleepTime, publishersXml, scheduleXml, historyXml);
		}

		public static string GenerateProjectXml(string name)
		{
			return GenerateProjectXml(name, GenerateMockBuildXml(), GenerateDefaultSourceControlXml(), GenerateMockPublisherXml(), GenerateScheduleXml(), GenerateXmlBuildHistoryXml());
		}

		public static string GenerateMockBuildXml()
		{
			return @"<build type=""mockbuildrunner""></build>";
		}

		public static string GenerateDefaultSourceControlXml()
		{
			return @"<sourcecontrol type=""defaultsourcecontrol""></sourcecontrol>";
		}

		public static string GenerateMockPublisherXml()
		{
			return @"<publishers><mockpublisher/></publishers>";
		}

		public static string GenerateScheduleXml()
		{
			return GenerateScheduleXml(1);
		}

		public static string GenerateScheduleXml(int iterations)
		{
			return String.Format(@"<schedule type=""schedule"" timeout=""1"" iterations=""{0}""/>", iterations);
		}

		public static string GenerateXmlBuildHistoryXml()
		{
			return @"<history type=""xmlhistory"" />";
		}

		public static string GenerateXmlBuildHistoryXml(string historyDir)
		{
			return String.Format(@"<history type=""xmlhistory"" historyDir=""{0}"" />", historyDir);
		}
	}
}