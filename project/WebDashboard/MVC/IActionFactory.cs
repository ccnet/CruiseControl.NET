using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public interface IActionFactory
	{
		IAction Create(IRequest request);
	}
}
