namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ILinkFactory
	{
		IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string text, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateStyledBuildLink(IBuildSpecifier buildSpecifier, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, string text, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string text, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateFarmLink(string text, IActionSpecifier actionSpecifier);
	}
}