using System;

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
		private readonly SerializableDateTime nextBuildTime = SerializableDateTime.Default;
        private string currentBuildStage;

		public ProjectStatus()
		{}

		public ProjectStatus(string name, IntegrationStatus buildStatus, DateTime lastBuildDate)
		{
			this.name = name;
			this.buildStatus = buildStatus;
			this.lastBuildDate = new SerializableDateTime(lastBuildDate);
		}

        public ProjectStatus(string name, string category, ProjectActivity activity, IntegrationStatus buildStatus, ProjectIntegratorState status, string webURL, DateTime lastBuildDate, string lastBuildLabel, string lastSuccessfulBuildLabel, DateTime nextBuildTime, string buildStage)
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
		}

        public string BuildStage
        {
            get { return currentBuildStage; }
            set { currentBuildStage = value; }
        }                                         
                                             


		public ProjectIntegratorState Status
		{
			get { return status; }
		}

		public IntegrationStatus BuildStatus
		{
			get { return buildStatus; }
		}

		public ProjectActivity Activity
		{
			get { return activity; }
		}

		public string Name
		{
			get { return name; }
		}

		public string Category
		{
			get { return category; }
		}

		public string WebURL
		{
			get { return webURL; }
		}

		public DateTime LastBuildDate
		{
			get { return lastBuildDate.DateTime; }
		}

		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
		}

		public string LastSuccessfulBuildLabel
		{
			get { return lastSuccessfulBuildLabel; }
		}

		public DateTime NextBuildTime
		{
			get { return nextBuildTime.DateTime; }
		}

		public Message[] Messages = new Message[0];
		
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