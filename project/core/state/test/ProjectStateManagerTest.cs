using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.State;

namespace ThoughtWorks.CruiseControl.Core.state.test
{
	[TestFixture]
	public class ProjectStateManagerTest : Assertion
	{
		private DynamicMock projectMock;
		private DynamicMock slaveManagerMock;

		[SetUp]
		public void Setup()
		{
			projectMock = new DynamicMock(typeof(IProject));
			slaveManagerMock = new DynamicMock(typeof(IFileStateManager));
		}

		private void VerifyAll()
		{
			projectMock.Verify()	;
			slaveManagerMock.Verify();
		}

		[Test]
		public void SetsUpSlaveStateManagerUsingCamelCasedNameOfProjectWheneverStateFileExistsIsCalled()
		{
			projectMock.ExpectAndReturn("Name","my project");
			slaveManagerMock.Expect("Filename", "MyProject.state");
			slaveManagerMock.ExpectAndReturn("StateFileExists", true);

			ProjectStateManager projectStateManager = new ProjectStateManager((IProject) projectMock.MockInstance, 
				(IFileStateManager) slaveManagerMock.MockInstance);
			projectStateManager.StateFileExists();

			VerifyAll();
		}

		[Test]
		public void SetsUpSlaveStateManagerUsingNameOfProjectWheneverLoadStateIsCalled()
		{
			projectMock.ExpectAndReturn("Name","my project");
			slaveManagerMock.Expect("Filename","MyProject.state");
			slaveManagerMock.ExpectAndReturn("LoadState", new IntegrationResult());

			ProjectStateManager projectStateManager = new ProjectStateManager((IProject) projectMock.MockInstance, 
				(IFileStateManager) slaveManagerMock.MockInstance);
			projectStateManager.LoadState();

			VerifyAll();
		}

		[Test]
		public void SetsUpSlaveStateManagerUsingNameOfProjectWheneverSaveStateIsCalled()
		{
			projectMock.ExpectAndReturn("Name","my project");
			slaveManagerMock.Expect("Filename", "MyProject.state");
			IntegrationResult result = new IntegrationResult();
			slaveManagerMock.Expect("SaveState", result);

			ProjectStateManager projectStateManager = new ProjectStateManager((IProject) projectMock.MockInstance, 
				(IFileStateManager) slaveManagerMock.MockInstance);
			projectStateManager.SaveState(result);

			VerifyAll();
		}
	}
}
