using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUserRequestSpecificSideBarViewBuilder
	{
		HtmlTable GetFarmSideBar();
		HtmlTable GetServerSideBar(string serverName);
		HtmlTable GetProjectSideBar(string serverName, string projectName);
		HtmlTable GetBuildSideBar(string serverName, string projectName, string buildName);
	}
}
