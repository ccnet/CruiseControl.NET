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
        private Message[] messages = new Message[0];
        private string queue;
        private int queuePriority;

        /// <summary>
        /// Initialises a new blank <see cref="ProjectStatus"/>.
        /// </summary>
		public ProjectStatus()
		{}

        /// <summary>
        /// Initialise a new populated <see cref="ProjectStatus"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="buildStatus"></param>
        /// <param name="lastBuildDate"></param>
		public ProjectStatus(string name, IntegrationStatus buildStatus, DateTime lastBuildDate)
		{
			this.name = name;
			this.buildStatus = buildStatus;
			this.lastBuildDate = new SerializableDateTime(lastBuildDate);
		}

        /// <summary>
        /// Initialise a new populated <see cref="ProjectStatus"/>.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="category"></param>
        /// <param name="activity"></param>
        /// <param name="buildStatus"></param>
        /// <param name="status"></param>
        /// <param name="webURL"></param>
        /// <param name="lastBuildDate"></param>
        /// <param name="lastBuildLabel"></param>
        /// <param name="lastSuccessfulBuildLabel"></param>
        /// <param name="nextBuildTime"></param>
        /// <param name="buildStage"></param>
        /// <param name="queue"></param>
        /// <param name="queuePriority"></param>
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

        /// <summary>
        /// The current stage of the build.
        /// </summary>
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

        /// <summary>
        /// The current status of the project.
        /// </summary>
        [XmlAttribute("status")]
		public ProjectIntegratorState Status
		{
			get { return status; }
            set { status = value; }
		}

        /// <summary>
        /// The current integration status.
        /// </summary>
        [XmlAttribute("buildStatus")]
		public IntegrationStatus BuildStatus
		{
			get { return buildStatus; }
            set { buildStatus = value; }
		}

        /// <summary>
        /// The current project activity.
        /// </summary>
        [XmlElement("activity")]
		public ProjectActivity Activity
		{
			get { return activity; }
            set { activity = value; }
		}

        /// <summary>
        /// The name of the project.
        /// </summary>
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

        /// <summary>
        /// The category of the project.
        /// </summary>
        [XmlAttribute("category")]
		public string Category
		{
			get { return category; }
            set { category = value; }
		}

        /// <summary>
        /// The name of the queue that the project belongs to.
        /// </summary>
        [XmlAttribute("queueName")]
        public string Queue
        {
            get { return this.queue; }
            set { this.queue = value; }
        }

        /// <summary>
        /// The project of this project within the queue.
        /// </summary>
        [XmlAttribute("queuePriority")]
        public int QueuePriority
        {
            get { return this.queuePriority; }
            set { this.queuePriority = value; }
        }

        /// <summary>
        /// The URL for viewing the project details.
        /// </summary>
        [XmlAttribute("url")]
        public string WebURL
		{
			get { return webURL; }
            set { webURL = value; }
		}

        /// <summary>
        /// The date the project last built.
        /// </summary>
        [XmlAttribute("lastBuildDate")]
		public DateTime LastBuildDate
		{
			get { return lastBuildDate.DateTime; }
            set { lastBuildDate = new SerializableDateTime(value); }
		}

        /// <summary>
        /// The label of the last build.
        /// </summary>
        [XmlAttribute("lastBuildLabel")]
		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
            set { lastBuildLabel = value; }
		}

        /// <summary>
        /// The label of the last successful build.
        /// </summary>
        [XmlAttribute("lastSuccessfulBuildLabel")]
		public string LastSuccessfulBuildLabel
		{
			get { return lastSuccessfulBuildLabel; }
            set { lastSuccessfulBuildLabel = value; }
		}

        /// <summary>
        /// The time the build will next be checked.
        /// </summary>
        [XmlAttribute("nextBuildTime")]
		public DateTime NextBuildTime
		{
			get { return nextBuildTime.DateTime; }
            set { nextBuildTime = new SerializableDateTime(value); }
		}

        /// <summary>
        /// Any messages for a build.
        /// </summary>
        [XmlElement("message")]
        public Message[] Messages
        {
            get { return messages ?? new Message[0]; }
            set { messages = value; }
        }
		
        /// <summary>
        /// The most current message in the build.
        /// </summary>
        [XmlIgnore]
		public string CurrentMessage
		{
			get
			{
				if (Messages.Length > 0) return Messages[Messages.Length-1].ToString();
				return string.Empty;
			}
		}
	}
}
