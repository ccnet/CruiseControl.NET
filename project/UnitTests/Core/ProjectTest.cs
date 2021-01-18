using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Exortech.NetReflector;
using Exortech.NetReflector.Util;
using Moq;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Queues;
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
        private MockRepository mocks;
		private Project project;
		private Mock<ISourceControl> mockSourceControl;
		private Mock<IStateManager> mockStateManager;
		private Mock<ITrigger> mockTrigger;
		private Mock<ILabeller> mockLabeller;
		private Mock<ITask> mockPublisher;
		private Mock<ITask> mockTask;
		private string workingDirPath;
		private string artifactDirPath;
		private const string ProjectName = "test";
		private Mockery mockery;
		private IntegrationQueue queue;

		[SetUp]
		public void SetUp()
		{
            this.mocks = new MockRepository(MockBehavior.Default);
            workingDirPath = TempFileUtil.CreateTempDir("workingDir");
			artifactDirPath = TempFileUtil.CreateTempDir("artifactDir");
			Assert.IsTrue(Directory.Exists(workingDirPath));
			Assert.IsTrue(Directory.Exists(artifactDirPath));
			queue = new IntegrationQueue("foo", new DefaultQueueConfiguration("foo"), null);
			mockery = new Mockery();
			mockSourceControl = mockery.NewStrictMock<ISourceControl>();
			mockStateManager = mockery.NewStrictMock<IStateManager>();
			mockTrigger = mockery.NewStrictMock<ITrigger>();
			mockLabeller = mockery.NewStrictMock<ILabeller>();
			mockPublisher = mockery.NewStrictMock<ITask>();
			mockTask = mockery.NewStrictMock<ITask>();

			project = new Project();
			SetupProject();
		}

		private void SetupProject()
		{
			project.Name = ProjectName;
			project.SourceControl = (ISourceControl) mockSourceControl.Object;
			project.StateManager = (IStateManager) mockStateManager.Object;
			project.Triggers = (ITrigger) mockTrigger.Object;
			project.Labeller = (ILabeller) mockLabeller.Object;
			project.Publishers = new ITask[] {new ThoughtWorks.CruiseControl.Core.Publishers.XmlLogPublisher(),  (ITask) mockPublisher.Object };
			project.Tasks = new ITask[] {(ITask) mockTask.Object };
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
<project name=""foo"" webURL=""http://localhost/ccnet"" modificationDelaySeconds=""60"" category=""category1"" queue=""queueName1"" queuePriority=""1"">
	<workingDirectory>c:\my\working\directory</workingDirectory>
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
		<merge />
	</tasks>
	<externalLinks>
		<externalLink name=""My Report"" url=""url1"" />
		<externalLink name=""My Other Report"" url=""url2"" />
	</externalLinks>
</project>";

			project = (Project) NetReflector.Read(xml);
			Assert.AreEqual("foo", project.Name);
			Assert.AreEqual("http://localhost/ccnet", project.WebURL);
			Assert.AreEqual("category1", project.Category);
			Assert.AreEqual("queueName1", project.QueueName);
			Assert.AreEqual(1, project.QueuePriority);
			Assert.AreEqual(60, project.ModificationDelaySeconds);
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
			Assert.IsTrue(project.SourceControl is NullSourceControl);
			Assert.IsTrue(project.Labeller is DefaultLabeller);
			Assert.AreEqual(typeof(MultipleTrigger), project.Triggers.GetType());
			Assert.AreEqual(1, project.Publishers.Length);
			Assert.IsTrue(project.Publishers[0] is XmlLogPublisher);
			Assert.AreEqual(1, project.Tasks.Length);
			Assert.AreEqual(typeof(NullTask), project.Tasks[0].GetType());
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

			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(true).Verifiable();
			mockStateManager.Setup(_manager => _manager.LoadState(ProjectName)).Returns(expected).Verifiable();

			Assert.AreEqual(expected, project.CurrentResult);
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
			mockSourceControl.Setup(sourceControl => sourceControl.Initialize(project)).Verifiable();

			// Execute
			project.Initialize();

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldNotCallSourceControlPurgeOrDeleteDirectoriesOnPurgeIfNoDeletesRequested()
		{
			// Execute
			project.Purge(false, false, false);

			// Verify
			mockSourceControl.Verify(sourceControl => sourceControl.Purge(It.IsAny<IProject>()), Times.Never);
			VerifyAll();
			Assert.IsTrue(Directory.Exists(workingDirPath));
			Assert.IsTrue(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldCallSourceControlPurgeIfRequested()
		{
			// Setup
			mockSourceControl.Setup(sourceControl => sourceControl.Purge(project)).Verifiable();

			// Execute
			project.Purge(false, false, true);

			// Verify
			VerifyAll();
		}

		[Test]
		public void ShouldCallSourceControlPurgeAndDeleteDirectoriesIfRequested()
		{
			// Setup
			mockSourceControl.Setup(sourceControl => sourceControl.Purge(project)).Verifiable();

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
			// Execute
			project.Purge(true, false, false);

			// Verify
			mockSourceControl.Verify(sourceControl => sourceControl.Purge(It.IsAny<IProject>()), Times.Never);
			VerifyAll();
			Assert.IsFalse(Directory.Exists(workingDirPath));
		}

		[Test]
		public void ShouldDeleteArtifactDirectoryOnPurgeIfRequested()
		{
			// Execute
			project.Purge(false, true, false);

			// Verify
			mockSourceControl.Verify(sourceControl => sourceControl.Purge(It.IsAny<IProject>()), Times.Never);
			VerifyAll();
			Assert.IsFalse(Directory.Exists(artifactDirPath));
		}

		[Test]
		public void ShouldNotDeleteDirectoriesIfSourceControlFailsOnPurge()
		{
			// Setup
			mockSourceControl.Setup(sourceControl => sourceControl.Purge(project)).Throws(new CruiseControlException()).Verifiable();

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
			TempFileUtil.DeleteTempDir("workingDir");
			Assert.IsFalse(Directory.Exists(workingDirPath));

			// Execute
			project.Purge(true, false, false);

			// Verify
			mockSourceControl.Verify(sourceControl => sourceControl.Purge(It.IsAny<IProject>()), Times.Never);
			VerifyAll();
		}

		[Test]
		public void ShouldHandleArtifactDirectoryNotExisting()
		{
			// Setup
			TempFileUtil.DeleteTempDir("artifactDir");
			Assert.IsFalse(Directory.Exists(artifactDirPath));

			// Execute
			project.Purge(false, true, false);

			// Verify
			mockSourceControl.Verify(sourceControl => sourceControl.Purge(It.IsAny<IProject>()), Times.Never);
			VerifyAll();
		}

		[Test]
		public void ShouldCallIntegratableWhenIntegrateCalled()
		{
			var integratableMock = new Mock<IIntegratable>();
			project = new Project((IIntegratable) integratableMock.Object);
			SetupProject();

            var resultMock = new Mock<IIntegrationResult>();
            resultMock.SetupGet(_result => _result.Status).Returns(IntegrationStatus.Unknown).Verifiable();
            resultMock.SetupGet(_result => _result.StartTime).Returns(DateTime.Now).Verifiable();
            resultMock.SetupGet(_result => _result.Succeeded).Returns(false).Verifiable();

            

			IIntegrationResult result = (IIntegrationResult)resultMock.Object;
			IntegrationRequest request = ForceBuildRequest();
			integratableMock.Setup(integratable => integratable.Integrate(request)).Returns(result).Verifiable();
			Assert.AreEqual(result, project.Integrate(request));
			VerifyAll();
		}

		// TRANSLATE THESE TESTS TO RUN UNDER INTEGRATION RUNNER TESTS

		[Test]
		public void RunningFirstIntegrationShouldForceBuild()
		{
			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable(); // running the first integration (no state file)
			mockStateManager.Setup(_manager => _manager.SaveState(It.IsAny<IIntegrationResult>())).Verifiable();
			mockLabeller.Setup(labeller => labeller.Generate(It.IsAny<IIntegrationResult>())).Returns("label").Verifiable(); // generate new label
			mockSourceControl.Setup(sourceControl => sourceControl.GetModifications(It.IsAny<IIntegrationResult>(), It.IsAny<IIntegrationResult>())).Returns(new Modification[0]).Verifiable(); // return no modifications found
			mockSourceControl.Setup(sourceControl => sourceControl.GetSource(It.IsAny<IIntegrationResult>())).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.LabelSourceControl(It.IsAny<IIntegrationResult>())).Verifiable();
			mockPublisher.Setup(publisher => publisher.Run(It.IsAny<IIntegrationResult>())).Verifiable();
			mockTask.Setup(task => task.Run(It.IsAny<IntegrationResult>())).Callback<IIntegrationResult>(r => r.AddTaskResult("success")).Verifiable();
			project.ConfiguredWorkingDirectory = Platform.IsWindows ? @"c:\temp" : @"/tmp";

			IIntegrationResult result = project.Integrate(ModificationExistRequest());

			Assert.AreEqual(ProjectName, result.ProjectName);
			Assert.AreEqual(null, result.ExceptionResult);
			Assert.AreEqual(ProjectActivity.Sleeping, project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			Assert.AreEqual(BuildCondition.ForceBuild, result.BuildCondition);
			Assert.AreEqual(Platform.IsWindows ? @"c:\temp" : @"/tmp", result.WorkingDirectory);
			Assert.AreEqual(result, project.CurrentResult);
			Assert.AreEqual("label", result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			Assert.IsTrue(result.EndTime >= result.StartTime);
			VerifyAll();
		}

		[Test] 
		public void RunningIntegrationWithNoModificationsShouldNotBuildOrPublish()
		{
			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(true).Verifiable();
			mockStateManager.Setup(_manager => _manager.LoadState(ProjectName)).Returns(IntegrationResultMother.CreateSuccessful()).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetModifications(It.IsAny<IIntegrationResult>(), It.IsAny<IIntegrationResult>())).Returns(new Modification[0]).Verifiable(); // return no modifications found

			IIntegrationResult result = project.Integrate(ModificationExistRequest());

			Assert.AreEqual(ProjectName, result.ProjectName);
			Assert.AreEqual(null, result.ExceptionResult);
			Assert.AreEqual(ProjectActivity.Sleeping, project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Unknown, result.Status);
			Assert.IsNotNull(project.CurrentResult);
			//Assert.AreEqual(IntegrationResult.InitialLabel, result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			Assert.AreEqual(string.Empty, result.TaskOutput, "no output is expected as builder is not called");
			//			Assert.IsTrue(result.EndTime >= result.StartTime);
			mockPublisher.Verify(publisher => publisher.Run(It.IsAny<IntegrationResult>()), Times.Never);
			VerifyAll();
		}

		[Test]
		public void RunningFirstIntegrationWithModificationsShouldBuildAndPublish()
		{
			Modification[] modifications = new Modification[1] {new Modification()};

			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable();
			mockStateManager.Setup(_manager => _manager.SaveState(It.IsAny<IIntegrationResult>())).Verifiable();
			mockLabeller.Setup(labeller => labeller.Generate(It.IsAny<IIntegrationResult>())).Returns("label").Verifiable(); // generate new label
			mockSourceControl.Setup(sourceControl => sourceControl.GetModifications(It.IsAny<IIntegrationResult>(), It.IsAny<IIntegrationResult>())).Returns(modifications).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.LabelSourceControl(It.IsAny<IIntegrationResult>())).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetSource(It.IsAny<IIntegrationResult>())).Verifiable();
			mockPublisher.Setup(publisher => publisher.Run(It.IsAny<IIntegrationResult>())).Verifiable();
			mockTask.Setup(task => task.Run(It.IsAny<IntegrationResult>())).Callback<IIntegrationResult>(r => r.AddTaskResult("success")).Verifiable();

			IIntegrationResult result = project.Integrate(ModificationExistRequest());

			Assert.AreEqual(ProjectName, result.ProjectName);
			Assert.AreEqual(ProjectActivity.Sleeping, project.CurrentActivity);
			Assert.AreEqual(IntegrationStatus.Success, result.Status);
			Assert.AreEqual(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			Assert.AreEqual("label", result.Label);
			Assert.IsTrue(result.HasModifications());
			Assert.AreEqual(project.WorkingDirectory, result.WorkingDirectory);
			Assert.AreEqual(modifications, result.Modifications);
			Assert.IsTrue(result.EndTime >= result.StartTime);
			VerifyAll();
		}

		[Test]
		public void RethrowExceptionIfLoadingStateFileThrowsException()
		{
			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(true).Verifiable();
			mockStateManager.Setup(_manager => _manager.LoadState(ProjectName)).Throws(new CruiseControlException("expected exception")).Verifiable();

            var resultMock = new Mock<IIntegrationResult>();
            resultMock.SetupGet(_result => _result.Status).Returns(IntegrationStatus.Unknown).Verifiable();
            resultMock.SetupGet(_result => _result.StartTime).Returns(DateTime.Now).Verifiable();
            resultMock.SetupGet(_result => _result.Succeeded).Returns(false).Verifiable();


            Assert.That(delegate { project.Integrate(ModificationExistRequest()); },
                        Throws.TypeOf<CruiseControlException>());
			VerifyAll();
		}

		[Test]
		public void SourceControlLabelled()
		{
			mockLabeller.Setup(labeller => labeller.Generate(It.IsAny<IIntegrationResult>())).Returns("1.2.1").Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetModifications(It.IsAny<IIntegrationResult>(), It.IsAny<IIntegrationResult>())).Returns(CreateModifications()).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetSource(It.IsAny<IIntegrationResult>())).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.LabelSourceControl(It.IsAny<IIntegrationResult>())).Verifiable();
			mockPublisher.Setup(publisher => publisher.Run(It.IsAny<IIntegrationResult>())).Verifiable();
			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable();
			mockTask.Setup(task => task.Run(It.IsAny<IntegrationResult>())).Callback<IIntegrationResult>(r => r.AddTaskResult("success")).Verifiable();
			mockStateManager.Setup(_manager => _manager.SaveState(It.IsAny<IIntegrationResult>())).Verifiable();

			IIntegrationResult results = project.Integrate(ModificationExistRequest());

			Assert.AreEqual(results, project.CurrentResult, "new integration result has not been set to the last integration result");
			Assert.IsNotNull(results.EndTime);
			VerifyAll();
		}

		// Run Tasks

		[Test]
		public void ShouldStopBuildIfTaskFails()
		{
			IntegrationResult result = IntegrationResultMother.CreateFailed();
			mockTask.Setup(task => task.Run(result)).Verifiable();

			var secondTask = new Mock<ITask>();

			project.Tasks = new ITask[] { (ITask) mockTask.Object, (ITask) secondTask.Object };
			project.Run(result);
			VerifyAll();
			secondTask.Verify(task => task.Run(It.IsAny<IntegrationResult>()), Times.Never);
		}

		private static Modification[] CreateModifications()
		{
			Modification[] modifications = new Modification[3];
			for (int i = 0; i < modifications.Length; i++)
			{
				modifications[i] = new Modification();
				modifications[i].ModifiedTime = DateTime.Today.AddDays(-1);
			}
			return modifications;
		}

		// publishers will need to log their own exceptions
		[Test] public void IfPublisherThrowsExceptionShouldStillSaveState()
		{
			mockLabeller.Setup(labeller => labeller.Generate(It.IsAny<IIntegrationResult>())).Returns("1.0").Verifiable();
			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable();
			mockStateManager.Setup(_manager => _manager.SaveState(It.IsAny<IIntegrationResult>())).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetModifications(It.IsAny<IIntegrationResult>(), It.IsAny<IIntegrationResult>())).Returns(CreateModifications()).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetSource(It.IsAny<IIntegrationResult>())).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.LabelSourceControl(It.IsAny<IIntegrationResult>())).Verifiable();
			Exception expectedException = new CruiseControlException("expected exception");
			mockPublisher.Setup(publisher => publisher.Run(It.IsAny<IIntegrationResult>())).Throws(expectedException).Verifiable();
			mockTask.Setup(task => task.Run(It.IsAny<IntegrationResult>())).Callback<IIntegrationResult>(r => r.AddTaskResult("success")).Verifiable();

			IIntegrationResult results = project.Integrate(ModificationExistRequest());

			// failure to save the integration result will register as a failed project
			Assert.AreEqual(results, project.CurrentResult, "new integration result has not been set to the last integration result");
			Assert.IsNotNull(results.EndTime);
			VerifyAll();
		}

		[Test]
		public void TimedoutTaskShouldFailBuildIfPublishExceptionsIsTrue()
		{
			mockLabeller.Setup(labeller => labeller.Generate(It.IsAny<IIntegrationResult>())).Returns("1.0").Verifiable();
			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable();
			mockStateManager.Setup(_manager => _manager.SaveState(It.IsAny<IIntegrationResult>())).Verifiable();
			mockTask.Setup(task => task.Run(It.IsAny<IIntegrationResult>())).Throws(new CruiseControlException()).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetModifications(It.IsAny<IIntegrationResult>(), It.IsAny<IIntegrationResult>())).Returns(CreateModifications()).Verifiable();
			mockSourceControl.Setup(sourceControl => sourceControl.GetSource(It.IsAny<IIntegrationResult>())).Verifiable();
			mockPublisher.Setup(publisher => publisher.Run(It.IsAny<IIntegrationResult>())).Verifiable();

			project.Integrate(ForceBuildRequest());

			VerifyAll();
		}

		[Test]
		public void AddedMessageShouldBeIncludedInProjectStatus()
		{
			mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable();
			mockTrigger.SetupGet(_trigger => _trigger.NextBuild).Returns(DateTime.Now).Verifiable();

			Message message = new Message("foo");
			project.AddMessage(message);
			ProjectStatus status = project.CreateProjectStatus(new ProjectIntegrator(project, queue));
			Assert.AreEqual(message, status.Messages[0]);
		}
		
        //[Test]
        //public void PrebuildShouldIncrementLabelAndRunPrebuildTasks()
        //{
        //    IntegrationResult result = IntegrationResult.CreateInitialIntegrationResult(ProjectName, "c:\\root\\workingdir", "c:\\root\\artifactdir");
        //    mockStateManager.ExpectAndReturn("HasPreviousState", false, ProjectName);
        //    mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
        //    IMock mockPrebuildTask = mockery.NewStrictMock(typeof(ITask));
        //    mockPrebuildTask.Expect("Run", result);
        //    project.PrebuildTasks = new ITask[] { (ITask) mockPrebuildTask.MockInstance };
        //    project.Prebuild(result);
        //    Assert.AreEqual("1.0", result.Label);
        //}

        [Test]
        public void RetrieveBuildFinalStatusReturnsNullIfThereIsNoDataStore()
        {
            var project = new Project();
            var status = project.RetrieveBuildFinalStatus("testBuild");
        }

        [Test]
        public void RetrieveBuildFinalStatusReturnsDataStoreResult()
        {
            var dataStoreMock = this.mocks.Create<IDataStore>(MockBehavior.Strict).Object;
            var project = new Project
                {
                    DataStore = dataStoreMock
                };
            var buildName = "testBuild";
            var expected = new ItemStatus();
            Mock.Get(dataStoreMock).Setup(_dataStoreMock => _dataStoreMock.LoadProjectSnapshot(project, buildName)).Returns(expected).Verifiable();

            var actual = project.RetrieveBuildFinalStatus(buildName);

            this.mocks.VerifyAll();
            Assert.AreSame(expected, actual);
        }

        #region PublishResults() tests
        [Test]
        public void ShouldClearMessagesAfterSuccessfulBuild()
        {
            mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable();
            mockTrigger.SetupGet(_trigger => _trigger.NextBuild).Returns(DateTime.Now).Verifiable();
            mockPublisher.Setup(publisher => publisher.Run(It.IsAny<IntegrationResult>())).Callback<IIntegrationResult>(r => r.AddTaskResult("success")).Verifiable();

            project.AddMessage(new Message("foo"));
            project.PublishResults(IntegrationResultMother.CreateSuccessful());
            ProjectStatus status = project.CreateProjectStatus(new ProjectIntegrator(project, queue));
            Assert.AreEqual(0, status.Messages.Length);
        }

        [Test]
        public void DoNotClearMessagesAfterFailedBuild()
        {
            mockStateManager.Setup(_manager => _manager.HasPreviousState(ProjectName)).Returns(false).Verifiable();
            mockTrigger.SetupGet(_trigger => _trigger.NextBuild).Returns(DateTime.Now).Verifiable();
            mockPublisher.Setup(publisher => publisher.Run(It.IsAny<IntegrationResult>())).Callback<IIntegrationResult>(r => r.AddTaskResult("success")).Verifiable();

            project.AddMessage(new Message("foo"));
            project.PublishResults(IntegrationResultMother.CreateFailed());
            ProjectStatus status = project.CreateProjectStatus(new ProjectIntegrator(project, queue));
            Assert.AreEqual(2, status.Messages.Length);
        }

        [Test]
        public void PublishResultsShouldCleanTemporaryResultsOnSuccess()
        {
            // Set up the test
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            var parameters = new List<NameValuePair>();
            Mock.Get(result).SetupGet(_result => _result.Parameters).Returns(parameters);
            Mock.Get(result).SetupGet(_result => _result.Succeeded).Returns(true);
            var results = new List<ITaskResult>();
            Mock.Get(result).SetupGet(_result => _result.TaskResults).Returns(results);
            var project = new Project();
            project.Publishers = new ITask[]
            {
                new PhantomPublisher(false)
            };
            var cleaned = false;
            var tempResult = new PhantomResult(p => { cleaned = true; });
            results.Add(tempResult);

            // Run the test
            project.PublishResults(result);

            // Check the results
            this.mocks.VerifyAll();
            Assert.IsTrue(cleaned);
        }

        [Test]
        public void PublishResultsShouldNotCleanTemporaryResultsWithoutAMerge()
        {
            // Set up the test
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            var parameters = new List<NameValuePair>();
            Mock.Get(result).SetupGet(_result => _result.Parameters).Returns(parameters);
            Mock.Get(result).SetupGet(_result => _result.Succeeded).Returns(true);
            var results = new List<ITaskResult>();
            Mock.Get(result).SetupGet(_result => _result.TaskResults).Returns(results);
            var project = new Project();
            project.Publishers = new ITask[0];
            var tempResult = new PhantomResult(p => { Assert.Fail("CleanUp() called"); });
            results.Add(tempResult);

            // Run the test
            project.PublishResults(result);

            // Check the results
            this.mocks.VerifyAll();
        }

        [Test]
        public void PublishResultsShouldCleanTemporaryResultsOnFailure()
        {
            // Set up the test
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            var parameters = new List<NameValuePair>();
            Mock.Get(result).SetupGet(_result => _result.Parameters).Returns(parameters);
            Mock.Get(result).SetupGet(_result => _result.Succeeded).Returns(false);
            Mock.Get(result).SetupGet(_result => _result.Modifications).Returns(new Modification[0]);
            Mock.Get(result).SetupGet(_result => _result.FailureUsers).Returns(new ArrayList());
            Mock.Get(result).SetupGet(_result => _result.FailureTasks).Returns(new ArrayList());
            var results = new List<ITaskResult>();
            Mock.Get(result).SetupGet(_result => _result.TaskResults).Returns(results);
            var project = new Project();
            project.Publishers = new ITask[]
            {
                new PhantomPublisher(false)
            };
            var cleaned = false;
            var tempResult = new PhantomResult(p => { cleaned = true; });
            results.Add(tempResult);

            // Run the test
            project.PublishResults(result);

            // Check the results
            this.mocks.Verify();
            Assert.IsTrue(cleaned);
        }

        [Test]
        public void PublishResultsShouldNotCleanTemporaryResultsOnMergeFailure()
        {
            // Set up the test
            var result = this.mocks.Create<IIntegrationResult>(MockBehavior.Strict).Object;
            var parameters = new List<NameValuePair>();
            Mock.Get(result).SetupGet(_result => _result.Parameters).Returns(parameters);
            Mock.Get(result).SetupGet(_result => _result.Succeeded).Returns(false);
            Mock.Get(result).SetupGet(_result => _result.Modifications).Returns(new Modification[0]);
            Mock.Get(result).SetupGet(_result => _result.FailureUsers).Returns(new ArrayList());
            Mock.Get(result).SetupGet(_result => _result.FailureTasks).Returns(new ArrayList());
            var results = new List<ITaskResult>();
            Mock.Get(result).SetupGet(_result => _result.TaskResults).Returns(results);
            var project = new Project();
            project.Publishers = new ITask[]
            {
                new PhantomPublisher(true)
            };
            var tempResult = new PhantomResult(p => { Assert.Fail("CleanUp() called"); });
            results.Add(tempResult);

            // Run the test
            project.PublishResults(result);

            // Check the results
            this.mocks.Verify();
        }
        #endregion

        private class PhantomPublisher
            : ITask, IMergeTask
        {
            private bool failOnRun;

            public PhantomPublisher(bool failOnRun)
            {
                this.failOnRun = failOnRun;
            }

            public void Run(IIntegrationResult result)
            {
                if (failOnRun)
                {
                    throw new CruiseControlException("Failing on run");
                }
            }
        }

        private class PhantomResult
            : ITaskResult, ITemporaryResult
        {
            private Action<PhantomResult> onCleanUp;

            public PhantomResult(Action<PhantomResult> onCleanUp)
            {
                this.onCleanUp = onCleanUp;
            }

            public string Data { get;set; }

            public bool CheckIfSuccess()
            {
                return true;
            }

            public void CleanUp()
            {
                this.onCleanUp(this);
            }
        }
	}
}
