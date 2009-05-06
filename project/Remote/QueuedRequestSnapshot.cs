using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// A snapshot of the state of an item on a particular named queue at this moment in time.
	/// </summary>
	[Serializable]
    [XmlRoot("queueRequest")]
	public class QueuedRequestSnapshot
	{
		private string projectName;
	    private ProjectActivity activity;
        private DateTime requestTime;

        /// <summary>
        /// Initialise a new blank <see cref="QueuedRequestSnapshot"/>.
        /// </summary>
        public QueuedRequestSnapshot()
        {
        }

        /// <summary>
        /// Initialise a new populated <see cref="QueuedRequestSnapshot"/>.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="activity"></param>
		public QueuedRequestSnapshot(string projectName, ProjectActivity activity)
            : this(projectName, activity, DateTime.MinValue) { }

        /// <summary>
        /// Initialise a new populated <see cref="QueuedRequestSnapshot"/>.
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="activity"></param>
        /// <param name="requestTime"></param>
        public QueuedRequestSnapshot(string projectName, ProjectActivity activity, DateTime requestTime)
		{
			this.projectName = projectName;
            this.activity = activity;
            this.requestTime = requestTime;
		}

        /// <summary>
        /// The name of the project.
        /// </summary>
        [XmlAttribute("projectName")]
		public string ProjectName
		{
			get { return projectName; }
            set { projectName = value; }
		}

        /// <summary>
        /// The current activity.
        /// </summary>
        [XmlElement("activity")]
	    public ProjectActivity Activity
	    {
	        get { return activity; }
            set { activity = value; }
	    }

        /// <summary>
        /// The time of the request.
        /// </summary>
        [XmlAttribute("time")]
        public DateTime RequestTime
        {
            get { return requestTime; }
            set { requestTime = value; }
        }
	}
}
