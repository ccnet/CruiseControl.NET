using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class ProjectMonitor : IProjectMonitor
	{
		ICruiseProjectManager cruiseProjectManager;
		ProjectStatus lastProjectStatus;

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

		public void Poll()
		{
			ProjectStatus newProjectStatus = cruiseProjectManager.ProjectStatus;

			if (lastProjectStatus != null && newProjectStatus != null)
			{
				if (lastProjectStatus.LastBuildDate != newProjectStatus.LastBuildDate)
				{
					BuildTransition transition = CalculateBuildTransition(lastProjectStatus, newProjectStatus);
					OnBuildOccurred(new BuildOccurredEventArgs(newProjectStatus,transition));
				}
			}

			lastProjectStatus = newProjectStatus;

			OnPolled(new PolledEventArgs(ProjectStatus));
		}

		public event BuildOccurredEventHandler BuildOccurred;
		public event PolledEventHandler Polled;


		protected void OnBuildOccurred(BuildOccurredEventArgs args)
		{
			if (BuildOccurred != null)
				BuildOccurred(this, args);
		}

		protected void OnPolled(PolledEventArgs args)
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
	}
}
