namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildRetriever
	{
		Build GetBuild(IBuildSpecifier buildSpecifier);
	}
}
