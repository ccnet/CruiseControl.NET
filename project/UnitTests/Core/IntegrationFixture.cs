using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	public class IntegrationFixture : CustomAssertion
	{
		protected IntegrationRequest ModificationExistRequest()
		{
			return Request(BuildCondition.IfModificationExists);
		}

		protected IntegrationRequest Request(BuildCondition buildCondition)
		{
			return new IntegrationRequest("foo", buildCondition);
		}
		
		protected IntegrationRequest ForceBuildRequest()
		{
			return Request(BuildCondition.ForceBuild);
		}
	}
}