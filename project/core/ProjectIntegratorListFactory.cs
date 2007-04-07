using ThoughtWorks.CruiseControl.Core.Queues;

namespace ThoughtWorks.CruiseControl.Core
{
	public class ProjectIntegratorListFactory : IProjectIntegratorListFactory
	{
		public IProjectIntegratorList CreateProjectIntegrators(IProjectList projects, IntegrationQueueSet integrationQueues)
		{
			ProjectIntegratorList list = new ProjectIntegratorList();
			foreach (IProject project in projects)
			{
				list.Add(new ProjectIntegrator(project, integrationQueues[project.QueueName]));
			}
			return list;
		}
	}
}