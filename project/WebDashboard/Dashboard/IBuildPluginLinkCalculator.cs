namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildPluginLinkCalculator
	{
		IAbsoluteLink[] GetBuildPluginLinks(string serverName, string projectName, string buildName);
	}
}
