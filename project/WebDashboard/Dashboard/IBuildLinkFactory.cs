namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildLinkFactory
	{
		IAbsoluteLink CreateBuildLink(string serverName, string projectName, string buildName, string description, IActionSpecifier actionSpecifier);
	}
}