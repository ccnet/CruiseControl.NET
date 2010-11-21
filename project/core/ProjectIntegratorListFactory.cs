
using ThoughtWorks.CruiseControl.Core.Queues;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public class ProjectIntegratorListFactory : IProjectIntegratorListFactory
	{
        /// <summary>
        /// Creates the project integrators.	
        /// </summary>
        /// <param name="projects">The projects.</param>
        /// <param name="integrationQueues">The integration queues.</param>
        /// <returns></returns>
        /// <remarks></remarks>
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