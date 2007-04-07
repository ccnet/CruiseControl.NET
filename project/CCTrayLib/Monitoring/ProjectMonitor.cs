using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class ProjectMonitor : IProjectMonitor, ISingleProjectDetail
	{
		private ICruiseProjectManager cruiseProjectManager;
		private ProjectStatus lastProjectStatus;
		private Exception connectException;
		private BuildDurationTracker buildDurationTracker;

		public ProjectMonitor(ICruiseProjectManager cruiseProjectManager)
			: this(cruiseProjectManager, new DateTimeProvider())
		{
		}
		
		public ProjectMonitor(ICruiseProjectManager cruiseProjectManager, DateTimeProvider dateTimeProvider)
		{
			buildDurationTracker = new BuildDurationTracker(dateTimeProvider);
			this.cruiseProjectManager = cruiseProjectManager;			
		}

		// public for testing only
		public ProjectStatus ProjectStatus
		{
			get { return lastProjectStatus; }
		}

		public bool IsConnected
		{
			get { return lastProjectStatus != null; }
		}

		public ProjectActivity Activity
		{
			get { return lastProjectStatus.Activity; }
		}

		public string LastBuildLabel
		{
			get { return lastProjectStatus.LastBuildLabel; }
		}

		public DateTime LastBuildTime
		{
			get { return lastProjectStatus.LastBuildDate; }
		}

		public DateTime NextBuildTime
		{
			get { return lastProjectStatus.NextBuildTime; }
		}

		public string WebURL
		{
			get { return lastProjectStatus.WebURL; }
		}

		public string CurrentMessage
		{
			get { return lastProjectStatus.CurrentMessage; }
		}

		public string ProjectName
		{
			get { return cruiseProjectManager.ProjectName; }
		}

		public Exception ConnectException
		{
			get { return connectException; }
		}

		public ProjectState ProjectState
		{
			get
			{
				// nb: deliberately copy project status variable for thread safety
				ProjectStatus status = ProjectStatus;

				if (status == null)
					return ProjectState.NotConnected;

				if (status.Activity.IsBuilding())
					return (status.BuildStatus == IntegrationStatus.Success) ? ProjectState.Building : ProjectState.BrokenAndBuilding;

				if (status.BuildStatus == IntegrationStatus.Success)
					return ProjectState.Success;

				return ProjectState.Broken;
			}
		}

		public IntegrationStatus IntegrationStatus
		{
			get
			{
				if (lastProjectStatus == null)
					return IntegrationStatus.Unknown;
				return lastProjectStatus.BuildStatus;
			}
		}

		public bool IsPending
		{
			get { return ProjectStatus.Activity.IsPending(); }
		}

		public ISingleProjectDetail Detail
		{
			get { return this; }
		}

		public void ForceBuild()
		{
			cruiseProjectManager.ForceBuild();
		}

		public void FixBuild()
		{
			cruiseProjectManager.FixBuild();
		}

		public void CancelPending()
		{
			cruiseProjectManager.CancelPendingRequest();
		}

		public void OnPollStarting()
		{
			// No initialisation required.
		}

		public void Poll()
		{
			try
			{
				ProjectStatus newProjectStatus = cruiseProjectManager.ProjectStatus;
				if (lastProjectStatus != null && newProjectStatus != null)
				{
					PollIntervalReporter duringInterval = new PollIntervalReporter(lastProjectStatus, newProjectStatus);
					
					if (duringInterval.IsAnotherBuildComplete && duringInterval.WasLatestBuildSuccessful) buildDurationTracker.OnSuccessfulBuild();
					if (duringInterval.IsAnotherBuildComplete) OnBuildOccurred(new MonitorBuildOccurredEventArgs(this, duringInterval.BuildTransition));

					if (duringInterval.HasNewBuildStarted) buildDurationTracker.OnBuildStart();

					if (duringInterval.WasNewStatusMessagesReceived) OnMessageReceived(duringInterval.LatestStatusMessage);
				}
				lastProjectStatus = newProjectStatus;
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Exception during poll: " + ex);
				lastProjectStatus = null;
				connectException = ex;
			}

			OnPolled(new MonitorPolledEventArgs(this));
		}

		public event MonitorBuildOccurredEventHandler BuildOccurred;
		public event MonitorPolledEventHandler Polled;
		public event MessageEventHandler MessageReceived;

		protected void OnBuildOccurred(MonitorBuildOccurredEventArgs args)
		{
			if (BuildOccurred != null) BuildOccurred(this, args);
		}

		protected void OnPolled(MonitorPolledEventArgs args)
		{
			if (Polled != null) Polled(this, args);
		}

		private void OnMessageReceived(Message message)
		{
			if (MessageReceived != null) MessageReceived(message);
		}

		public string SummaryStatusString
		{
			get
			{
				ProjectState state = ProjectState;

				if (state == ProjectState.Success)
					return String.Empty;

				return ProjectName + ": " + state;
			}
		}

		public TimeSpan EstimatedTimeRemainingOnCurrentBuild
		{
			get { return buildDurationTracker.EstimatedTimeRemainingOnCurrentBuild; }
		}
	}

	public delegate void MessageEventHandler(Message message);
}
