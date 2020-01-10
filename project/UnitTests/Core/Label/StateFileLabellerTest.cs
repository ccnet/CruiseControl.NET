using Moq;
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
		private Mock<IStateManager> mockStateManager;

		[SetUp]
		public void SetUp()
		{
			mockStateManager = new Mock<IStateManager>();
			labeller = new StateFileLabeller((IStateManager) mockStateManager.Object);
		}

		[Test]
		public void ShouldLoadIntegrationResultFromStateManagerAndReturnLastSuccessfulBuildLabel()
		{
			mockStateManager.Setup(manager => manager.LoadState("Project1")).Returns(SuccessfulResult("success")).Verifiable();
			labeller.Project = "Project1";

			Assert.AreEqual("success", labeller.Generate(new IntegrationResult()));
			mockStateManager.Verify();
		}
	}
}