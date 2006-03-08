namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegratable
	{
		/// <summary>
		/// Runs an integration of this project.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The result of the integration, or null if no integration took place.</returns>
		IIntegrationResult Integrate(IntegrationRequest request);
	}
}