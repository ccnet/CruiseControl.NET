using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultBuildNameFormatter : IBuildNameFormatter
	{
		public string GetPrettyBuildName(string originalBuildName)
		{
			LogFile logFile = new LogFile(originalBuildName);
			return string.Format("{0} ({1})", logFile.FormattedDateString, logFile.Succeeded ? logFile.Label : "Failed");	
		}

		public string GetCssClassForBuildLink(string originalBuildName)
		{
			return new LogFile(originalBuildName).Succeeded ? "build-passed-link" : "build-failed-link";
		}
	}
}
