using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public interface IErrorViewBuilder
	{
		Control BuildView(string errorMessage);
	}
}
