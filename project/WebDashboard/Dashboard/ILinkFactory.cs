namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ILinkFactory
	{
		IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string text, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, string text, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string text, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateStyledBuildLink(IBuildSpecifier specifier, IActionSpecifier actionSpecifier);
	}
}