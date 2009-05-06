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

        public QueuedRequestSnapshot()
        {
        }

		public QueuedRequestSnapshot(string projectName, ProjectActivity activity)
            : this(projectName, activity, DateTime.MinValue) { }

        public QueuedRequestSnapshot(string projectName, ProjectActivity activity, DateTime requestTime)
		{
			this.projectName = projectName;
            this.activity = activity;
            this.requestTime = requestTime;
		}

        [XmlAttribute("projectName")]
		public string ProjectName
		{
			get { return projectName; }
            set { projectName = value; }
		}

        [XmlElement("activity")]
	    public ProjectActivity Activity
	    {
	        get { return activity; }
            set { activity = value; }
	    }

        [XmlAttribute("time")]
        public DateTime RequestTime
        {
            get { return requestTime; }
            set { requestTime = value; }
        }
	}
}
