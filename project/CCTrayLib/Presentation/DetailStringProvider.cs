using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class DetailStringProvider : IDetailStringProvider
	{
		public string FormatDetailString(ISingleProjectDetail projectStatus)
		{
			if (projectStatus.ProjectState == ProjectState.NotConnected)
			{
				if (projectStatus.ConnectException == null)
					return "Connecting...";

				return string.Format("Error: {0}", projectStatus.ConnectException.Message);
			}

			string message = GetTimeRemainingMessage(projectStatus);
			if (projectStatus.CurrentMessage.Length > 0)
				message += " - " + projectStatus.CurrentMessage;

			return message;
		}

		private static string GetTimeRemainingMessage(ISingleProjectDetail projectStatus)
		{
			if (projectStatus.Activity.IsSleeping())
			{
				if (projectStatus.NextBuildTime == DateTime.MaxValue)
					return "Project is not automatically triggered";

                if (projectStatus.NextBuildTime.Date != DateTime.Now.Date)
                {
                    return string.Format("Next build check: {0:G}", projectStatus.NextBuildTime);
                }
                else
                {
                    return string.Format("Next build check: {0:T}", projectStatus.NextBuildTime);
                }
			}

			TimeSpan durationRemaining = projectStatus.EstimatedTimeRemainingOnCurrentBuild;

			if (durationRemaining != TimeSpan.MaxValue)
			{
				if (durationRemaining <= TimeSpan.Zero)
				{
					return string.Format("Taking {0} longer than last build", new CCTimeFormatter(durationRemaining.Negate()));
				}

				return string.Format("{0} estimated remaining", new CCTimeFormatter(durationRemaining));
			}
			return string.Empty;
		}
	}
}
