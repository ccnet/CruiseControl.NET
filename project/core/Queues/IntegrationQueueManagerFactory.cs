using System;
using ThoughtWorks.CruiseControl.Core.State;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
    /// <summary>
    /// Factory class for generating IntegrationQueueManager instances.
    /// </summary>
    /// <remarks>
    /// The static <see cref="CreateManager"/> method will generate an instance of 
    /// <see cref="IntegrationQueueManager"/> using <see cref="IntegrationQueueManagerFactory"/> 
    /// by default. If a different queue manager is required (e.g. in unit testing) use the 
    /// static method <see cref="OverrideFactory"/> to change the default factory.
    /// </remarks>
    public class IntegrationQueueManagerFactory :
        IQueueManagerFactory
    {
        private static IQueueManagerFactory managerFactory = new IntegrationQueueManagerFactory();

        /// <summary>
        /// Creates an instance of a queue manager.
        /// </summary>
        /// <param name="projectIntegratorListFactory">The integrator factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="stateManager">The state manager to use.</param>
        /// <returns>The new queue manager.</returns>
        public virtual IQueueManager Create(IProjectIntegratorListFactory projectIntegratorListFactory,
            IConfiguration configuration,
            IProjectStateManager stateManager)
        {
            IQueueManager integrationQueueManager = new IntegrationQueueManager(projectIntegratorListFactory, configuration, stateManager);
            return integrationQueueManager;
        }

        /// <summary>
        /// Changes the default manager factory.
        /// </summary>
        /// <param name="newFactory">The new factory to use.</param>
        public static void OverrideFactory(IQueueManagerFactory newFactory)
        {
            managerFactory = newFactory;
        }

        /// <summary>
        /// Changes back to the default factory/
        /// </summary>
        public static void ResetFactory()
        {
            managerFactory = new IntegrationQueueManagerFactory();
        }

        /// <summary>
        /// Generates a queue manager using the default factory.
        /// </summary>
        /// <param name="projectIntegratorListFactory">The integrator factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="stateManager">The state manager to use.</param>
        /// <returns>The new queue manager.</returns>
        public static IQueueManager CreateManager(IProjectIntegratorListFactory projectIntegratorListFactory,
            IConfiguration configuration,
            IProjectStateManager stateManager)
        {
            return managerFactory.Create(projectIntegratorListFactory, configuration, stateManager);
        }
    }
}
