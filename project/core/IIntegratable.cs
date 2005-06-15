using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegratable
	{
		/// <summary>
		/// Runs an integration of this project.
		/// </summary>
		/// <param name="buildCondition"></param>
		/// <returns>The result of the integration, or null if no integration took place.</returns>
		IIntegrationResult RunIntegration(BuildCondition buildCondition);
	}
}