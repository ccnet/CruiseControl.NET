using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IRecentBuildsPanelViewBuilder
	{
		HtmlTable BuildRecentBuildsPanel(string serverName, string projectName);
	}
}
