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

        public static string GenerateProjectXml(string name, string queueName, string buildXml, string sourceControlXml, string publishersXml, string historyXml)
		{
            return string.Format(@"<project name=""{0}"" queue=""{5}"">{1}{2}{3}{4}</project>", 
				name, buildXml, sourceControlXml, publishersXml, historyXml, queueName);
		}

        public static string GenerateProjectXml(string name, string buildXml, string sourceControlXml, string publishersXml, string historyXml)
        {
            return string.Format(@"<project name=""{0}"">{1}{2}{3}{4}</project>",
                name, buildXml, sourceControlXml, publishersXml, historyXml);
        }

        public static string GenerateProjectXml(string name, string queueName)
        {
            if (string.IsNullOrEmpty(queueName))
            {
                return GenerateProjectXml(name, GenerateNullTasksXml(), GenerateNullSourceControlXml(), GenerateNullPublishersXml(), GenerateStateManagerXml());
            }
            else
            {
                return GenerateProjectXml(name, queueName, GenerateNullTasksXml(), GenerateNullSourceControlXml(), GenerateNullPublishersXml(), GenerateStateManagerXml());
            }
        }

		public static string GenerateProjectXml(string name)
		{
			return GenerateProjectXml(name, GenerateNullTasksXml(), GenerateNullSourceControlXml(), GenerateNullPublishersXml(), GenerateStateManagerXml());
		}

		public static string GenerateNullTasksXml()
		{
			return @"<tasks><nullTask /></tasks>";
		}

		public static string GenerateNullSourceControlXml()
		{
			return @"<sourcecontrol type=""nullSourceControl""></sourcecontrol>";
		}

		public static string GenerateNullPublishersXml()
		{
			return @"<publishers><nullTask /></publishers>";
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