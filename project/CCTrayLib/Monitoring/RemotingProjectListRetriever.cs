using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class RemotingProjectListRetriever
	{
		private readonly ICruiseManagerFactory manager;

		public RemotingProjectListRetriever(ICruiseManagerFactory manager)
		{
			this.manager = manager;
		}

		public Project[] GetProjectList(BuildServer server)
		{
			ProjectStatus[] statuses = manager.GetCruiseManager(server.Url).GetProjectStatus();
			Project[] projects = new Project[statuses.Length];

			for (int i = 0; i < statuses.Length; i++)
			{
				ProjectStatus status = statuses[i];
				projects[i] = new Project(server, status.Name);
			}

			return projects;
		}
	}
}