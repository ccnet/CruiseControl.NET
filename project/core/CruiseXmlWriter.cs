using System.Collections.Generic;
using System.IO;
using System.Xml;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
    public class CruiseXmlWriter
    {
        public string Write(IEnumerable<CruiseServerSnapshot> cruiseServerSnapshots)
        {
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.WriteStartElement("CruiseControl");

            xmlWriter.WriteStartElement("Projects");
            foreach (CruiseServerSnapshot snapshot in cruiseServerSnapshots)
            {
                WriteProjects(xmlWriter, snapshot.ProjectStatuses);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("Queues");
            foreach (CruiseServerSnapshot snapshot in cruiseServerSnapshots)
            {
                WriteQueueSetSnapshot(xmlWriter, snapshot.QueueSetSnapshot);
            }
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();
            return stringWriter.ToString();
        }

        public string Write(ProjectStatus projectStatus)
        {
            return Write(new CruiseServerSnapshot[] { new CruiseServerSnapshot(new ProjectStatus[] { projectStatus }, new QueueSetSnapshot()) });
        }

        private static void WriteProjects(XmlWriter xmlWriter, IEnumerable<ProjectStatus> projectStatuses)
        {
            foreach (ProjectStatus projectStatus in projectStatuses)
            {
                WriteProjectStatus(xmlWriter, projectStatus);
            }
        }

        private static void WriteProjectStatus(XmlWriter xmlWriter, ProjectStatus projectStatus)
        {
            xmlWriter.WriteStartElement("Project");
            xmlWriter.WriteAttributeString("name", projectStatus.Name);
            xmlWriter.WriteAttributeString("category", projectStatus.Category);
            xmlWriter.WriteAttributeString("activity", projectStatus.Activity.ToString());
			xmlWriter.WriteAttributeString("status", projectStatus.Status.ToString());
            xmlWriter.WriteAttributeString("lastBuildStatus", projectStatus.BuildStatus.ToString());
            xmlWriter.WriteAttributeString("lastBuildLabel", projectStatus.LastSuccessfulBuildLabel);
            xmlWriter.WriteAttributeString("lastBuildTime", XmlConvert.ToString(projectStatus.LastBuildDate, XmlDateTimeSerializationMode.Local));
            xmlWriter.WriteAttributeString("nextBuildTime", XmlConvert.ToString(projectStatus.NextBuildTime, XmlDateTimeSerializationMode.Local));
            xmlWriter.WriteAttributeString("webUrl", projectStatus.WebURL);
            xmlWriter.WriteAttributeString("buildStage", projectStatus.BuildStage);
            xmlWriter.WriteAttributeString("serverName", projectStatus.ServerName);
            xmlWriter.WriteEndElement();
        }

        private static void WriteQueueSetSnapshot(XmlWriter xmlWriter, QueueSetSnapshot queueSet)
        {
            foreach (QueueSnapshot queue in queueSet.Queues)
            {
                WriteQueueSnapshot(xmlWriter, queue);
            }
        }

        private static void WriteQueueSnapshot(XmlWriter xmlWriter, QueueSnapshot queue)
        {
            xmlWriter.WriteStartElement("Queue");
            xmlWriter.WriteAttributeString("name", queue.QueueName);

            foreach (QueuedRequestSnapshot request in queue.Requests)
            {
                WriteQueuedRequestSnapshot(xmlWriter, request);
            }

            xmlWriter.WriteEndElement();
        }

        private static void WriteQueuedRequestSnapshot(XmlWriter xmlWriter, QueuedRequestSnapshot queuedRequest)
        {
            xmlWriter.WriteStartElement("Request");
            xmlWriter.WriteAttributeString("projectName", queuedRequest.ProjectName);
            xmlWriter.WriteAttributeString("activity", queuedRequest.Activity.ToString());
            xmlWriter.WriteEndElement();
        }
    }
}
