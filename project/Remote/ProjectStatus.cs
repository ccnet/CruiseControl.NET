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
	/// particular project, hence is is marked <see cref="Serializable"/>.
	/// </remarks>
	[Serializable]
	public class ProjectStatus
	{
		private CruiseControlStatus status;
		private IntegrationStatus buildStatus;
		private ProjectActivity activity;
		private string name;
		private string webURL;
		private DateTime lastBuildDate;
		private string lastBuildLabel;

		public ProjectStatus() { }

		public ProjectStatus(
			CruiseControlStatus status, 
			IntegrationStatus buildStatus, 
			ProjectActivity activity, 
			string name, 
			string webURL,
			DateTime lastBuildDate,
			string lastBuildLabel
			) 
		{
			this.status = status;
			this.buildStatus = buildStatus;
			this.activity = activity;
			this.name = name;
			this.webURL = webURL;
			this.lastBuildDate = lastBuildDate;
			this.lastBuildLabel = lastBuildLabel;
		}

		/// <summary>
		/// The state of the CruiseControl.NET server.
		/// </summary>
		public CruiseControlStatus Status 
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
			get { return lastBuildDate; }
			set { lastBuildDate = value; }
		}

		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
			set { lastBuildLabel = value; }
		}
	}
}
