using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IRecentBuildsViewBuilder
	{
        string BuildRecentBuildsTable(IProjectSpecifier projectSpecifier, string sessionToken);
        string BuildRecentBuildsTable(IBuildSpecifier buildSpecifier, string sessionToken);
	}
}
