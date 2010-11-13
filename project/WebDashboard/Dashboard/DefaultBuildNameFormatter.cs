using System;
using System.Globalization;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public class DefaultBuildNameFormatter : IBuildNameFormatter
	{
		public string GetPrettyBuildName(IBuildSpecifier buildSpecifier)
		{
			return GetPrettyBuildName(buildSpecifier, CultureInfo.CurrentCulture);	
		}

		public string GetPrettyBuildName(IBuildSpecifier buildSpecifier, IFormatProvider formatter)
		{
			LogFile logFile = new LogFile(buildSpecifier.BuildName, formatter);
			return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0} ({1})", logFile.FormattedDateString, logFile.Succeeded ? logFile.Label : "Failed");	
		}

		public string GetCssClassForBuildLink(IBuildSpecifier buildSpecifier)
		{
			return new LogFile(buildSpecifier.BuildName).Succeeded ? "build-passed-link" : "build-failed-link";
		}

		public string GetCssClassForSelectedBuildLink(IBuildSpecifier buildSpecifier)
		{
			return new LogFile(buildSpecifier.BuildName).Succeeded ? "selected build-passed-link" : "selected build-failed-link";
		}
	}
}
