using System;
using System.IO;
using System.Xml.Serialization;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	[XmlRoot(Namespace="", IsNullable=false, ElementName="Projects")]
	public class DashboardProjects
	{
		[XmlElement("Project")]
		public DashboardProject[] Project;
	}

	public class DashboardProject
	{
		[XmlAttribute(DataType="NMTOKEN")]
		public string name;

		[XmlAttribute(DataType="NMTOKEN")]
		public string activity;

		[XmlAttribute(DataType="NMTOKEN")]
		public string lastBuildStatus;

		[XmlAttribute(DataType="NMTOKEN")]
		public string lastBuildLabel;

		[XmlAttribute()]
		public DateTime lastBuildTime;

		[XmlAttribute()]
		public DateTime nextBuildTime;

		[XmlAttribute()]
		public string webUrl;
	}

	public class DashboardXmlParser : IDashboardXmlParser
	{
		private readonly XmlSerializer serializer = new XmlSerializer(typeof (DashboardProjects));

		public ProjectStatus ExtractAsProjectStatus(string sourceXml, string projectName)
		{
			DashboardProjects projects = (DashboardProjects) serializer.Deserialize(new StringReader(sourceXml));

			if (projects.Project != null)
			{
				foreach (DashboardProject project in projects.Project)
				{
					if (project.name == projectName)
					{
						return new ProjectStatus(
							project.name,
							new ProjectActivity(project.activity),
							(IntegrationStatus) Enum.Parse(typeof (IntegrationStatus), project.lastBuildStatus),
							ProjectIntegratorState.Running,
							project.webUrl,
							project.lastBuildTime,
							project.lastBuildLabel,
							project.lastBuildLabel,
							project.nextBuildTime);
					}
				}
			}

			throw new ApplicationException("Project " + projectName + " is not known to the dashboard");
		}
	}
}