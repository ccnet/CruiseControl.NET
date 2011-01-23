using System.Collections.Generic;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface ICruiseProjectManagerFactory
	{
		CCTrayProject[] GetProjectList(BuildServer server, bool newServer);
		ICruiseProjectManager Create(CCTrayProject project, IDictionary<BuildServer, ICruiseServerManager> list);
	}
}
