using System;

namespace tw.ccnet.remote
{
	/// <remarks>
	/// Remote Interface to CruiseControl.NET
	/// </remarks>
	public interface ICruiseManager
	{
		/// <summary>
		/// Starts a stopped instance of CruiseControl.NET
		/// </summary>
		void StartCruiseControl();

		/// <summary>
		/// Attempts to stop CruiseControl.NET after it completes its current task.
		/// </summary>
		void StopCruiseControl();

		/// <summary>
		/// Stops CruiseControl.NET immediately
		/// </summary>
		void StopCruiseControlNow();

		/// <summary>
		/// Gets the current status of the CruiseControl.NET server.
		/// </summary>
		CruiseControlStatus GetStatus();

		/// <summary>
		/// Gets information about the last build status, current activity and project name.
		/// </summary>
		ProjectStatus GetProjectStatus();

		void Run(string project, ISchedule schedule);

        string Configuration { get; set; }
	}

	/// <remarks>
	/// Status of cruise control status
	/// </remarks>
	public enum CruiseControlStatus
	{
		Stopped,
		Running,
		WillBeStopped,
		Unknown
	}

	public enum ProjectActivity 
	{
		CheckingModifications,
		Building,
		Sleeping,
		Unknown
	}

	[Serializable]
	public struct ProjectStatus
	{
		private CruiseControlStatus status;
		private IntegrationStatus buildStatus;
		private ProjectActivity activity;
		private string name;
		private string webURL;
		private DateTime lastBuildDate;
		private string lastBuildLabel;

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

		public CruiseControlStatus Status 
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

		public string WebURL
		{
			get { return webURL; }
		}

		public DateTime LastBuildDate
		{
			get { return lastBuildDate; }
		}

		public string LastBuildLabel
		{
			get { return lastBuildLabel; }
		}
	}

	public enum IntegrationStatus
	{
		Success,
		Failure,
		Exception,
		Unknown
	}
}
