using System.Web.UI;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	// Same pattern as a normal IAction, but request is already converted to a ICruiseRequest
	// See CruiseActionProxyAction
	public interface ICruiseAction
	{
		Control Execute(ICruiseRequest cruiseRequest);
	}
}
