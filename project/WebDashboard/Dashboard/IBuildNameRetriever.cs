namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildNameRetriever
	{
		IBuildSpecifier GetLatestBuildSpecifier(IProjectSpecifier projectSpecifier);
		IBuildSpecifier GetNextBuildSpecifier(IBuildSpecifier buildSpecifier);
		IBuildSpecifier GetPreviousBuildSpecifier(IBuildSpecifier buildSpecifier);
	}
}
