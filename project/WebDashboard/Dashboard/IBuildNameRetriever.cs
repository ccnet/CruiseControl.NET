using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildNameRetriever
	{
        IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier, string sessionToken);
        IBuildSpecifier GetNextBuildSpecifier(IBuildSpecifier buildSpecifier, string sessionToken);
        IBuildSpecifier GetPreviousBuildSpecifier(IBuildSpecifier buildSpecifier, string sessionToken);
	}
}
