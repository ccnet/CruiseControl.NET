namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildLinkFactory
	{
		IAbsoluteLink CreateBuildLink(IBuildSpecifier buildSpecifier, string description, IActionSpecifier actionSpecifier);
	}
}