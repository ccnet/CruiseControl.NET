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

		public IIntegrationResult Integration(string project, string workingDirectory)
		{
			return new IntegrationResult(project, workingDirectory, ModificationExistRequest());
		}
	}
}