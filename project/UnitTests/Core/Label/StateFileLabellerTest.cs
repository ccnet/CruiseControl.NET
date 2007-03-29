using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.State;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Label
{
	[TestFixture]
	public class StateFileLabellerTest : IntegrationFixture
	{
		private StateFileLabeller labeller;
		private IMock mockStateManager;

		[SetUp]
		public void SetUp()
		{
			mockStateManager = new DynamicMock(typeof (IStateManager));
			labeller = new StateFileLabeller((IStateManager) mockStateManager.MockInstance);
		}

		[Test]
		public void ShouldLoadIntegrationResultFromStateManagerAndReturnLastSuccessfulBuildLabel()
		{
			mockStateManager.ExpectAndReturn("LoadState", SuccessfulResult("success"), "Project1");
			labeller.Project = "Project1";

			Assert.AreEqual("success", labeller.Generate(new IntegrationResult()));
			mockStateManager.Verify();
		}
	}
}