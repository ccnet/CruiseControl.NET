using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public interface IViewAllBuildsViewBuilder
	{
		Control BuildView();
	}
}
