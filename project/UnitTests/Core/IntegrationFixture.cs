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
	}
}