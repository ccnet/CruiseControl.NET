namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ILinkFactory
	{
		IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier);
		IAbsoluteLink CreateProjectLink(IProjectSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier);
	}
}