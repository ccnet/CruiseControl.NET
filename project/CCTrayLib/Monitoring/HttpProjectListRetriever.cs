using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpProjectListRetriever
	{
        private readonly CruiseServerClientBase manager;

        public HttpProjectListRetriever(CruiseServerClientBase manager)
		{
            this.manager = manager;
        }

		public CCTrayProject[] GetProjectList(BuildServer server)
		{
            ProjectStatus[] statuses = manager.GetProjectStatus();
            CCTrayProject[] projects = new CCTrayProject[statuses.Length];

            for (int i = 0; i < statuses.Length; i++)
            {
                ProjectStatus status = statuses[i];
                projects[i] = new CCTrayProject(server, status.Name);
            }

            return projects;
        }
	}
}