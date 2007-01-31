using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class PollIntervalReporter
	{
		private readonly ProjectStatus lastProjectStatus;
		private readonly ProjectStatus newProjectStatus;

		public PollIntervalReporter(ProjectStatus lastProjectStatus, ProjectStatus newProjectStatus)
		{
			this.lastProjectStatus = lastProjectStatus;
			this.newProjectStatus = newProjectStatus;
		}

		public bool HasNewBuildStarted
		{
			get
			{
				bool isStartDetectedFromStatusChange = !lastProjectStatus.Activity.IsBuilding() &&
				                                       newProjectStatus.Activity.IsBuilding();
				bool isStartDetectedFromDateChange = lastProjectStatus.Activity.IsBuilding() &&
				                                     newProjectStatus.Activity.IsBuilding()
				                                     && (lastProjectStatus.LastBuildDate != newProjectStatus.LastBuildDate);

				return isStartDetectedFromStatusChange || isStartDetectedFromDateChange;
			}
		}

		public bool WasNewStatusMessagesReceived
		{
			get { return lastProjectStatus.Messages.Length < newProjectStatus.Messages.Length; }
		}

		public Message LatestStatusMessage
		{
			get
			{
				if (newProjectStatus.Messages.Length == 0) return new Message(string.Empty);
				return newProjectStatus.Messages[newProjectStatus.Messages.Length - 1];
			}
		}

		public bool IsAnotherBuildComplete
		{
			get { return lastProjectStatus.LastBuildDate != newProjectStatus.LastBuildDate; }
		}

		public bool WasLatestBuildSuccessful
		{
			get { return IntegrationStatus.Success == newProjectStatus.BuildStatus; }
		}

		public BuildTransition BuildTransition
		{
			get
			{
				bool wasOk = lastProjectStatus.BuildStatus == IntegrationStatus.Success;
				bool isOk = newProjectStatus.BuildStatus == IntegrationStatus.Success;

				if (wasOk && isOk)
					return CCTrayLib.BuildTransition.StillSuccessful;
				else if (!wasOk && !isOk)
					return CCTrayLib.BuildTransition.StillFailing;
				else if (wasOk && !isOk)
					return CCTrayLib.BuildTransition.Broken;
				else if (!wasOk && isOk)
					return CCTrayLib.BuildTransition.Fixed;

				throw new Exception("The universe has gone crazy.");
			}
		}
	}
}