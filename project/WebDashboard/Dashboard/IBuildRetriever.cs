namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildRetriever
	{
		Build GetBuild();
		Build GetPreviousBuild(Build build);
		Build GetNextBuild(Build build);
	}
}
