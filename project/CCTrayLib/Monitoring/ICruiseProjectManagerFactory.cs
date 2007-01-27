using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ICruiseProjectManagerFactory
	{
		ICruiseProjectManager Create(CCTrayProject project);
		CCTrayProject[] GetProjectList(BuildServer server);
	}
}