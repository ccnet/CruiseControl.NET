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
		public DashboardProject[] Projects;
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

		[XmlAttribute()]
		public string category;
	}

    [XmlRoot(Namespace = "", IsNullable = false, ElementName = "CruiseControl")]
    public class DashboardCruiseServerSnapshot
    {
        [XmlArray("Projects")]
        [XmlArrayItem("Project", typeof(DashboardProject))]
        public DashboardProject[] Projects;

        [XmlArray("Queues")]
        [XmlArrayItem("Queue", typeof(DashboardQueue))]
        public DashboardQueue[] Queues;
    }

    public class DashboardQueue
    {
        [XmlAttribute("name", DataType = "NMTOKEN")]
        public string Name;

        [XmlElement("Request")]
        public DashboardQueuedRequest[] Requests;
    }

    public class DashboardQueuedRequest
    {
        [XmlAttribute("projectName", DataType = "NMTOKEN")]
        public string ProjectName;
    }

	public class DashboardXmlParser : IDashboardXmlParser
	{
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(DashboardProjects));
        private readonly XmlSerializer cruiseServerSerializer = new XmlSerializer(typeof(DashboardCruiseServerSnapshot));

		private DashboardProjects GetProjects(string sourceXml)
		{
			return (DashboardProjects) serializer.Deserialize(new StringReader(sourceXml));
		}

		public string[] ExtractProjectNames(string sourceXml)
		{
		    CruiseServerSnapshot cruiseServerSnapshot = ExtractAsCruiseServerSnapshot(sourceXml);

            if (cruiseServerSnapshot.ProjectStatuses == null)
				return new string[0];

            string[] retVal = new string[cruiseServerSnapshot.ProjectStatuses.Length];
            for (int i = 0; i < cruiseServerSnapshot.ProjectStatuses.Length; i++)
			{
                retVal[i] = cruiseServerSnapshot.ProjectStatuses[i].Name;
			}
			
			return retVal;
		}

	    public CruiseServerSnapshot ExtractAsCruiseServerSnapshot(string sourceXml)
	    {
	        const string CRUISE_CONTROL_NODE = "<CruiseControl";
            if (sourceXml.IndexOf(CRUISE_CONTROL_NODE) != -1)
            {
                return BuildCruiseServerSnapshotFromProjectsAndQueues(sourceXml);
            }
            else
            {
                return BuildCruiseServerSnapshotFromProjectsOnly(sourceXml);
            }
	    }

        private CruiseServerSnapshot BuildCruiseServerSnapshotFromProjectsAndQueues(string sourceXml)
	    {
            DashboardCruiseServerSnapshot dashboardCruiseServerSnapshot = GetDashboardCruiseServerSnapshot(sourceXml);
            
            ProjectStatus[] projectStatuses = ConvertDashboardProjects(dashboardCruiseServerSnapshot.Projects);
            QueueSetSnapshot queueSetSnapshot = ConvertDashboardQueues(dashboardCruiseServerSnapshot.Queues);
            
            return new CruiseServerSnapshot(projectStatuses, queueSetSnapshot);
        }

        private CruiseServerSnapshot BuildCruiseServerSnapshotFromProjectsOnly(string sourceXml)
	    {
            DashboardProjects dashboardProjects = GetProjects(sourceXml);
            
            ProjectStatus[] projectStatuses = ConvertDashboardProjects(dashboardProjects.Projects);
	        
            return new CruiseServerSnapshot(projectStatuses, null);
	    }

	    private ProjectStatus[] ConvertDashboardProjects(DashboardProject[] dashboardProjects)
        {
            ProjectStatus[] projectStatuses = null;
            if (dashboardProjects != null)
            {
                projectStatuses = new ProjectStatus[dashboardProjects.Length];
                for (int index = 0; index < dashboardProjects.Length; index++)
                {
                    DashboardProject dashboardProject = dashboardProjects[index];
                    projectStatuses[index] = new ProjectStatus(
                        dashboardProject.name,
                        dashboardProject.category,
                        new ProjectActivity(dashboardProject.activity),
                        (IntegrationStatus)Enum.Parse(typeof(IntegrationStatus), dashboardProject.lastBuildStatus),
                        ProjectIntegratorState.Running,
                        dashboardProject.webUrl,
                        dashboardProject.lastBuildTime,
                        dashboardProject.lastBuildLabel,
                        dashboardProject.lastBuildLabel,
                        dashboardProject.nextBuildTime);
                }
            }
            return projectStatuses;
        }

	    private QueueSetSnapshot ConvertDashboardQueues(DashboardQueue[] dashboardQueues)
        {
            QueueSetSnapshot queueSetSnapshot = new QueueSetSnapshot();
            if (dashboardQueues != null)
            {
                foreach (DashboardQueue dashboardQueue in dashboardQueues)
                {
                    QueueSnapshot queueSnapshot = new QueueSnapshot(dashboardQueue.Name);
                    if (dashboardQueue.Requests != null)
                    {
                        foreach (DashboardQueuedRequest dashboardQueuedRequest in dashboardQueue.Requests)
                        {
                            QueuedRequestSnapshot queuedRequestSnapshot = new QueuedRequestSnapshot(
                                dashboardQueuedRequest.ProjectName);
                            queueSnapshot.Requests.Add(queuedRequestSnapshot);
                        }
                    }
                    queueSetSnapshot.Queues.Add(queueSnapshot);
                }
            }
            return queueSetSnapshot;
        }

        private DashboardCruiseServerSnapshot GetDashboardCruiseServerSnapshot(string sourceXml)
        {
            return (DashboardCruiseServerSnapshot)cruiseServerSerializer.Deserialize(new StringReader(sourceXml));
        }
    }
}