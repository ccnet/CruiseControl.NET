using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.Remote
{
	/// <summary>
	/// Value type that contains extensive details about a project's most recent
	/// integration.
	/// </summary>
	/// <remarks>
	/// This class is serialized to persist CruiseControl.NET's state for a
	/// particular project, hence is is marked with a <see cref="SerializableAttribute"/>.
	/// </remarks>
	[Serializable]
    [XmlRoot("projectStatus")]
	public class ProjectStatus
	{
		private ProjectIntegratorState status;
		private IntegrationStatus buildStatus;
		private ProjectActivity activity = ProjectActivity.Sleeping;
		private string name;
		private string category;
		private string webURL;
		private SerializableDateTime lastBuildDate = SerializableDateTime.Default;
		private string lastBuildLabel;
		private string lastSuccessfulBuildLabel;
		private SerializableDateTime nextBuildTime = SerializableDateTime.Default;
        private string currentBuildStage;
        private string _serverName = Environment.MachineName;       // Store the machine name that this project is running on

        private string queue;
        private int queuePriority;

		public ProjectStatus()
		{}

		public ProjectStatus(string name, IntegrationStatus buildStatus, DateTime lastBuildDate)
		{
			this.name = name;
			this.buildStatus = buildStatus;
			this.lastBuildDate = new SerializableDateTime(lastBuildDate);
		}

        public ProjectStatus(string name, string category, ProjectActivity activity, IntegrationStatus buildStatus, ProjectIntegratorState status, string webURL, DateTime lastBuildDate, string lastBuildLabel, string lastSuccessfulBuildLabel, DateTime nextBuildTime, string buildStage, string queue, int queuePriority)
		{
			this.status = status;
			this.buildStatus = buildStatus;
			this.activity = activity;
			this.name = name;
			this.category = category;
			this.webURL = webURL;
			this.lastBuildDate = new SerializableDateTime(lastBuildDate);
			this.lastBuildLabel = lastBuildLabel;
			this.lastSuccessfulBuildLabel = lastSuccessfulBuildLabel;
			this.nextBuildTime = new SerializableDateTime(nextBuildTime);
            this.currentBuildStage = buildStage;
            this.queue = queue;
            this.queuePriority = queuePriority;
		}

        [XmlAttribute("stage")]
        public string BuildStage
        {
            get { return currentBuildStage; }
            set { currentBuildStage = value; }
        }

        /// <summary>
        /// The name of the server this status is from.
        /// </summary>
        [XmlAttribute("serverName")]
        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        [XmlAttribute("status")]
		public ProjectIntegratorState Status
		{
			get { return status; }
            set { status = value; }
		}

        [XmlAttribute("buildStatus")]
		public IntegrationStatus BuildStatus
		{
			get { return buildStatus; }
            set { buildStatus = value; }
		}

        [XmlElement("activity")]
		public ProjectActivity Activity
		{
			get { return activity; }
            set { activity = value; }
		}

        [XmlAttribute("name")]
		public string Name
		{
			get { return name; }
            set { name = value; }
		}

        /// <summary>
        /// The description of the project (optional).
        /// </summary>
        [XmlAttribute("description")]
        public string Description { get; set; }

        [XmlAttribute("category")]
		public string Category
		{
			get { return category; }
            set { category = value; }
		}

        [XmlAttribute("queueName")]
        public string Queue
        {
            get { return this.queue; }
            set { this.queue = value; }
        }

        [XmlAttribute("queuePriority")]
        public int QueuePriority
        {
            get { return this.queuePriority; }
            set { this.queuePriority = value; }
        }

        [XmlAttribute("url")]
        public string WebURL
		{
			get { return webURL; }
            set { webURL = value; }
		}

        [XmlAttribute("lastBuildDate")]
		public DateTime LastBuildDate
		{
			get { return lastBuildDate.DateTime; }
            set { lastBuildDate = new SerializableDateTime(value); }
		}

        [XmlAttribute("lastBuildLabel")]
		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
            set { lastBuildLabel = value; }
		}

        [XmlAttribute("lastSuccessfulBuildLabel")]
		public string LastSuccessfulBuildLabel
		{
			get { return lastSuccessfulBuildLabel; }
            set { lastSuccessfulBuildLabel = value; }
		}

        [XmlAttribute("nextBuildTime")]
		public DateTime NextBuildTime
		{
			get { return nextBuildTime.DateTime; }
            set { nextBuildTime = new SerializableDateTime(value); }
		}

        [XmlElement("message")]
		public Message[] Messages = new Message[0];
		
        [XmlIgnore]
		public string CurrentMessage
		{
			get
			{
				if (Messages.Length > 0)
					return Messages[Messages.Length-1].ToString();
				return string.Empty;
			}
		}
	}
}
