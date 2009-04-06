using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ICruiseServerManagerFactory
	{
		ICruiseServerManager Create(BuildServer buildServer);
	}
}
