
namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IRecentBuildsViewBuilder
	{
		string BuildRecentBuildsTable(IProjectSpecifier projectSpecifier);
	}
}
