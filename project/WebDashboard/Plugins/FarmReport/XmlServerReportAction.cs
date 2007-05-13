using System.IO;
using System.Xml;

using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;
using ThoughtWorks.CruiseControl.WebDashboard.ServerConnection;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.FarmReport
{
	public class XmlServerReportAction : IAction
	{
		public const string ACTION_NAME = "XmlServerReport";

		private readonly IFarmService farmService;

        public XmlServerReportAction(IFarmService farmService)
		{
			this.farmService = farmService;
		}

		public IResponse Execute(IRequest request)
		{
            CruiseServerSnapshotListAndExceptions allCruiseServerSnapshots =
                farmService.GetCruiseServerSnapshotListAndExceptions();

			StringWriter stringWriter = new StringWriter();
			XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartElement("CruiseControl");

	        xmlWriter.WriteStartElement("Projects");
		    foreach (CruiseServerSnapshotOnServer snapshotOnServer in allCruiseServerSnapshots.SnapshotAndServerList)
		    {
                WriteProjects(xmlWriter, snapshotOnServer.CruiseServerSnapshot.ProjectStatuses);
		    }
	        xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Queues");
            foreach (CruiseServerSnapshotOnServer snapshotOnServer in allCruiseServerSnapshots.SnapshotAndServerList)
            {
                WriteQueueSetSnapshot(xmlWriter, snapshotOnServer.CruiseServerSnapshot.QueueSetSnapshot);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            return new XmlFragmentResponse(stringWriter.ToString());
		}

	    private static void WriteProjects(XmlTextWriter xmlWriter, ProjectStatus[] projectStatuses)
	    {
            foreach (ProjectStatus projectStatus in projectStatuses)
	        {
                WriteProjectStatus(xmlWriter, projectStatus);
	        }
	    }

        private static void WriteProjectStatus(XmlTextWriter xmlWriter, ProjectStatus projectStatus)
		{
			xmlWriter.WriteStartElement("Project");
            xmlWriter.WriteAttributeString("name", projectStatus.Name);
            xmlWriter.WriteAttributeString("category", projectStatus.Category);
            xmlWriter.WriteAttributeString("activity", projectStatus.Activity.ToString());
            xmlWriter.WriteAttributeString("lastBuildStatus", projectStatus.BuildStatus.ToString());
            xmlWriter.WriteAttributeString("lastBuildLabel", projectStatus.LastSuccessfulBuildLabel);
            xmlWriter.WriteAttributeString("lastBuildTime", XmlConvert.ToString(projectStatus.LastBuildDate, XmlDateTimeSerializationMode.Local));
            xmlWriter.WriteAttributeString("nextBuildTime", XmlConvert.ToString(projectStatus.NextBuildTime, XmlDateTimeSerializationMode.Local));
            xmlWriter.WriteAttributeString("webUrl", projectStatus.WebURL);
			xmlWriter.WriteEndElement();
		}

        private static void WriteQueueSetSnapshot(XmlTextWriter xmlWriter, QueueSetSnapshot queueSet)
        {
            foreach (QueueSnapshot queue in queueSet.Queues)
            {
                WriteQueueSnapshot(xmlWriter, queue);
            }
       }

        private static void WriteQueueSnapshot(XmlTextWriter xmlWriter, QueueSnapshot queue)
        {
            xmlWriter.WriteStartElement("Queue");
            xmlWriter.WriteAttributeString("name", queue.QueueName);

            foreach (QueuedRequestSnapshot request in queue.Requests)
            {
                WriteQueuedRequestSnapshot(xmlWriter, request);
            }

            xmlWriter.WriteEndElement();
        }

        private static void WriteQueuedRequestSnapshot(XmlTextWriter xmlWriter, QueuedRequestSnapshot queuedRequest)
        {
            xmlWriter.WriteStartElement("Request");
            xmlWriter.WriteAttributeString("projectName", queuedRequest.ProjectName);
            xmlWriter.WriteAttributeString("activity", queuedRequest.Activity.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}