using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
    /// <summary>
    /// Same pattern as a normal IAction, but request is already converted to a ICruiseRequest
    /// See CruiseActionProxyAction
    /// </summary>
	public interface ICruiseAction
	{
		IResponse Execute(ICruiseRequest cruiseRequest);
	}
}