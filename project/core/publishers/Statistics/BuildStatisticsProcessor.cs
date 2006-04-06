namespace ThoughtWorks.CruiseControl.Core.Publishers.Statistics
{
	public class BuildStatisticsProcessor
	{
		public virtual IntegrationStatistics ProcessBuildResults(IIntegrationResult integrationResult)
		{
			IntegrationStatistics integrationStatistics = new IntegrationStatistics();
			integrationStatistics.BuildLabel = integrationResult.Label;
			integrationStatistics.IntegrationStatus = integrationResult.Status;
			integrationStatistics.IntegrationTime = integrationResult.TotalIntegrationTime;
			integrationStatistics.ProjectName = integrationResult.ProjectName;
			return integrationStatistics;
		}
	}
}