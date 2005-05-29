using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IRecentBuildsViewBuilder
	{
		string BuildRecentBuildsTable(IProjectSpecifier projectSpecifier);
	}
}
