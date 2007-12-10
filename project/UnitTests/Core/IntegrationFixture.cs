using System;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class IntegrationFixture : CustomAssertion
	{
		public string Source = "foo";

		public IntegrationRequest ModificationExistRequest()
		{
			return Request(BuildCondition.IfModificationExists);
		}

		public IntegrationRequest Request(BuildCondition buildCondition)
		{
			return new IntegrationRequest(buildCondition, Source);
		}

		public IntegrationRequest ForceBuildRequest()
		{
			return Request(BuildCondition.ForceBuild);
		}

		public IIntegrationResult Integration(string project, string workingDirectory, string artifactDirectory)
		{
			return new IntegrationResult(project, workingDirectory, artifactDirectory, ModificationExistRequest(), IntegrationSummary.Initial);
		}

		public IntegrationResult SuccessfulResult(string previousLabel)
		{
			return IntegrationResultMother.Create(new IntegrationSummary(IntegrationStatus.Success, previousLabel, previousLabel, DateTime.MinValue));
		}

		public IntegrationResult FailedResult(string previousLabel)
		{
			return FailedResult(previousLabel, previousLabel);
		}

		public IntegrationResult FailedResult(string previousLabel, string lastSuccessfulLabel)
		{
			return IntegrationResultMother.Create(new IntegrationSummary(IntegrationStatus.Failure, previousLabel, lastSuccessfulLabel, DateTime.MinValue));
		}

		public IntegrationResult InitialIntegrationResult()
		{
			return IntegrationResultMother.CreateInitial();
		}
	}
}
