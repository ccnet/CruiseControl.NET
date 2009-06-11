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

        /// <summary>
        /// Initialise a new blank <see cref="CruiseServerSnapshot"/>.
        /// </summary>
        public CruiseServerSnapshot()
        {
            projectStatuses = new ProjectStatus[0];
            queueSetSnapshot = new QueueSetSnapshot();
        }

        /// <summary>
        /// Initialise a new populated <see cref="CruiseServerSnapshot"/>.
        /// </summary>
        /// <param name="projectStatuses"></param>
        /// <param name="queueSetSnapshot"></param>
        public CruiseServerSnapshot(ProjectStatus[] projectStatuses, QueueSetSnapshot queueSetSnapshot)
        {
            this.projectStatuses = projectStatuses;
            this.queueSetSnapshot = queueSetSnapshot;
        }

        /// <summary>
        /// The current state of the projects.
        /// </summary>
        [XmlArray("projects")]
        [XmlArrayItem("projectStatus")]
        public ProjectStatus[] ProjectStatuses
        {
            get { return projectStatuses; }
            set { projectStatuses = value; }
        }

        /// <summary>
        /// The current state of the queues.
        /// </summary>
        [XmlElement("queueSet")]
        public QueueSetSnapshot QueueSetSnapshot
        {
            get { return queueSetSnapshot; }
            set { queueSetSnapshot = value; }
        }

        /// <summary>
        /// Checks if a snapshot has changed.
        /// </summary>
        /// <param name="queueSetSnapshotToCompare"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Retrieves the status of a project.
        /// </summary>
        /// <param name="projectName"></param>
        /// <returns></returns>
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
		
        /// <summary>
        /// Retrieves the hashcode for the snapshot.
        /// </summary>
        /// <returns></returns>
		public override int GetHashCode()
		{
			int hashCode = 0;
			unchecked {
				if (projectStatuses != null) hashCode += 1000000007 * projectStatuses.GetHashCode(); 
				if (queueSetSnapshot != null) hashCode += 1000000009 * queueSetSnapshot.GetHashCode(); 
			}
			return hashCode;
		}
    	
        /// <summary>
        /// Compares two snapshots to see if they are equal.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public override bool Equals(object obj)
		{
			CruiseServerSnapshot other = obj as CruiseServerSnapshot;
			if (other == null) return false; 
			return object.Equals(this.projectStatuses, other.projectStatuses) && object.Equals(this.queueSetSnapshot, other.queueSetSnapshot);
		}
		
    }
}
