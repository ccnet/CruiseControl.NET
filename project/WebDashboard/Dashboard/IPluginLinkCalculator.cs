namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IPluginLinkCalculator
	{
		IAbsoluteLink[] GetServerPluginLinks(IServerSpecifier serverSpecifier);
		IAbsoluteLink[] GetProjectPluginLinks(IProjectSpecifier projectSpecifier);
		IAbsoluteLink[] GetBuildPluginLinks(IBuildSpecifier buildSpecifier);
	}
}
