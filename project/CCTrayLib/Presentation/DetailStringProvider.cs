using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class DetailStringProvider : IDetailStringProvider
	{
		public string FormatDetailString(IProjectMonitor monitor)
		{
			if (monitor.ProjectState == ProjectState.NotConnected)
			{
				if (monitor.ConnectException == null)
					return "Connecting...";

				return string.Format("Error: {0}", monitor.ConnectException.Message);
			}

			ProjectStatus status = monitor.ProjectStatus;
			if (status.Activity == ProjectActivity.Sleeping)
			{
				return string.Format("Next build check: {0:T}", status.NextBuildTime);
			}
			return string.Empty;
		}
	}
}