using System;
using System.Diagnostics;
using System.IO;
using Exortech.NetReflector;
using NMock;
using NMock.Constraints;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.Builder.Test;
using ThoughtWorks.CruiseControl.Core.Label;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Publishers.Test;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Tasks;
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
		private IMock _mockSchedule;
		private IMock _mockLabeller;
		private IMock _mockPublisher;
		private IMock _mockTask;
		private TestTraceListener _listener;
		private const string PROJECT_NAME = "test";

		[SetUp]
		public void SetUp()
		{
			_mockBuilder = new DynamicMock(typeof (IBuilder));
			_mockBuilder.Strict = true;
			_mockSourceControl = new DynamicMock(typeof (ISourceControl));
			_mockSourceControl.Strict = true;
			_mockStateManager = new DynamicMock(typeof (IStateManager));
			_mockStateManager.Strict = true;
			_mockSchedule = new DynamicMock(typeof (ISchedule));
			_mockSchedule.Strict = true;
			_mockLabeller = new DynamicMock(typeof (ILabeller));
			_mockLabeller.Strict = true;
			_mockPublisher = new DynamicMock((typeof (PublisherBase)));
			_mockPublisher.Strict = true;
			_mockTask = new DynamicMock((typeof (ITask)));
			_mockTask.Strict = true;

			_project = new Project();
			_project.Name = PROJECT_NAME;
			_project.Builder = (IBuilder) _mockBuilder.MockInstance;
			_project.SourceControl = (ISourceControl) _mockSourceControl.MockInstance;
			_project.StateManager = (IStateManager) _mockStateManager.MockInstance;
			_project.Schedule = (ISchedule) _mockSchedule.MockInstance;
			_project.Labeller = (ILabeller) _mockLabeller.MockInstance;
			_project.Publishers = new IIntegrationCompletedEventHandler[] {(IIntegrationCompletedEventHandler) _mockPublisher.MockInstance};
			_project.Tasks = new ITask[] {(ITask) _mockTask.MockInstance};

			_listener = new TestTraceListener();
			Trace.Listeners.Add(_listener);
		}

		[TearDown]
		public void TearDown()
		{
			Trace.Listeners.Remove(_listener);

			_mockBuilder.Verify();
			_mockSourceControl.Verify();
			_mockStateManager.Verify();
			_mockSchedule.Verify();
			_mockLabeller.Verify();
			_mockPublisher.Verify();
			_mockTask.Verify();
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
	<schedule type=""schedule"" sleepSeconds=""30"" />
	<publishers>
		<xmllogger logDir=""C:\temp"" />
	</publishers>
	<tasks>
		<merge files="""" />
	</tasks>
</project>";

			Project project = (Project) NetReflector.Read(xml);
			AssertEquals("foo", project.Name);
			AssertEquals("http://localhost/ccnet", project.WebURL);
			AssertEquals(60, project.ModificationDelaySeconds);
			AssertEquals(true, project.PublishExceptions);
			AssertEquals(typeof (NAntBuilder), project.Builder);
			AssertEquals(typeof (MockSourceControl), project.SourceControl);
			AssertEquals(typeof (DefaultLabeller), project.Labeller);
			AssertEquals(typeof (IntegrationStateManager), project.StateManager);
			AssertEquals(typeof (Schedule), project.Schedule);
			AssertEquals(typeof (XmlLogPublisher), project.Publishers[0]);
			AssertEquals(typeof (MergeFilesTask), project.Tasks[0]);
			AssertEquals(@"c:\my\working\directory", project.ConfiguredWorkingDirectory);
		}

		[Test]
		public void LoadMinimalProjectXmlFromConfiguration()
		{
			string xml = @"
<project name=""foo"">
	<build type=""nant"" />
	<sourcecontrol type=""mock"" />
</project>";

			Project project = (Project) NetReflector.Read(xml);
			AssertEquals("foo", project.Name);
			AssertEquals(Project.DEFAULT_WEB_URL, project.WebURL);
			AssertEquals(0, project.ModificationDelaySeconds); //TODO: is this the correct default?  should quiet period be turned off by default?  is this sourcecontrol specific?
			AssertEquals(false, project.PublishExceptions);
			AssertEquals(typeof (NAntBuilder), project.Builder);
			AssertEquals(typeof (MockSourceControl), project.SourceControl);
			AssertEquals(typeof (DefaultLabeller), project.Labeller);
			AssertEquals(typeof (ProjectStateManager), project.StateManager);
			AssertEquals(typeof (Schedule), project.Schedule);
			AssertNull("project should contain no publishers", project.Publishers);
			AssertEquals(0, project.Tasks.Length);
		}

		[Test]
		public void RunningFirstIntegrationShouldForceBuild()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false); // running the first integration (no state file)
			_mockStateManager.Expect("SaveState", new IsAnything());
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			_mockSourceControl.ExpectAndReturn("GetModifications", new Modification[0], new IsAnything(), new IsAnything()); // return no modifications found
			_mockSourceControl.Expect("GetSource", new IsAnything());
			_mockSourceControl.Expect("LabelSourceControl", "label", new IsAnything());
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything(), new IsAnything());
			_mockTask.Expect("Run", new IsAnything(), _project);
			_project.Builder = new MockBuilder(); // need to use mock builder in order to set properties on IntegrationResult

			IntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			AssertEquals(PROJECT_NAME, result.ProjectName);
			AssertEquals("No exception should be thrown", null, result.ExceptionResult);
			AssertEquals(ProjectActivity.Sleeping, _project.CurrentActivity);
			AssertEquals(IntegrationStatus.Success, result.Status);
			AssertEquals(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			AssertEquals(BuildCondition.ForceBuild, result.BuildCondition);
			AssertEquals(result, _project.LastIntegrationResult);
			AssertEquals("label", result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			AssertEquals("no output is expected as builder is not called", MockBuilder.BUILDER_OUTPUT, result.Output);
			Assert("end time should come after start time", result.EndTime >= result.StartTime);
		}

		[Test] //TODO: question: should state be saved after a poll with no modifications and no build?? -- i think it should: implication for last build though
		public void RunningIntegrationWithNoModificationsShouldNotBuildOrPublish()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", true); // running the first integration (no state file)
			_mockStateManager.ExpectAndReturn("LoadState", IntegrationResultMother.CreateSuccessful());
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			_mockSourceControl.ExpectAndReturn("GetModifications", new Modification[0], new IsAnything(), new IsAnything()); // return no modifications found
			_mockBuilder.ExpectNoCall("Run", typeof (IntegrationResult), typeof(IProject));
			_mockPublisher.ExpectNoCall("PublishIntegrationResults", typeof (IProject), typeof (IntegrationResult));

			IntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			AssertEquals(PROJECT_NAME, result.ProjectName);
			AssertEquals("No exception should be thrown", null, result.ExceptionResult);
			AssertEquals(ProjectActivity.Sleeping, _project.CurrentActivity);
			AssertEquals(IntegrationStatus.Unknown, result.Status);
			AssertNotNull(_project.LastIntegrationResult);
			AssertEquals("label", result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			AssertNull("no output is expected as builder is not called", result.Output);
			Assert("end time should come after start time", result.EndTime >= result.StartTime);
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
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything(), new IsAnything());
			_mockTask.Expect("Run", new IsAnything(), _project);

			_project.Builder = new MockBuilder(); // need to use mock builder in order to set properties on IntegrationResult
			IntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			AssertEquals(PROJECT_NAME, result.ProjectName);
			AssertEquals(ProjectActivity.Sleeping, _project.CurrentActivity);
			AssertEquals(IntegrationStatus.Success, result.Status);
			AssertEquals(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			AssertEquals("label", result.Label);
			Assert("no modifications were returned", result.HasModifications());
			AssertEquals(modifications, result.Modifications);
			Assert("end time should come after start time", result.EndTime >= result.StartTime);
		}

		[Test]
		public void ShouldNotPublishIntegrationResultsIfPublishExceptionsIsFalseAndSourceControlThrowsAnException()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false); // running the first integration (no state file)
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			CruiseControlException expectedException = new CruiseControlException();
			_mockSourceControl.ExpectAndThrow("GetModifications", expectedException, new IsAnything(), new IsAnything());
			_mockPublisher.ExpectNoCall("PublishIntegrationResults", typeof (IProject), typeof (IntegrationResult));
			_mockStateManager.ExpectNoCall("SaveState", typeof (IntegrationResult));

			_project.PublishExceptions = false;
			IntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);
			AssertEquals(expectedException, result.ExceptionResult);
		}

		[Test]
		public void ShouldPublishIntegrationResultsIfPublishExceptionsIsTrueAndSourceControlThrowsAnException()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false); // running the first integration (no state file)
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything()); // generate new label
			CruiseControlException expectedException = new CruiseControlException();
			_mockSourceControl.ExpectAndThrow("GetModifications", expectedException, new IsAnything(), new IsAnything());
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything(), new IsAnything());
			_mockStateManager.Expect("SaveState", new IsAnything());

			_project.PublishExceptions = true;
			IntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);
			AssertEquals(expectedException, result.ExceptionResult);
		}

		// test: verify correct args are passed to sourcecontrol?  should use date of last modification from last successful build IMO

		[Test]
		public void ShouldCreateInitialIntegrationResultIfThisIsTheFirstIntegration()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false, null);
			Assert(_project.LastIntegrationResult.IsInitial());
		}

		[Test]
		public void ShouldLoadLastStateIfIntegrationHasBeenRunPreviously()
		{
			IntegrationResult expected = new IntegrationResult();
			expected.Label = "previous";
			expected.Output = "<foo>blah</foo>";
			expected.Status = IntegrationStatus.Success;

			_mockStateManager.ExpectAndReturn("StateFileExists", true);
			_mockStateManager.ExpectAndReturn("LoadState", expected);

			AssertEquals(expected, _project.LastIntegrationResult);
		}

		[Test]
		public void PostBuild()
		{
			IntegrationResult results = new IntegrationResult();
			results.Status = IntegrationStatus.Success;

			CollectingConstraint constraint = new CollectingConstraint();
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything(), constraint);
			_mockStateManager.Expect("SaveState", results);
			_mockSourceControl.Expect("LabelSourceControl", new IsAnything(), new IsAnything());

			_project.PostBuild(results);

			// verify event was sent
			AssertNotNull(constraint.Parameter);
			AssertEquals(results, (IntegrationResult) constraint.Parameter);
			AssertEquals("verify that current build has become last build", results, _project.LastIntegrationResult);
		}

		[Test]
		public void ShouldRunBuild()
		{
			IntegrationResult results = new IntegrationResult();
			results.Modifications = new Modification[0];
			Assert("There are no modifications, project should not run", !_project.ShouldRunBuild(results));

			Modification mod = new Modification();
			mod.ModifiedTime = DateTime.Now.AddSeconds(-2);
			results.Modifications = new Modification[] {mod};

			Assert("There are modifications, project should run", _project.ShouldRunBuild(results));

			_project.ModificationDelaySeconds = 100;
			Assert("There are modifications within ModificationDelay, project should not run", !_project.ShouldRunBuild(results));

			mod.ModifiedTime = DateTime.Now.AddMinutes(-2);
			Assert("There are no modifications within ModificationDelay, project should run", _project.ShouldRunBuild(results));
		}

		[Test]
		public void CreateTemporaryLabelMethodIsInvoked()
		{
			_mockSourceControl = new DynamicMock(typeof (ITemporaryLabeller));
			_project.SourceControl = (ISourceControl) _mockSourceControl.MockInstance;
			_mockSourceControl.Expect("CreateTemporaryLabel");

			_project.CreateTemporaryLabelIfNeeded();
		}

		[Test]
		public void CreateTemporaryLabelMethodNotInvokedIfNotTemporaryLabeller()
		{
			// the mock has strict test to true, so an exception will be thrown in CreateTemporaryLabel is invoked
			_project.CreateTemporaryLabelIfNeeded();
		}


		[Test]
		public void DeleteTemporaryLabelMethodIsInvokedIfBuildFailed()
		{
			IntegrationResult result = new IntegrationResult();
			result.LastIntegrationStatus = IntegrationStatus.Failure;
			_mockSourceControl = new DynamicMock(typeof (ITemporaryLabeller));
			_project.SourceControl = (ISourceControl) _mockSourceControl.MockInstance;
			_mockSourceControl.Expect("DeleteTemporaryLabel");

			_project.HandleProjectLabelling(result);
		}

		[Test]
		public void DeleteTemporaryLabelMethodNotInvokedIfBuildSuceeded()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;
			_mockSourceControl = new DynamicMock(typeof (ITemporaryLabeller));
			_project.SourceControl = (ISourceControl) _mockSourceControl.MockInstance;
			_mockSourceControl.ExpectNoCall("DeleteTemporaryLabel");

			_project.HandleProjectLabelling(result);
		}


		[Test]
		public void DeleteTemporaryLabelMethodNotInvokedIfNotTemporaryLabeller()
		{
			IntegrationResult result = new IntegrationResult();
			result.Status = IntegrationStatus.Success;

			_project.DeleteTemporaryLabelIfNeeded();
		}

		[Test]
		public void InitialActivityState()
		{
			AssertEquals(ProjectActivity.Unknown, _project.CurrentActivity);
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void RethrowExceptionIfLoadingStateFileThrowsException()
		{
			_mockStateManager.ExpectAndThrow("StateFileExists", new CruiseControlException("expected exception"));

			_project.RunIntegration(BuildCondition.IfModificationExists);
		}

		[Test, ExpectedException(typeof (CruiseControlException))]
		public void RethrowExceptionIfLabellerThrowsException()
		{
			Exception expectedException = new CruiseControlException("expected exception");
			_mockStateManager.ExpectAndReturn("StateFileExists", false);
			_mockLabeller.ExpectAndThrow("Generate", expectedException, new IsAnything());

			_project.RunIntegration(BuildCondition.IfModificationExists);
		}

		[Test]
		public void SourceControlLabeled()
		{
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			MockPublisher publisher = new MockPublisher();
			_mockLabeller.ExpectAndReturn("Generate", "1.2.1", new IsAnything());
			_mockTask.Expect("Run", new IsAnything(), _project);
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
			IMock stateMock = new DynamicMock(typeof (IStateManager));
			stateMock.ExpectAndReturn("StateFileExists", false);
			_project.StateManager = (IStateManager) stateMock.MockInstance;

			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			stateMock.Expect("SaveState", results);

			AssertEquals("new integration result has not been set to the last integration result", results, _project.LastIntegrationResult);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			AssertEquals("1.2.1", ((MockSourceControl) _project.SourceControl).Label);
		}

		[Test]
		public void HandleBuildResultSaveException()
		{
			_mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
			_mockStateManager.ExpectAndReturn("StateFileExists", false);
			Exception expectedException = new CruiseControlException("expected exception");
			_mockStateManager.ExpectAndThrow("SaveState", expectedException, new IsAnything());
			_mockTask.Expect("Run", new IsAnything(), _project);
			MockPublisher publisher = new MockPublisher();
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();

			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			// failure to save the integration result will register as a failed project
			AssertEquals("new integration result has not been set to the last integration result", results, _project.LastIntegrationResult);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegrationResult.Status);
			AssertEquals(expectedException, _project.LastIntegrationResult.ExceptionResult);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			AssertEquals("No messages logged.", 1, _listener.Traces.Count);
			Assert("Wrong message logged.", _listener.Traces[0].ToString().IndexOf(expectedException.ToString()) > 0);
		}

		[Test]
		public void HandlePublisherException()
		{
			_mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
			_mockStateManager.ExpectAndReturn("StateFileExists", false);
			Exception expectedException = new CruiseControlException("expected exception");
			_mockStateManager.ExpectAndThrow("SaveState", expectedException, new IsAnything());
			_mockTask.Expect("Run", new IsAnything(), _project);
			MockPublisher publisher = new MockPublisher();
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();

			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			// failure to save the integration result will register as a failed project
			AssertEquals("new integration result has not been set to the last integration result", results, _project.LastIntegrationResult);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegrationResult.Status);
			AssertEquals(expectedException, _project.LastIntegrationResult.ExceptionResult);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			AssertEquals("No messages logged.", 1, _listener.Traces.Count);
			Assert("Wrong message logged.", _listener.Traces[0].ToString().IndexOf(expectedException.ToString()) > 0);
		}

		[Test]
		public void ShouldCallSourceControlInitializeOnInitialize()
		{
			// Setup
			Project project = new Project();

			DynamicMock sourceControlMock = new DynamicMock(typeof(ISourceControl));
			sourceControlMock.Expect("Initialize", project);
			project.SourceControl = (ISourceControl) sourceControlMock.MockInstance;

			// Execute
			project.Initialize();
			
			// Verify
			sourceControlMock.Verify();
		}
	}
}