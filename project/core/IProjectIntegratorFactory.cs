
using ThoughtWorks.CruiseControl.Core.Queues;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IProjectIntegratorListFactory
	{
		IProjectIntegratorList CreateProjectIntegrators(IProjectList projects, IntegrationQueueSet integrationQueues);
	}
}
