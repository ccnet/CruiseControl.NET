using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUserRequestSpecificSideBarViewBuilder
	{
		Control GetFarmSideBar();
		Control GetServerSideBar(string serverName);
		Control GetProjectSideBar(string serverName, string projectName);
		Control GetBuildSideBar(string serverName, string projectName, string buildName);
	}
}
