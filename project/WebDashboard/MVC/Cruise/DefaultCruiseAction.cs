using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class DefaultCruiseAction : IAction
	{
		public Control Execute(IRequest request)
		{
			HtmlGenericControl control = new HtmlGenericControl("p");
			control.InnerText = "To Do - Default Cruise Action";
			return control;
		}
	}
}
