using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
    /// <summary>
    /// 	
    /// </summary>
    public interface IIntegratable
    {
        /// <summary>
        /// Starts a new integration result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>
        /// The new <see cref="IIntegrationResult"/>.
        /// </returns>
        IIntegrationResult StartNewIntegration(IntegrationRequest request);

        /// <summary>
        /// Runs an integration of this project.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The result of the integration, or null if no integration took place.</returns>
        IIntegrationResult Integrate(IntegrationRequest request);
    }
}