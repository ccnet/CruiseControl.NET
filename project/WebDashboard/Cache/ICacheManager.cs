using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.Cache
{
	public interface ICacheManager
	{
		void AddContent(IProjectSpecifier projectSpecifier, string directory, string fileName, string content);
		string GetContent(IProjectSpecifier projectSpecifier, string directory, string fileName);
		string GetURLForFile(IProjectSpecifier projectSpecifier, string directory, string fileName);
	}
}
