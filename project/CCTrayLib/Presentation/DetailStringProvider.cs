using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

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

			if (projectStatus.Activity.IsSleeping())
			{
				if (projectStatus.NextBuildTime == DateTime.MaxValue)
					return "Project is not automatically triggered";
				
				return string.Format("Next build check: {0:T}", projectStatus.NextBuildTime);
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