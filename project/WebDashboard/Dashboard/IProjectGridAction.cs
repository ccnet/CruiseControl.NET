using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IProjectGridAction
	{
		IView Execute(string[] actionArguments, string actionName, IRequest request);
		IView Execute(string[] actionArguments, string actionName, IServerSpecifier serverSpecifer, IRequest request);
	}
}
