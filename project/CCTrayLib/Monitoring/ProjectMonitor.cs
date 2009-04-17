using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class ProjectMonitor : IProjectMonitor, ISingleProjectDetail
	{
		private readonly ICruiseProjectManager cruiseProjectManager;
        private readonly ISingleServerMonitor serverMonitor;
		private ProjectStatus lastProjectStatus;
		private Exception connectException;
		private readonly BuildDurationTracker buildDurationTracker;
        // CCNET-1179: Store the project configuration.
        private readonly CCTrayProject _configuration;

        // CCNET-1179: Include the configuration in the arguments.
        public ProjectMonitor(CCTrayProject configuration, ICruiseProjectManager cruiseProjectManager, ISingleServerMonitor serverMonitor)
            : this(configuration, cruiseProjectManager, serverMonitor, new DateTimeProvider())
		{
		}

        // CCNET-1179: Include the configuration in the arguments.
        public ProjectMonitor(CCTrayProject configuration, ICruiseProjectManager cruiseProjectManager, ISingleServerMonitor serverMonitor, DateTimeProvider dateTimeProvider)
		{
			buildDurationTracker = new BuildDurationTracker(dateTimeProvider);
			this.cruiseProjectManager = cruiseProjectManager;
            this.serverMonitor = serverMonitor;
            this._configuration = configuration;
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
			get
			{
				if (IsConnected)
				{
					return lastProjectStatus.Activity;
				}
				return new ProjectActivity("");
			}
		}

		public string LastBuildLabel
		{
			get
			{
				if (IsConnected)
				{
					return lastProjectStatus.LastBuildLabel;
				}
				return string.Empty;
			}
		}

        public string ServerName
        {
            get
            {
                if (IsConnected)
                {
                    return lastProjectStatus.ServerName;
                }
                return string.Empty;
            }
        }

		public DateTime LastBuildTime
		{
			get
			{
				if (IsConnected)
				{
					return lastProjectStatus.LastBuildDate;
				}
				return DateTime.MinValue;
			}
		}

		public DateTime NextBuildTime
		{
			get
			{
				if (IsConnected)
				{
					return lastProjectStatus.NextBuildTime;
				}
				return DateTime.MinValue;
			}
		}

		public string WebURL
		{
			get
			{
				if (IsConnected)
				{
					return lastProjectStatus.WebURL;
				}
				return string.Empty;
			}
		}

        public string CurrentBuildStage
        {
            get
			{
				if (IsConnected)
				{
					return lastProjectStatus.BuildStage;
				}
				return string.Empty;
			}
        }                                                                                                            

		public string CurrentMessage
		{
			get
			{
				if (IsConnected)
				{
					return lastProjectStatus.CurrentMessage;
				}
				return string.Empty;
			}
		}

		public string ProjectName
		{
			get { return cruiseProjectManager.ProjectName; }
		}

        /// <summary>
        /// Retrieve the configuration for this project.
        /// </summary>
        /// <remarks>
        /// This is part of the fix for CCNET-1179.
        /// </remarks>
        public CCTrayProject Configuration
        {
            get { return this._configuration; }
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
		
		public string ProjectIntegratorState
		{
            get
            {
                if (lastProjectStatus == null)
                {
                    return "Unknown";
                }
                else
                {
                    return lastProjectStatus.Status.ToString();
                }
            }
		}
		
		public bool IsPending
		{
			get { return ProjectStatus != null && ProjectStatus.Activity.IsPending(); }
		}

		public ISingleProjectDetail Detail
		{
			get { return this; }
		}

        private void AttemptActionWithRetry(ActionHandler actionToTry)
        {
            try
            {
                actionToTry();
            }
            catch (SessionInvalidException)
            {
                // Let's assume the session has expired, so login again
                if (this.serverMonitor.RefreshSession())
                {
                    // Now retry the action again
                    actionToTry();
                }
            }
        }

        private delegate void ActionHandler();

		public void ForceBuild()
		{
            AttemptActionWithRetry(delegate()
            {
                cruiseProjectManager.ForceBuild(serverMonitor.SessionToken);
            });
		}
		
		public void AbortBuild()
		{
            AttemptActionWithRetry(delegate()
            {
                cruiseProjectManager.AbortBuild(serverMonitor.SessionToken);
            });
		}
		
		public void FixBuild(string fixingUserName)
		{
            AttemptActionWithRetry(delegate()
            {
                cruiseProjectManager.FixBuild(serverMonitor.SessionToken, fixingUserName);
            });
		}

		public void StopProject()
		{
            AttemptActionWithRetry(delegate()
            {
                cruiseProjectManager.StopProject(serverMonitor.SessionToken);
            });
		}
		
		public void StartProject()
		{
            AttemptActionWithRetry(delegate()
            {
                cruiseProjectManager.StartProject(serverMonitor.SessionToken);
            });
		}
		
		public void CancelPending()
		{
            AttemptActionWithRetry(delegate()
            {
                cruiseProjectManager.CancelPendingRequest(serverMonitor.SessionToken);
            });
		}

		public void OnPollStarting()
		{
			// No initialisation required.
		}

		public void Poll()
		{
			try
			{
                ProjectStatus newProjectStatus = serverMonitor.GetProjectStatus(ProjectName);
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

        #region RetrieveSnapshot()
        /// <summary>
        /// Retrieves a snapshot of the current build status.
        /// </summary>
        /// <returns>The current build status of the project.</returns>
        public virtual ProjectStatusSnapshot RetrieveSnapshot()
        {
            ProjectStatusSnapshot snapshot = cruiseProjectManager.RetrieveSnapshot();
            return snapshot;
        }
        #endregion

        #region RetrievePackageList()
        /// <summary>
        /// Retrieves the current list of available packages.
        /// </summary>
        /// <returns></returns>
        public virtual PackageDetails[] RetrievePackageList()
        {
            PackageDetails[] list = cruiseProjectManager.RetrievePackageList();
            return list;
        }
        #endregion

        #region RetrieveFileTransfer()
        /// <summary>
        /// Retrieve a file transfer object.
        /// </summary>
        /// <param name="project">The project to retrieve the file for.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="source">Where to retrieve the file from.</param>
        public virtual IFileTransfer RetrieveFileTransfer(string fileName, FileTransferSource source)
        {
            IFileTransfer fileTransfer = cruiseProjectManager.RetrieveFileTransfer(fileName, source);
            return fileTransfer;
        }
        #endregion
	}

	public delegate void MessageEventHandler(Message message);
}
