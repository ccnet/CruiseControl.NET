using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultBuildNameFormatter : IBuildNameFormatter
	{
		public string GetPrettyBuildName(IBuildSpecifier buildSpecifier)
		{
			LogFile logFile = new LogFile(buildSpecifier.BuildName);
			return string.Format("{0} ({1})", logFile.FormattedDateString, logFile.Succeeded ? logFile.Label : "Failed");	
		}

		public string GetCssClassForBuildLink(IBuildSpecifier buildSpecifier)
		{
			return new LogFile(buildSpecifier.BuildName).Succeeded ? "build-passed-link" : "build-failed-link";
		}
	}
}
