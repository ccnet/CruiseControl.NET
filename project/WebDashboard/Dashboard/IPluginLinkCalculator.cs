namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IPluginLinkCalculator
	{
		IAbsoluteLink[] GetBuildPluginLinks(IBuildSpecifier buildSpecifier);
		IAbsoluteLink[] GetProjectPluginLinks(IProjectSpecifier projectSpecifier);
	}
}
