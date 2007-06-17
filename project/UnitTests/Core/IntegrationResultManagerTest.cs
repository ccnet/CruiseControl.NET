using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class IntegrationResultManagerTest : IntegrationFixture
	{
		private IMock mockStateManager;
		private Project project;
		private IntegrationResultManager manager;

		[SetUp]
		public void SetUp()
		{
			mockStateManager = new DynamicMock(typeof(IStateManager));

			project = CreateProject();
			manager = new IntegrationResultManager(project);
		}

		[TearDown]
		public void Verify()
		{
			mockStateManager.Verify();
		}

		[Test]
		public void StartNewIntegrationShouldCreateNewIntegrationResultAndProperlyPopulate()
		{
			ExpectToLoadState(IntegrationResultMother.CreateSuccessful("success"));

			IIntegrationResult result = manager.StartNewIntegration(ForceBuildRequest());
			Assert.AreEqual("project", result.ProjectName);
			Assert.AreEqual(@"c:\temp", result.WorkingDirectory);
			Assert.AreEqual(BuildCondition.ForceBuild, result.BuildCondition);
			Assert.AreEqual(IntegrationResult.InitialLabel, result.Label);
			Assert.AreEqual(project.ArtifactDirectory, result.ArtifactDirectory);
			Assert.AreEqual(project.WebURL, result.ProjectUrl);
			Assert.AreEqual("success", result.LastSuccessfulIntegrationLabel);
            Assert.AreEqual(Source, result.IntegrationRequest.Source);
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

			IIntegrationResult expected = manager.StartNewIntegration(ModificationExistRequest());
			Assert.AreEqual(lastResult, manager.LastIntegrationResult);

			mockStateManager.Expect("SaveState", expected);
			manager.FinishIntegration();
			Assert.AreEqual(expected, manager.LastIntegrationResult);
		}

	    [Test]
	    public void InitialBuildShouldBeForced()
	    {
            mockStateManager.ExpectAndReturn("HasPreviousState", false, "project");

            IIntegrationResult expected = manager.StartNewIntegration(ModificationExistRequest());
	        Assert.AreEqual(BuildCondition.ForceBuild, expected.BuildCondition);
        }

		private void ExpectToLoadState(IIntegrationResult result)
		{
			mockStateManager.ExpectAndReturn("HasPreviousState", true, "project");
			mockStateManager.ExpectAndReturn("LoadState", result, "project");
		}

		private Project CreateProject()
		{	
			project = new Project();
			project.Name = "project";
			project.ConfiguredWorkingDirectory = @"c:\temp";
			project.StateManager = (IStateManager) mockStateManager.MockInstance;
			project.ConfiguredArtifactDirectory = project.ConfiguredWorkingDirectory;
			return project;
		}
	}
}
