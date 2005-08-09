using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IAllBuildsViewBuilder
	{
		IResponse GenerateAllBuildsView(IProjectSpecifier projectSpecifier);	
	}
}
