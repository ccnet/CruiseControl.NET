namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildPluginLinkCalculator
	{
		IAbsoluteLink[] GetBuildPluginLinks(IBuildSpecifier buildSpecifier);
	}
}
