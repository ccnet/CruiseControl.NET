using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IAllBuildsViewBuilder
	{
		HtmlTable BuildAllBuildsTable(string serverName, string projectName);	
	}
}
