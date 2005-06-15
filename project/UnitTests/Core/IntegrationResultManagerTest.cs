using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class IntegrationResultManagerTest
	{
		private IMock mockLabeller;
		private IMock mockStateManager;
		private Project project;
		private IntegrationResultManager manager;

		[SetUp]
		public void SetUp()
		{
			mockLabeller = new DynamicMock(typeof(ILabeller));
			mockStateManager = new DynamicMock(typeof(IStateManager));

			project = CreateProject();
			manager = new IntegrationResultManager(project);
		}

		[TearDown]
		public void Verify()
		{
			mockStateManager.Verify();
			mockLabeller.Verify();
		}

		[Test]
		public void StartNewIntegrationShouldCreateNewIntegrationResultAndProperlyPopulate()
		{
			mockLabeller.ExpectAndReturn("Generate", "foo", new IsAnything());
			ExpectToLoadState(IntegrationResultMother.CreateSuccessful("success"));

			IIntegrationResult result = manager.StartNewIntegration(BuildCondition.ForceBuild);
			Assert.AreEqual("project", result.ProjectName);
			Assert.AreEqual(@"c:\temp", result.WorkingDirectory);
			Assert.AreEqual(BuildCondition.ForceBuild, result.BuildCondition);
			Assert.AreEqual("foo", result.Label);
			Assert.AreEqual(project.ArtifactDirectory, result.ArtifactDirectory);
			Assert.AreEqual(project.WebURL, result.ProjectUrl);
			Assert.AreEqual("success", result.LastSuccessfulIntegrationLabel);
		}

		[Test]
		public void LastIntegrationResultShouldBeLoadedOnlyOnceFromStateManager()
		{
			IntegrationResult expected = new IntegrationResult();
			ExpectToLoadState(expected);

			IIntegrationResult actual = manager.LastIntegrationResult;
			Assert.AreEqual(expected, actual);

			// re-request should not reload integration result
			actual = manager.LastIntegrationResult;
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void SavingCurrentIntegrationShouldSetItToLastIntegrationResult()
		{
			IIntegrationResult lastResult = new IntegrationResult();
			ExpectToLoadState(lastResult);

			IIntegrationResult expected = manager.StartNewIntegration(BuildCondition.IfModificationExists);
			Assert.AreEqual(lastResult, manager.LastIntegrationResult);

			mockStateManager.Expect("SaveState", expected);
			manager.FinishIntegration();
			Assert.AreEqual(expected, manager.LastIntegrationResult);
		}

		private void ExpectToLoadState(IIntegrationResult result)
		{
			mockStateManager.ExpectAndReturn("LoadState", result, "project");
		}

		private Project CreateProject()
		{	
			Project project = new Project();
			project.Name = "project";
			project.ConfiguredWorkingDirectory = @"c:\temp";
			project.Labeller = (ILabeller) mockLabeller.MockInstance;
			project.StateManager = (IStateManager) mockStateManager.MockInstance;
			project.ConfiguredArtifactDirectory = project.ConfiguredWorkingDirectory;
			return project;
		}
	}
}
