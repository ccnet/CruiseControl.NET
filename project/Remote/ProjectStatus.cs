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
		private string webURL;
		private SerializableDateTime lastBuildDate = SerializableDateTime.Default;
		private string lastBuildLabel;
		private string lastSuccessfulBuildLabel;
		private readonly SerializableDateTime nextBuildTime = SerializableDateTime.Default;

		public ProjectStatus()
		{}

		public ProjectStatus(ProjectIntegratorState status, IntegrationStatus buildStatus, ProjectActivity activity, string name, string webURL, DateTime lastBuildDate, string lastBuildLabel, string lastSuccessfulBuildLabel, DateTime nextBuildTime)
		{
			this.status = status;
			this.buildStatus = buildStatus;
			this.activity = activity;
			this.name = name;
			this.webURL = webURL;
			this.lastBuildDate = new SerializableDateTime(lastBuildDate);
			this.lastBuildLabel = lastBuildLabel;
			this.lastSuccessfulBuildLabel = lastSuccessfulBuildLabel;
			this.nextBuildTime = new SerializableDateTime(nextBuildTime);
		}

		public ProjectIntegratorState Status
		{
			get { return status; }
			set { status = value; }
		}

		public IntegrationStatus BuildStatus
		{
			get { return buildStatus; }
			set { buildStatus = value; }
		}

		public ProjectActivity Activity
		{
			get { return activity; }
			set { activity = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string WebURL
		{
			get { return webURL; }
			set { webURL = value; }
		}

		public DateTime LastBuildDate
		{
			get { return lastBuildDate.DateTime; }
			set { lastBuildDate = new SerializableDateTime(value); }
		}

		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
			set { lastBuildLabel = value; }
		}

		public string LastSuccessfulBuildLabel
		{
			get { return lastSuccessfulBuildLabel; }
			set { lastSuccessfulBuildLabel = value; }
		}

		public DateTime NextBuildTime
		{
			get { return nextBuildTime.DateTime; }
		}
	}
}