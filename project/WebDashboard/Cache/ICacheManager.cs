
namespace ThoughtWorks.CruiseControl.WebDashboard.Cache
{
	public interface ICacheManager
	{
		void AddContent(string serverName, string projectName, string directory, string fileName, string content);
		string GetURLForFile(string serverName, string projectName, string directory, string fileName);
	}
}
