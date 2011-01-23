using ThoughtWorks.CruiseControl.Core.State;

namespace ThoughtWorks.CruiseControl.Core.Queues
{
    /// <summary>
    /// Factory class for generating queue managers.
    /// </summary>
    public interface IQueueManagerFactory
    {
        /// <summary>
        /// Creates an instance of a queue manager.
        /// </summary>
        /// <param name="projectIntegratorListFactory">The integrator factory.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="stateManager">The state manager to use.</param>
        /// <returns>The new queue manager.</returns>
        IQueueManager Create(IProjectIntegratorListFactory projectIntegratorListFactory,
            IConfiguration configuration,
            IProjectStateManager stateManager);
    }
}
