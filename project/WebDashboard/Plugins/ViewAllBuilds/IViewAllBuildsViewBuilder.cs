using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.ViewAllBuilds
{
	public interface IViewAllBuildsViewBuilder
	{
		Control BuildView();
	}
}
