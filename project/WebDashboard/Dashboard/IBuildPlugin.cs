using ThoughtWorks.CruiseControl.Core.Reporting.Dashboard.Navigation;

namespace ThoughtWorks.CruiseControl.WebDashboard.Dashboard
{
	public interface IBuildPlugin : IPlugin
	{
		bool IsDisplayedForProject(IProjectSpecifier project);
	}
}
