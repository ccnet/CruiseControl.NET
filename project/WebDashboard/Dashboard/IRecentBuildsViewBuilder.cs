using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IRecentBuildsViewBuilder
	{
		HtmlTable BuildRecentBuildsTable(IProjectSpecifier projectSpecifier);
	}
}
