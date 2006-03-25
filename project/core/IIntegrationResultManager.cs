namespace ThoughtWorks.CruiseControl.Core
{
	public interface IIntegrationResultManager
	{
		IIntegrationResult LastIntegrationResult { get; }

		IIntegrationResult StartNewIntegration(IntegrationRequest buildCondition);
		void FinishIntegration();
	}
}