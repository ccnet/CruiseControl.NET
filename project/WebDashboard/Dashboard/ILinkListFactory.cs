namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ILinkListFactory
	{
		IAbsoluteLink[] CreateStyledBuildLinkList(IBuildSpecifier[] buildSpecifiers, IActionSpecifier actionSpecifier);
	}
}
