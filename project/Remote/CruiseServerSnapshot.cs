using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
    /// <summary>
    /// Contains a snapshot of the current CC.Net server status and activity.
    /// </summary>
    [Serializable]
    [XmlRoot("serverSnapshot")]
    public class CruiseServerSnapshot
    {
        private ProjectStatus[] projectStatuses;
        private QueueSetSnapshot queueSetSnapshot;

        public CruiseServerSnapshot()
        {
            projectStatuses = new ProjectStatus[0];
            queueSetSnapshot = new QueueSetSnapshot();
        }

        public CruiseServerSnapshot(ProjectStatus[] projectStatuses, QueueSetSnapshot queueSetSnapshot)
        {
            this.projectStatuses = projectStatuses;
            this.queueSetSnapshot = queueSetSnapshot;
        }

        [XmlArray("projects")]
        [XmlArrayItem("projectStatus")]
        public ProjectStatus[] ProjectStatuses
        {
            get { return projectStatuses; }
            set { projectStatuses = value; }
        }

        [XmlElement("queueSet")]
        public QueueSetSnapshot QueueSetSnapshot
        {
            get { return queueSetSnapshot; }
            set { queueSetSnapshot = value; }
        }

        public bool IsQueueSetSnapshotChanged(QueueSetSnapshot queueSetSnapshotToCompare)
        {
            if (queueSetSnapshotToCompare == null)
            {
                if (queueSetSnapshot == null)
                    return false;
                else
                    return true;
            }
            if (queueSetSnapshot == null)
                return true;
            if (queueSetSnapshotToCompare.Queues.Count != queueSetSnapshot.Queues.Count)
                return true;

            for (int queueIndex = 0; queueIndex < queueSetSnapshot.Queues.Count; queueIndex++)
            {
                QueueSnapshot queueSnapshot = queueSetSnapshot.Queues[queueIndex];
                QueueSnapshot queueSnapshotToCompare = queueSetSnapshotToCompare.Queues[queueIndex];
                if (queueSnapshotToCompare.QueueName != queueSnapshot.QueueName)
                    return true;
                if (queueSnapshotToCompare.Requests.Count != queueSnapshot.Requests.Count)
                    return true;

                for (int requestIndex = 0; requestIndex < queueSnapshot.Requests.Count; requestIndex++)
                {
                    QueuedRequestSnapshot request = queueSnapshot.Requests[requestIndex];
                    QueuedRequestSnapshot requestToCompare = queueSnapshotToCompare.Requests[requestIndex];
                    if (requestToCompare.ProjectName != request.ProjectName)
                        return true;
                    if (requestToCompare.Activity != request.Activity)
                        return true;
                }
            }
            return false;
        }

		public ProjectStatus GetProjectStatus(string projectName)
		{
			foreach (ProjectStatus projectStatus in ProjectStatuses)
			{
				if (projectStatus.Name == projectName)
				{
					return projectStatus;
				}
			}
			return null;
		}
    }
}
