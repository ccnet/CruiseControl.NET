using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectGridAction
	{
		IResponse Execute(string actionName, IRequest request);
		IResponse Execute(string actionName, IServerSpecifier serverSpecifer, IRequest request);
	}
}
