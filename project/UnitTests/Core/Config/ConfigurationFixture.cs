using System.Xml;

using ThoughtWorks.CruiseControl.Core.Util;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Config
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
			return string.Format("<cruisecontrol>{0}</cruisecontrol>", projectXml);
		}

		public static string GenerateProjectXml(string name, string buildXml, string sourceControlXml, string publishersXml, string scheduleXml, string historyXml)
		{
			return string.Format(@"<project name=""{0}"">{1}{2}{3}{4}{5}</project>", 
				name, buildXml, sourceControlXml, publishersXml, scheduleXml, historyXml);
		}

		public static string GenerateProjectXml(string name)
		{
			return GenerateProjectXml(name, GenerateMockBuildXml(), GenerateNullSourceControlXml(), GenerateMockPublisherXml(), GenerateScheduleXml(), GenerateStateManagerXml());
		}

		public static string GenerateMockBuildXml()
		{
			return @"<build type=""mockbuildrunner""></build>";
		}

		public static string GenerateNullSourceControlXml()
		{
			return @"<sourcecontrol type=""nullSourceControl""></sourcecontrol>";
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
			return string.Format(@"<schedule type=""schedule"" sleepSeconds=""1"" iterations=""{0}""/>", iterations);
		}

		public static string GenerateStateManagerXml()
		{
			return @"<state type=""state"" />";
		}

		public static string GenerateStateManagerXml(string dir)
		{
			return string.Format(@"<state type=""state"" directory=""{0}"" />", dir);
		}
	}
}