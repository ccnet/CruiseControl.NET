using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class ViewAllBuildsAction : IAction
	{
		public Control Execute(IRequest request)
		{
			HtmlGenericControl control = new HtmlGenericControl("p");
			control.InnerText = "All Builds Will Appear Here";
			return control;
		}
	}
}
