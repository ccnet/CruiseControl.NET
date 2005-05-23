using ThoughtWorks.CruiseControl.WebDashboard.IO;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectGridAction
	{
		IView Execute(string actionName, IRequest request);
		IView Execute(string actionName, IServerSpecifier serverSpecifer, IRequest request);
	}
}
