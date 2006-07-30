using System;
using System.IO;
using System.Reflection;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Tasks;
using ThoughtWorks.CruiseControl.Core.Triggers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core
{
	[TestFixture]
	public class ProjectTest : IntegrationFixture
	{
		private Project project;
		private IMock mockBuilder;
		private IMock mockSourceControl;
		private IMock mockStateManager;
		private IMock mockTrigger;
		private IMock mockLabeller;
		private IMock mockPublisher;
		private IMock mockTask;
		private string workingDirPath;
		private string artifactDirPath;
		private const string ProjectName = "test";
		private Mockery mockery;

		[SetUp]
		public void SetUp()
		{
			workingDirPath = TempFileUtil.CreateTempDir("workingDir");
			artifactDirPath = TempFileUtil.CreateTempDir("artifactDir");
			Assert.IsTrue(Directory.Exists(workingDirPath));
			Assert.IsTrue(Directory.Exists(artifactDirPath));

			mockery = new Mockery();
			mockBuilder = mockery.NewStrictMock(typeof (ITask));
			mockSourceControl = mockery.NewStrictMock(typeof (ISourceControl));
			mockStateManager = mockery.NewStrictMock(typeof (IStateManager));
			mockTrigger = mockery.NewStrictMock(typeof (ITrigger));
			mockLabeller = mockery.NewStrictMock(typeof (ILabeller));
			mockPublisher = mockery.NewStrictMock((typeof (ITask)));
			mockTask = mockery.NewStrictMock((typeof (ITask)));

			project = new Project();
			SetupProject();
		}

		private void SetupProject()
		{
			project.Name = ProjectName;
			project.Builder = (ITask) mockBuilder.MockInstance;
			project.SourceControl = (ISourceControl) mockSourceControl.MockInstance;
			project.StateManager = (IStateManager) mockStateManager.MockInstance;
			project.Triggers = (ITrigger) mockTrigger.MockInstance;
			project.Labeller = (ILabeller) mockLabeller.MockInstance;
			project.Publishers = new ITask[] {(ITask) mockPublisher.MockInstance};
			project.Tasks = new ITask[] {(ITask) mockTask.MockInstance};
			project.ConfiguredWorkingDirectory = workingDirPath;
			project.ConfiguredArtifactDirectory = artifactDirPath;
		}

		private void VerifyAll()
		{
			mockery.Verify();
		}

		[Test]
		public void ShouldCreateCollectionSerialiserWhenCollectionPropertyIsPassed()
		{
			DefaultSerialiserFactory factory = new DefaultSerialiserFactory();
			PropertyInfo property = typeof (Project).GetProperty("Triggers");
			ReflectorPropertyAttribute attribute = (ReflectorPropertyAttribute) property.GetCustomAttributes(false)[0];
			IXmlSerialiser serialiser = factory.Create(ReflectorMember.Create(property), attribute);
			Assert.AreEqual(typeof(XmlCollectionSerialiser), serialiser.GetType());
		}

		[Test]
		public void LoadFullySpecifiedProjectFromConfiguration()
		{
			string xml = @"
<project name=""foo"" webURL=""http://localhost/ccnet"" modificationDelaySeconds=""60"" publishExceptions=""true"">
	<workingDirectory>c:\my\working\directory</workingDirectory>
	<build type=""nant"" />
	<sourcecontrol type=""filesystem"">
		<repositoryRoot>C:\</repositoryRoot>
	</sourcecontrol>
	<labeller type=""defaultlabeller"" />
	<state type=""state"" />
	<triggers>
		<scheduleTrigger time=""23:30"" buildCondition=""ForceBuild"" />
	</triggers>
	<publishers>
		<xmllogger logDir=""C:\temp"" />
		<nullTask />
	</publishers>
	<prebuild>
		<nullTask />
	</prebuild>
	<tasks>
		<merge files="""" />
	</tasks>
	<externalLinks>
		<externalLink name=""My Report"" url=""url1"" />
		<externalLink name=""My Other Report"" url=""url2"" />
	</externalLinks>
</project>";

			project = (Project) NetReflector.Read(xml);
			Assert.AreEqual("foo", project.Name);
			Assert.AreEqual("http://localhost/ccnet", project.WebURL);
			Assert.AreEqual(60, project.ModificationDelaySeconds);
			Assert.AreEqual(true, project.PublishExceptions);
			Assert.IsTrue(project.Builder is NAntTask);
			Assert.IsTrue(project.SourceControl is FileSourceControl);
			Assert.IsTrue(project.Labeller is DefaultLabeller);
			Assert.IsTrue(project.StateManager is FileStateManager);
			Assert.AreEqual(1, ((MultipleTrigger)project.Triggers).Triggers.Length);
			Assert.AreEqual(typeof(ScheduleTrigger), ((MultipleTrigger)project.Triggers).Triggers[0].GetType());
			Assert.IsTrue(project.Publishers[0] is XmlLogPublisher);
			Assert.IsTrue(project.Publishers[1] is NullTask);
			Assert.IsTrue(project.Tasks[0] is MergeFilesTask);
			Assert.IsTrue(project.PrebuildTasks[0] is NullTask);
			Assert.AreEqual("My Other Report", project.ExternalLinks[1].Name);
			Assert.AreEqual(@"c:\my\working\directory", project.ConfiguredWorkingDirectory);
			VerifyAll();
		}

		[Test]
		public void LoadMinimalProjectXmlFromConfiguration()
		{
			string xml = @"
<project name=""foo"" />";

			project = (Project) NetReflector.Read(xml);
			Assert.AreEqual("foo", project.Name);
			Assert.AreEqual(Project.DefaultUrl(), project.WebURL);
			Assert.AreEqual(0, project.ModificationDelaySeconds); //TODO: is this the correct default?  should quiet period be turned off by default?  is this sourcecontrol specific?
			Assert.AreEqual(true, project.PublishExceptions);
			Assert.IsTrue(project.Builder is NullTask);
			Assert.IsTrue(project.SourceControl is NullSourceControl);
			Assert.IsTrue(project.Labeller is DefaultLabeller);
			Assert.AreEqual(typeof(MultipleTrigger), project.Triggers.GetType());
			Assert.AreEqual(typeof(IntervalTrigger), ((MultipleTrigger)project.Triggers).Triggers[0].GetType());
			Assert.AreEqual(1, project.Publishers.Length);
			Assert.IsTrue(project.Publishers[0] is XmlLogPublisher);
			Assert.AreEqual(0, project.Tasks.Length);
			Assert.AreEqual(0, project.ExternalLinks.Length);
			VerifyAll();
		}

		[Test]
		public void LoadMinimalProjectXmlWithAnEmptyTriggersBlock()
		{
			string xml = @"
<project name=""foo"">
	<triggers/>
</project>";

			project = (Project) NetReflector.Read(xml);
			Assert.AreEqual(typeof(MultipleTrigger), project.Triggers.GetType());
		}

		// test: verify correct args are passed to sourcecontrol?  should use date of last modification from last successful build IMO

		[Test]
		public void ShouldLoadLastStateIfIntegrationHasBeenRunPreviously()
		{
			IntegrationResult expected = new IntegrationResult();
			expected.Label = "previous";
			expected.Status = IntegrationStatus.Success;

			mockStateManager.ExpectAndReturn("LoadState", expected, ProjectName);

			Assert.AreEqual(expected, project.LastIntegrationResult);
			VerifyAll();
		}

		[Test]
		public void InitialActivityState()
		{
			Assert.AreEqual(ProjectActivity.Sleeping, project.CurrentActivity);
			VerifyAll();
		}

		[Test]
		public void ShouldCallSourceControlInitializeOnInitialize()
		{
			// Setup
			mockSourceControl.Expect("Initialize", project);

			// Execute
			project.Initialize();

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldNotCallSourceControlPurgeOrDeleteDirectoriesOnPurgeIfNoDeletesRequested()
		{
			// Setup
			mockSourceControl.ExpectNoCall("Purge", typeof (IProject));

			// Execute
			project.Purge(false, false, false);

			// Verify
			VerifyAll();
			Assert.IsTrue(Directory.Exists(workingDirPath));
			Assert.IsTrue(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldCallSourceControlPurgeIfRequested()
		{
			// Setup
			mockSourceControl.Expect("Purge", project);

			// Execute
			project.Purge(false, false, true);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallSourceControlPurgeAndDeleteDirectoriesIfRequested()
		{
			// Setup
			mockSourceControl.Expect("Purge", project);

			// Execute
			project.Purge(true, true, true);

			// Verify
			VerifyAll();
			Assert.IsFalse(Directory.Exists(workingDirPath));
			Assert.IsFalse(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldDeleteWorkingDirectoryOnPurgeIfRequested()
		{
			// Setup
			mockSourceControl.ExpectNoCall("Purge", typeof (IProject));

			// Execute
			project.Purge(true, false, false);

			// Verify
			VerifyAll();
			Assert.IsFalse(Directory.Exists(workingDirPath));
		}

		[Test]
		public void ShouldDeleteArtifactDirectoryOnPurgeIfRequested()
		{
			// Setup
			mockSourceControl.ExpectNoCall("Purge", typeof (IProject));

			// Execute
			project.Purge(false, true, false);

			// Verify
			VerifyAll();
			Assert.IsFalse(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldNotDeleteDirectoriesIfSourceControlFailsOnPurge()
		{
			// Setup
			mockSourceControl.ExpectAndThrow("Purge", new CruiseControlException(), project);

			// Execute
			try
			{
				project.Purge(true, true, true);
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
			mockSourceControl.ExpectNoCall("Purge", typeof (IProject));
			TempFileUtil.DeleteTempDir("workingDir");
			Assert.IsFalse(Directory.Exists(workingDirPath));

			// Execute
			project.Purge(true, false, false);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldHandleArtifactDirectoryNotExisting()
		{
			// Setup
			mockSourceControl.ExpectNoCall("Purge", typeof (IProject));
			TempFileUtil.DeleteTempDir("artifactDir");
			Assert.IsFalse(Directory.Exists(artifactDirPath));

			// Execute
			project.Purge(false, true, false);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallIntegratableWhenIntegrateCalled()
		{
			DynamicMock integratableMock = new DynamicMock(typeof (IIntegratable));
			project = new Project((IIntegratable) integratableMock.MockInstance);
			SetupProject();

			IIntegrationResult result = (IIntegrationResult) new DynamicMock(typeof (IIntegrationResult)).MockInstance;
			IntegrationRequest request = ForceBuildRequest();
			integratableMock.ExpectAndReturn("Integrate", result, request);
			Assert.AreEqual(result, project.Integrate(request));
			VerifyAll();
		}

		// TRANSLATE THESE TESTS TO RUN UNDER INTEGRATION RUNNER TESTS

		[Test]
		public void RunningFirstIntegrationShouldForceBuild()
		{
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName); // running the first integration (no state file)
			mockStateManager.Expect("SaveState", new IsAnything());
			mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			mockSourceControl.ExpectAndReturn("GetModifications", new Modification[0], new IsAnything(), new IsAnything()); // return no modifications found
			mockSourceControl.Expect("GetSource", new IsAnything());
			mockSourceControl.Expect("LabelSourceControl", new IsAnything());
			mockPublisher.Expect("Run", new IsAnything());
			mockTask.Expect("Run", new IsAnything());
			project.Builder = new MockBuilder(); // need to use mock builder in order to set properties on IntegrationResult
			project.ConfiguredWorkingDirectory = @"c:\temp";

			IIntegrationResult result = project.Integrate(ModificationExistRequest());

			Assert.AreEqual(ProjectName, result.ProjectName);
			Assert.AreEqual(null, result.ExceptionResult);
			Assert.AreEqual(ProjectActivity.Sleeping, project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			Assert.AreEqual(BuildCondition.ForceBuild, result.BuildCondition);
			Assert.AreEqual(@"c:\temp", result.WorkingDirectory);
			Assert.AreEqual(result, project.LastIntegrationResult);
			Assert.AreEqual("label", result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			Assert.AreEqual(MockBuilder.BUILDER_OUTPUT, result.TaskOutput, "no output is expected as builder is not called");
			Assert.IsTrue(result.EndTime >= result.StartTime);
			VerifyAll();
		}

		//TODO: question: should state be saved after a poll with no modifications and no build?? -- i think it should: implication for last build though
		[Test] 
		public void RunningIntegrationWithNoModificationsShouldNotBuildOrPublish()
		{
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResultMother.CreateSuccessful(), ProjectName);
			mockSourceControl.ExpectAndReturn("GetModifications", new Modification[0], new IsAnything(), new IsAnything()); // return no modifications found
			mockBuilder.ExpectNoCall("Run", typeof (IntegrationResult));
			mockPublisher.ExpectNoCall("Run", typeof (IntegrationResult));

			IIntegrationResult result = project.Integrate(ModificationExistRequest());

			Assert.AreEqual(ProjectName, result.ProjectName);
			Assert.AreEqual(null, result.ExceptionResult);
			Assert.AreEqual(ProjectActivity.Sleeping, project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Unknown, result.Status);
			Assert.IsNotNull(project.LastIntegrationResult);
			Assert.AreEqual(IntegrationResult.InitialLabel, result.Label);
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

			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName); // running the first integration (no state file)
			mockStateManager.Expect("SaveState", new IsAnything());
			mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			mockSourceControl.ExpectAndReturn("GetModifications", modifications, new IsAnything(), new IsAnything());
			mockSourceControl.Expect("LabelSourceControl", new IsAnything());
			mockSourceControl.Expect("GetSource", new IsAnything());
			mockPublisher.Expect("Run", new IsAnything());
			mockTask.Expect("Run", new IsAnything());

			project.Builder = new MockBuilder(); // need to use mock builder in order to set properties on IntegrationResult
			IIntegrationResult result = project.Integrate(ModificationExistRequest());

			Assert.AreEqual(ProjectName, result.ProjectName);
			Assert.AreEqual(ProjectActivity.Sleeping, project.CurrentActivity);
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
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName); // running the first integration (no state file)
			CruiseControlException expectedException = new CruiseControlException();
			mockSourceControl.ExpectAndThrow("GetModifications", expectedException, new IsAnything(), new IsAnything());
			mockPublisher.ExpectNoCall("Run", typeof (IntegrationResult));
			mockStateManager.ExpectNoCall("SaveState", typeof (IntegrationResult));

			project.PublishExceptions = false;
			IIntegrationResult result = project.Integrate(ModificationExistRequest());
			Assert.AreEqual(expectedException, result.ExceptionResult);
			VerifyAll();
		}

		[Test]
		public void ShouldPublishIntegrationResultsIfPublishExceptionsIsTrueAndSourceControlThrowsAnException()
		{
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName); // running the first integration (no state file)
			CruiseControlException expectedException = new CruiseControlException();
			mockSourceControl.ExpectAndThrow("GetModifications", expectedException, new IsAnything(), new IsAnything());
			mockPublisher.Expect("Run", new IsAnything());
			mockStateManager.Expect("SaveState", new IsAnything());

			project.PublishExceptions = true;
			IIntegrationResult result = project.Integrate(ModificationExistRequest());
			Assert.AreEqual(expectedException, result.ExceptionResult);
			VerifyAll();
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void RethrowExceptionIfLoadingStateFileThrowsException()
		{
			mockStateManager.ExpectAndThrow("LoadState", new CruiseControlException("expected exception"), ProjectName);

			project.Integrate(ModificationExistRequest());
			VerifyAll();
		}

		[Test]
		public void SourceControlLabelled()
		{
			project.Builder = new MockBuilder();
			mockLabeller.ExpectAndReturn("Generate", "1.2.1", new IsAnything());
			mockTask.Expect("Run", new IsAnything());
			mockSourceControl.ExpectAndReturn("GetModifications", CreateModifications(), new IsAnything(), new IsAnything());
			mockSourceControl.Expect("GetSource", new IsAnything());
			mockSourceControl.Expect("LabelSourceControl", new IsAnything());
			mockPublisher.Expect("Run", new IsAnything());
			IMock stateMock = new DynamicMock(typeof (IStateManager));
			stateMock.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName); // running the first integration (no state file)
			project.StateManager = (IStateManager) stateMock.MockInstance;

			IIntegrationResult results = project.Integrate(ModificationExistRequest());

			stateMock.Expect("SaveState", results);

			Assert.AreEqual(results, project.LastIntegrationResult, "new integration result has not been set to the last integration result");
			Assert.IsNotNull(results.EndTime);
			VerifyAll();
		}

		// Run Tasks

		[Test]
		public void ShouldRunBuilderFirst()
		{
			IntegrationResult result = new IntegrationResult();
			mockTask.Expect("Run", result);
			MockBuilder builder = new MockBuilder();
			project.Builder = builder;
			project.Run(result);
			Assert.IsTrue(builder.HasRun);
			AssertStartsWith(MockBuilder.BUILDER_OUTPUT, result.TaskOutput);
			VerifyAll();
		}

		[Test]
		public void ShouldNotRunBuilderIfItDoesNotExist()
		{
			IntegrationResult result = new IntegrationResult();
			mockTask.Expect("Run", result);
			project.Builder = null;
			project.Run(result);
			VerifyAll();
		}

		[Test]
		public void ShouldStopBuildIfTaskFails()
		{
			IntegrationResult result = IntegrationResultMother.CreateFailed();
			mockTask.Expect("Run", result);

			IMock secondTask = new DynamicMock(typeof (ITask));
			secondTask.ExpectNoCall("Run", typeof (IntegrationResult));

			project.Tasks = new ITask[] {(ITask) mockTask.MockInstance, (ITask) secondTask.MockInstance};
			project.Builder = null;
			project.Run(result);
			VerifyAll();
			secondTask.Verify();
		}

		private Modification[] CreateModifications()
		{
			Modification[] modifications = new Modification[3];
			for (int i = 0; i < modifications.Length; i++)
			{
				modifications[i] = new Modification();
				modifications[i].ModifiedTime = DateTime.Today.AddDays(-1);
			}
			return modifications;
		}

		[Test] // publishers will need to log their own exceptions
			public void IfPublisherThrowsExceptionShouldStillSaveState()
		{
			mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName); // running the first integration (no state file)
			mockStateManager.Expect("SaveState", new IsAnything());
			mockSourceControl.ExpectAndReturn("GetModifications", CreateModifications(), new IsAnything(), new IsAnything());
			mockSourceControl.Expect("GetSource", new IsAnything());
			mockSourceControl.Expect("LabelSourceControl", new IsAnything());
			mockTask.Expect("Run", new IsAnything());
			Exception expectedException = new CruiseControlException("expected exception");
			mockPublisher.ExpectAndThrow("Run", expectedException, new IsAnything());
			project.Builder = new MockBuilder();

			IIntegrationResult results = project.Integrate(ModificationExistRequest());

			// failure to save the integration result will register as a failed project
			Assert.AreEqual(results, project.LastIntegrationResult, "new integration result has not been set to the last integration result");
			Assert.IsNotNull(results.EndTime);
			VerifyAll();
		}

		[Test]
		public void TimedoutTaskShouldFailBuildIfPublishExceptionsIsTrue()
		{
			mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName); // running the first integration (no state file)
			mockStateManager.Expect("SaveState", new IsAnything());
			mockTask.ExpectAndThrow("Run", new CruiseControlException(), new IsAnything());
			mockSourceControl.ExpectAndReturn("GetModifications", CreateModifications(), new IsAnything(), new IsAnything());
			mockSourceControl.Expect("GetSource", new IsAnything());
			mockPublisher.Expect("Run", new IsAnything());

			project.Builder = null;
			project.PublishExceptions = true;
			project.Integrate(ForceBuildRequest());

			VerifyAll();
		}

		[Test]
		public void AddedMessageShouldBeIncludedInProjectStatus()
		{
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName);
			mockTrigger.ExpectAndReturn("NextBuild", DateTime.Now);

			Message message = new Message("foo");
			project.AddMessage(message);
			ProjectStatus status = project.CreateProjectStatus(new ProjectIntegrator(project));
			Assert.AreEqual(message, status.Messages[0]);
		}

		[Test]
		public void PrebuildShouldIncrementLabelAndRunPrebuildTasks()
		{
			IntegrationResult result = IntegrationResult.CreateInitialIntegrationResult(ProjectName, "c:\\root");
			mockStateManager.ExpectAndReturn("LoadState", IntegrationResult.CreateInitialIntegrationResult(ProjectName, @"c:\temp"), ProjectName);
			mockLabeller.ExpectAndReturn("Generate", "1.0", result);
			IMock mockPrebuildTask = mockery.NewStrictMock(typeof(ITask));
			mockPrebuildTask.Expect("Run", result);
			project.PrebuildTasks = new ITask[] { (ITask) mockPrebuildTask.MockInstance };
			project.Prebuild(result);
			Assert.AreEqual("1.0", result.Label);
		}
	}
}