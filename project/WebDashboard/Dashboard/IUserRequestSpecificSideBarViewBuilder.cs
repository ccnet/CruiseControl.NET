using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IUserRequestSpecificSideBarViewBuilder
	{
		HtmlTable GetFarmSideBar();
		HtmlTable GetServerSideBar(IServerSpecifier serverSpecifier);
		HtmlTable GetProjectSideBar(IProjectSpecifier projectSpecifier);
		HtmlTable GetBuildSideBar(IBuildSpecifier buildSpecifier);
	}
}
