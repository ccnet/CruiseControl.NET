using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IAction
	{
		Control Execute(IRequest request);
	}
}
