namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ILinkFactory
	{
		IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateProjectLink(IProjectSpecifier projectSpecifier, string description, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateServerLink(IServerSpecifier serverSpecifier, string description, IActionSpecifier actionSpecifier);
	}
}