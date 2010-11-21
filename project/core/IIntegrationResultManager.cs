using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
	public interface IIntegrationResultManager
	{
        /// <summary>
        /// Gets the last integration result.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		IIntegrationResult LastIntegrationResult { get; }
        /// <summary>
        /// Gets the current integration.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		IIntegrationResult CurrentIntegration { get; }
        /// <summary>
        /// Gets the last integration.	
        /// </summary>
        /// <value></value>
        /// <remarks></remarks>
		IntegrationSummary LastIntegration { get; }

        /// <summary>
        /// Starts the new integration.	
        /// </summary>
        /// <param name="buildCondition">The build condition.</param>
        /// <returns></returns>
        /// <remarks></remarks>
		IIntegrationResult StartNewIntegration(IntegrationRequest buildCondition);
        /// <summary>
        /// Finishes the integration.	
        /// </summary>
        /// <remarks></remarks>
		void FinishIntegration();
	}
}