using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationResultManager
	{
		IIntegrationResult LastIntegrationResult { get; }
		IIntegrationResult CurrentIntegration { get; }

		IntegrationSummary LastIntegration
		{
			get;
		}

		IIntegrationResult StartNewIntegration(IntegrationRequest buildCondition);
		void FinishIntegration();
	}
}