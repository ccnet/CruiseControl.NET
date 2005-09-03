using System;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class ProjectMonitor : IProjectMonitor
	{
		ICruiseProjectManager cruiseProjectManager;
		ProjectStatus lastProjectStatus;
		Exception connectException;

		public ProjectMonitor( ICruiseProjectManager cruiseProjectManager )
		{
			this.cruiseProjectManager = cruiseProjectManager;
		}

		public ProjectStatus ProjectStatus
		{
			get { return lastProjectStatus; }
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

				if (status  == null)
					return ProjectState.NotConnected;

				if (status.Activity == ProjectActivity.Building)
					return ProjectState.Building;

				if (status.BuildStatus == IntegrationStatus.Success)
					return ProjectState.Success;

				return ProjectState.Broken;
			}
		}

		public void ForceBuild()
		{
			cruiseProjectManager.ForceBuild();
		}

		public void Poll()
		{
			try
			{
				ProjectStatus newProjectStatus = cruiseProjectManager.ProjectStatus;

				if (lastProjectStatus != null && newProjectStatus != null)
				{
					if (lastProjectStatus.LastBuildDate != newProjectStatus.LastBuildDate)
					{
						BuildTransition transition = CalculateBuildTransition(lastProjectStatus, newProjectStatus);
						OnBuildOccurred(new MonitorBuildOccurredEventArgs(this,transition));
					}
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


		protected void OnBuildOccurred(MonitorBuildOccurredEventArgs args)
		{
			if (BuildOccurred != null)
				BuildOccurred(this, args);
		}

		protected void OnPolled(MonitorPolledEventArgs args)
		{
			if (Polled != null)
				Polled(this, args); 
		}

		private BuildTransition CalculateBuildTransition(ProjectStatus oldStatus, ProjectStatus newStatus)
		{
			bool wasOk = oldStatus.BuildStatus == IntegrationStatus.Success;
			bool isOk = newStatus.BuildStatus == IntegrationStatus.Success;

			if (wasOk && isOk)
				return BuildTransition.StillSuccessful;
			else if (!wasOk && !isOk)
				return BuildTransition.StillFailing;
			else if (wasOk && !isOk)
				return BuildTransition.Broken;
			else if (!wasOk && isOk)
				return BuildTransition.Fixed;

			throw new Exception ("The universe has gone crazy.");
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
	}
}
