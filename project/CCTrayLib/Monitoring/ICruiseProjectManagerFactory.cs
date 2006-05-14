using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ICruiseProjectManagerFactory
	{
		ICruiseProjectManager Create(Project project);
		Project[] GetProjectList(BuildServer server);
	}
}