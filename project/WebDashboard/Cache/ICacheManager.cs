
namespace ThoughtWorks.CruiseControl.WebDashboard.Cache
{
	public interface ICacheManager
	{
		void AddContent(string serverName, string projectName, string directory, string fileName, string content);
		string GetContent(string serverName, string projectName, string directory, string fileName);
		string GetURLForFile(string serverName, string projectName, string directory, string fileName);
	}
}
