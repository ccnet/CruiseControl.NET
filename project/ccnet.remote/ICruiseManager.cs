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
		/// Attempts to stop CruiseControl.NET after it completes it's current task.
		/// </summary>
		void StopCruiseControl();

		/// <summary>
		/// Stops CruiseControl.NET immediatly
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

	[Serializable]
	public struct ProjectStatus
	{
		private CruiseControlStatus status;
		private IntegrationStatus buildStatus;
		private string activity;
		private string name;

		public ProjectStatus(CruiseControlStatus status, IntegrationStatus buildStatus, string activity, string name) 
		{
			this.status = status;
			this.buildStatus = buildStatus;
			this.activity = activity;
			this.name = name;
		}

		public CruiseControlStatus Status 
		{
			get { return status; }
		}

		public IntegrationStatus BuildStatus 
		{
			get { return buildStatus; }
		}

		public string Activity 
		{
			get { return activity; }
		}

		public string Name 
		{
			get { return name; }
		}
	}

	public enum IntegrationStatus
	{
		Success,
		Failure,
		Unknown
	}
}
