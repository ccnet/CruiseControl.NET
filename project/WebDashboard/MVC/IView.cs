using System.Web.UI;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IView
	{
		Control Control { get; }
	}
}
