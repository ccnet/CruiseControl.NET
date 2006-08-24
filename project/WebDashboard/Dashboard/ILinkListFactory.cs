using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface ILinkListFactory
	{
		IAbsoluteLink[] CreateStyledBuildLinkList(IBuildSpecifier[] buildSpecifiers, string action);
		IAbsoluteLink[] CreateServerLinkList(IServerSpecifier[] serverSpecifiers, string action);
	}
}
