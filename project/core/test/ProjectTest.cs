using System;
using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Tasks.Test;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class ProjectTest : CustomAssertion
	{
		private Project _project;
		private IMock _mockBuilder;
		private IMock _mockSourceControl;
		private IMock _mockStateManager;
		private IMock _mockIntegrationTrigger;
		private IMock _mockLabeller;
		private IMock _mockPublisher;
		private IMock _mockTask;
		private string workingDirPath;
		private string artifactDirPath;
		private TraceListenerBackup backup;
		private const string PROJECT_NAME = "test";

		[SetUp]
		public void SetUp()
		{
			workingDirPath = TempFileUtil.CreateTempDir("workingDir");
			artifactDirPath = TempFileUtil.CreateTempDir("artifactDir");
			Assert.IsTrue(Directory.Exists(workingDirPath));
			Assert.IsTrue(Directory.Exists(artifactDirPath));

			_mockBuilder = new DynamicMock(typeof (ITask));
			_mockBuilder.Strict = true;
			_mockSourceControl = new DynamicMock(typeof (ISourceControl));
			_mockSourceControl.Strict = true;
			_mockStateManager = new DynamicMock(typeof (IStateManager));
			_mockStateManager.Strict = true;
			_mockIntegrationTrigger = new DynamicMock(typeof (ITrigger));
			_mockIntegrationTrigger.Strict = true;
			_mockLabeller = new DynamicMock(typeof (ILabeller));
			_mockLabeller.Strict = true;
			_mockPublisher = new DynamicMock((typeof (PublisherBase)));
			_mockPublisher.Strict = true;
			_mockTask = new DynamicMock((typeof (ITask)));
			_mockTask.Strict = true;

			_project = new Project();
			SetupProject();

			backup = new TraceListenerBackup();
			backup.AddTestTraceListener();
		}

		private void SetupProject()
		{
			_project.Name = PROJECT_NAME;
			_project.Builder = (ITask) _mockBuilder.MockInstance;
			_project.SourceControl = (ISourceControl) _mockSourceControl.MockInstance;
			_project.StateManager = (IStateManager) _mockStateManager.MockInstance;
			_project.Triggers = new ITrigger[] {(ITrigger) _mockIntegrationTrigger.MockInstance};
			_project.Labeller = (ILabeller) _mockLabeller.MockInstance;
			_project.Publishers = new IIntegrationCompletedEventHandler[] {(IIntegrationCompletedEventHandler) _mockPublisher.MockInstance};
			_project.Tasks = new ITask[] {(ITask) _mockTask.MockInstance};
			_project.ConfiguredWorkingDirectory = workingDirPath;
			_project.ConfiguredArtifactDirectory = artifactDirPath;
		}

		private void VerifyAll()
		{
			_mockBuilder.Verify();
			_mockSourceControl.Verify();
			_mockStateManager.Verify();
			_mockIntegrationTrigger.Verify();
			_mockLabeller.Verify();
			_mockPublisher.Verify();
			_mockTask.Verify();
		}

		[TearDown]
		public void TearDown()
		{
			backup.Reset();

			TempFileUtil.DeleteTempDir("workingDir");
			TempFileUtil.DeleteTempDir("artifactDir");
		}

		[Test]
		public void LoadFullySpecifiedProjectFromConfiguration()
		{
			string xml = @"
<project name=""foo"" webURL=""http://localhost/ccnet"" modificationDelaySeconds=""60"" publishExceptions=""true"">
	<workingDirectory>c:\my\working\directory</workingDirectory>
	<build type=""nant"" />
	<sourcecontrol type=""mock"" />
	<labeller type=""defaultlabeller"" />
	<state type=""state"" />
	<triggers>
		<scheduleTrigger time=""23:30"" buildCondition=""ForceBuild"" />
	</triggers>
	<publishers>
		<xmllogger logDir=""C:\temp"" />
	</publishers>
	<tasks>
		<merge files="""" />
	</tasks>
	<externalLinks>
		<externalLink name=""My Report"" url=""url1"" />
		<externalLink name=""My Other Report"" url=""url2"" />
	</externalLinks>
</project>";

			Project project = (Project) NetReflector.Read(xml);
			Assert.AreEqual("foo", project.Name);
			Assert.AreEqual("http://localhost/ccnet", project.WebURL);
			Assert.AreEqual(60, project.ModificationDelaySeconds);
			Assert.AreEqual(true, project.PublishExceptions);
			Assert.IsTrue(project.Builder is NAntTask);
			Assert.IsTrue(project.SourceControl is MockSourceControl);
			Assert.IsTrue(project.Labeller is DefaultLabeller);
			Assert.IsTrue(project.StateManager is IntegrationStateManager);
			Assert.IsTrue(project.Triggers[0] is ScheduleTrigger);
			Assert.IsTrue(project.Publishers[0] is XmlLogPublisher);
			Assert.IsTrue(project.Tasks[0] is MergeFilesTask);
			Assert.AreEqual("My Other Report", project.ExternalLinks[1].Name );
			Assert.AreEqual(@"c:\my\working\directory", project.ConfiguredWorkingDirectory);

			VerifyAll();
		}

		[Test]
		public void LoadMinimalProjectXmlFromConfiguration()
		{
			string xml = @"
<project name=""foo"" />";

			Project project = (Project) NetReflector.Read(xml);
			Assert.AreEqual("foo", project.Name);
			Assert.AreEqual(Project.DEFAULT_WEB_URL, project.WebURL);
			Assert.AreEqual(0, project.ModificationDelaySeconds); //TODO: is this the correct default?  should quiet period be turned off by default?  is this sourcecontrol specific?
			Assert.AreEqual(true, project.PublishExceptions);
			Assert.IsTrue(project.Builder is NullTask);
			Assert.IsTrue(project.SourceControl is NullSourceControl);
			Assert.IsTrue(project.Labeller is DefaultLabeller);
			Assert.AreEqual(1, project.Triggers.Length);
			Assert.AreEqual(0, project.Publishers.Length);
			Assert.AreEqual(0, project.Tasks.Length);
			Assert.AreEqual(0, project.ExternalLinks.Length);
			VerifyAll();
		}

		[Test]
		public void asf()
		{
			string xml = @"
<project name=""foo"">
	<build type=""nant"" />
	<sourcecontrol type=""mock"" />
	<triggers/>
</project>";

			Project project = (Project) NetReflector.Read(xml);
			Assert.AreEqual(0, project.Triggers.Length);
		}

		// test: verify correct args are passed to sourcecontrol?  should use date of last modification from last successful build IMO

		[Test]
		public void ShouldCreateInitialIntegrationResultIfThisIsTheFirstIntegration()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false, null);
			Assert.IsTrue(_project.LastIntegrationResult.IsInitial());
			VerifyAll();
		}

		[Test]
		public void ShouldLoadLastStateIfIntegrationHasBeenRunPreviously()
		{
			IntegrationResult expected = new IntegrationResult();
			expected.Label = "previous";
			expected.Status = IntegrationStatus.Success;

			_mockStateManager.ExpectAndReturn("StateFileExists", true);
			_mockStateManager.ExpectAndReturn("LoadState", expected);

			Assert.AreEqual(expected, _project.LastIntegrationResult);
			VerifyAll();
		}

		[Test]
		public void InitialActivityState()
		{
			Assert.AreEqual(ProjectActivity.Sleeping, _project.CurrentActivity);
			VerifyAll();
		}

		[Test]
		public void ShouldCallSourceControlInitializeOnInitialize()
		{
			// Setup
			_mockSourceControl.Expect("Initialize", _project);

			// Execute
			_project.Initialize();

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldNotCallSourceControlPurgeOrDeleteDirectoriesOnPurgeIfNoDeletesRequested()
		{
			// Setup
			_mockSourceControl.ExpectNoCall("Purge", typeof (IProject));

			// Execute
			_project.Purge(false, false, false);

			// Verify
			VerifyAll();
			Assert.IsTrue(Directory.Exists(workingDirPath));
			Assert.IsTrue(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldCallSourceControlPurgeIfRequested()
		{
			// Setup
			_mockSourceControl.Expect("Purge", _project);

			// Execute
			_project.Purge(false, false, true);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallSourceControlPurgeAndDeleteDirectoriesIfRequested()
		{
			// Setup
			_mockSourceControl.Expect("Purge", _project);

			// Execute
			_project.Purge(true, true, true);

			// Verify
			VerifyAll();
			Assert.IsFalse(Directory.Exists(workingDirPath));
			Assert.IsFalse(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldDeleteWorkingDirectoryOnPurgeIfRequested()
		{
			// Setup
			_mockSourceControl.ExpectNoCall("Purge", typeof (IProject));

			// Execute
			_project.Purge(true, false, false);

			// Verify
			VerifyAll();
			Assert.IsFalse(Directory.Exists(workingDirPath));
		}

		[Test]
		public void ShouldDeleteArtifactDirectoryOnPurgeIfRequested()
		{
			// Setup
			_mockSourceControl.ExpectNoCall("Purge", typeof (IProject));

			// Execute
			_project.Purge(false, true, false);

			// Verify
			VerifyAll();
			Assert.IsFalse(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldNotDeleteDirectoriesIfSourceControlFailsOnPurge()
		{
			// Setup
			_mockSourceControl.ExpectAndThrow("Purge", new CruiseControlException(), _project);

			// Execute
			try
			{
				_project.Purge(true, true, true);
			}
			catch (CruiseControlException)
			{}

			// Verify
			VerifyAll();
			Assert.IsTrue(Directory.Exists(workingDirPath));
			Assert.IsTrue(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldHandleWorkingDirectoryNotExisting()
		{
			// Setup
			_mockSourceControl.ExpectNoCall("Purge", typeof (IProject));
			TempFileUtil.DeleteTempDir("workingDir");
			Assert.IsFalse(Directory.Exists(workingDirPath));

			// Execute
			_project.Purge(true, false, false);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldHandleArtifactDirectoryNotExisting()
		{
			// Setup
			_mockSourceControl.ExpectNoCall("Purge", typeof (IProject));
			TempFileUtil.DeleteTempDir("artifactDir");
			Assert.IsFalse(Directory.Exists(artifactDirPath));

			// Execute
			_project.Purge(false, true, false);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldBuildBuilderAndThenTasksWhenBuildCalled()
		{
			IIntegrationResult result = (IIntegrationResult) new DynamicMock(typeof(IIntegrationResult)).MockInstance;
			_mockBuilder.Expect("Run", result);
			_mockTask.Expect("Run", result);
			_project.Run(result);
			VerifyAll();
		}

		[Test]
		public void ShouldCallIntegratableWhenRunIntegrationCalled()
		{
			DynamicMock integratableMock = new DynamicMock(typeof(IIntegratable));
			_project = new Project((IIntegratable) integratableMock.MockInstance);
			SetupProject();

			IIntegrationResult result = (IIntegrationResult) new DynamicMock(typeof(IIntegrationResult)).MockInstance;
			integratableMock.ExpectAndReturn("RunIntegration", result, BuildCondition.ForceBuild);
			Assert.AreEqual(result, _project.RunIntegration(BuildCondition.ForceBuild));
			VerifyAll();
		}

		// TRANSLATE THESE TESTS TO RUN UNDER INTEGRATION RUNNER TESTS

		[Test]
		public void RunningFirstIntegrationShouldForceBuild()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false); // running the first integration (no state file)
			_mockStateManager.Expect("SaveState", new IsAnything());
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			_mockSourceControl.ExpectAndReturn("GetModifications", new Modification[0], new IsAnything(), new IsAnything()); // return no modifications found
			_mockSourceControl.Expect("GetSource", new IsAnything());
			_mockSourceControl.Expect("LabelSourceControl", "label", new IsAnything());
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything());
			_mockTask.Expect("Run", new IsAnything());
			_project.Builder = new MockBuilder(); // need to use mock builder in order to set properties on IntegrationResult
			_project.ConfiguredWorkingDirectory = @"c:\temp";

			IIntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			Assert.AreEqual(PROJECT_NAME, result.ProjectName);
			Assert.AreEqual(null, result.ExceptionResult);
			Assert.AreEqual(ProjectActivity.Sleeping, _project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			Assert.AreEqual(BuildCondition.ForceBuild, result.BuildCondition);
			Assert.AreEqual(@"c:\temp", result.WorkingDirectory);
			Assert.AreEqual(result, _project.LastIntegrationResult);
			Assert.AreEqual("label", result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			Assert.AreEqual(MockBuilder.BUILDER_OUTPUT, result.TaskOutput, "no output is expected as builder is not called");
			Assert.IsTrue(result.EndTime >= result.StartTime);
			VerifyAll();
		}

		[Test] //TODO: question: should state be saved after a poll with no modifications and no build?? -- i think it should: implication for last build though
		public void RunningIntegrationWithNoModificationsShouldNotBuildOrPublish()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", true); // running the first integration (no state file)
			_mockStateManager.ExpectAndReturn("LoadState", IntegrationResultMother.CreateSuccessful());
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			_mockSourceControl.ExpectAndReturn("GetModifications", new Modification[0], new IsAnything(), new IsAnything()); // return no modifications found
			_mockBuilder.ExpectNoCall("Run", typeof (IntegrationResult));
			_mockPublisher.ExpectNoCall("PublishIntegrationResults", typeof (IntegrationResult));

			IIntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			Assert.AreEqual(PROJECT_NAME, result.ProjectName);
			Assert.AreEqual(null, result.ExceptionResult);
			Assert.AreEqual(ProjectActivity.Sleeping, _project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Unknown, result.Status);
			Assert.IsNotNull(_project.LastIntegrationResult);
			Assert.AreEqual("label", result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			Assert.AreEqual(string.Empty, result.TaskOutput, "no output is expected as builder is not called");
			Assert.IsTrue(result.EndTime >= result.StartTime);
			VerifyAll();
		}

		[Test]
		public void RunningFirstIntegrationWithModificationsShouldBuildAndPublish()
		{
			Modification[] modifications = new Modification[1] {new Modification()};

			_mockStateManager.ExpectAndReturn("StateFileExists", false); // running the first integration (no state file)
			_mockStateManager.Expect("SaveState", new IsAnything());
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			_mockSourceControl.ExpectAndReturn("GetModifications", modifications, new IsAnything(), new IsAnything());
			_mockSourceControl.Expect("LabelSourceControl", "label", new IsAnything());
			_mockSourceControl.Expect("GetSource", new IsAnything());
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything());
			_mockTask.Expect("Run", new IsAnything());

			_project.Builder = new MockBuilder(); // need to use mock builder in order to set properties on IntegrationResult
			IIntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			
			Assert.AreEqual(PROJECT_NAME, result.ProjectName);
			Assert.AreEqual(ProjectActivity.Sleeping, _project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			Assert.AreEqual("label", result.Label);
			Assert.IsTrue(result.HasModifications());
			Assert.AreEqual(modifications, result.Modifications);
			Assert.IsTrue(result.EndTime >= result.StartTime);
			VerifyAll();
		}

		[Test]
		public void ShouldNotPublishIntegrationResultsIfPublishExceptionsIsFalseAndSourceControlThrowsAnException()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false); // running the first integration (no state file)
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			CruiseControlException expectedException = new CruiseControlException();
			_mockSourceControl.ExpectAndThrow("GetModifications", expectedException, new IsAnything(), new IsAnything());
			_mockPublisher.ExpectNoCall("PublishIntegrationResults", typeof (IntegrationResult));
			_mockStateManager.ExpectNoCall("SaveState", typeof (IntegrationResult));

			_project.PublishExceptions = false;
			IIntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);
			Assert.AreEqual(expectedException, result.ExceptionResult);
			VerifyAll();
		}

		[Test]
		public void ShouldPublishIntegrationResultsIfPublishExceptionsIsTrueAndSourceControlThrowsAnException()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false); // running the first integration (no state file)
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			CruiseControlException expectedException = new CruiseControlException();
			_mockSourceControl.ExpectAndThrow("GetModifications", expectedException, new IsAnything(), new IsAnything());
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything());
			_mockStateManager.Expect("SaveState", new IsAnything());

			_project.PublishExceptions = true;
			IIntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);
			Assert.AreEqual(expectedException, result.ExceptionResult);
			VerifyAll();
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void RethrowExceptionIfLoadingStateFileThrowsException()
		{
			_mockStateManager.ExpectAndThrow("StateFileExists", new CruiseControlException("expected exception"));

			_project.RunIntegration(BuildCondition.IfModificationExists);
			VerifyAll();
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void RethrowExceptionIfLabellerThrowsException()
		{
			Exception expectedException = new CruiseControlException("expected exception");
			_mockStateManager.ExpectAndReturn("StateFileExists", false);
			_mockLabeller.ExpectAndThrow("Generate", expectedException, new IsAnything());

			_project.RunIntegration(BuildCondition.IfModificationExists);
			VerifyAll();
		}

		[Test]
		public void SourceControlLabeled()
		{
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			_mockLabeller.ExpectAndReturn("Generate", "1.2.1", new IsAnything());
			_mockTask.Expect("Run", new IsAnything());
			integrationCompletedCalled = false;
			_project.IntegrationCompleted += new IntegrationCompletedEventHandler(Project_IntegrationCompleted);
			IMock stateMock = new DynamicMock(typeof (IStateManager));
			stateMock.ExpectAndReturn("StateFileExists", false);
			_project.StateManager = (IStateManager) stateMock.MockInstance;

			IIntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			stateMock.Expect("SaveState", results);

			Assert.AreEqual(results, _project.LastIntegrationResult, "new integration result has not been set to the last integration result");
			Assert.IsNotNull(results.EndTime);
			Assert.IsTrue(integrationCompletedCalled);
			Assert.AreEqual("1.2.1", ((MockSourceControl) _project.SourceControl).Label);
			VerifyAll();
		}

		private bool integrationCompletedCalled;
		private void Project_IntegrationCompleted(object source, IntegrationCompletedEventArgs e)
		{
			integrationCompletedCalled = true;
		}

		[Test] // publishers will need to log their own exceptions
		public void IfPublisherThrowsExceptionShouldStillSaveState()
		{
			_mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
			_mockStateManager.ExpectAndReturn("StateFileExists", false);
			_mockStateManager.Expect("SaveState", new IsAnything());
			_mockTask.Expect("Run", new IsAnything());
			Exception expectedException = new CruiseControlException("expected exception");
			_mockPublisher.ExpectAndThrow("PublishIntegrationResults", expectedException, new IsAnything());
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();

			IIntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			// failure to save the integration result will register as a failed project
			Assert.AreEqual(results, _project.LastIntegrationResult, "new integration result has not been set to the last integration result");
			Assert.IsNotNull(results.EndTime);
			VerifyAll();
		}
	}
}