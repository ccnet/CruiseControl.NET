using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class UnknownActionAction : IAction
	{
		public Control Execute(IRequest request)
		{
			HtmlGenericControl control = new HtmlGenericControl("p");

			string actionName = request.FindParameterStartingWith(CruiseActionFactory.ACTION_PARAMETER_PREFIX);
			if (actionName == "")
			{
				control.InnerText = "Internal Error - 'UnknownActionAction' called but there is no action is request!";
			}
			else
			{
				actionName = actionName.Substring(CruiseActionFactory.ACTION_PARAMETER_PREFIX.Length);	
				control.InnerText = "Unknown action requested - " + actionName;
			}
			
			return control;
		}
	}
}
