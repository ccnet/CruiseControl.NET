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

			if (projectStatus.Activity == ProjectActivity.Sleeping)
			{
				return string.Format("Next build check: {0:T}", projectStatus.NextBuildTime);
			}
			return string.Empty;
		}
	}
}