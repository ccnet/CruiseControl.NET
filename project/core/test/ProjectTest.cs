using NMock;
using NMock.Constraints;
using NUnit.Framework;
using System;
using System.Collections;
using System.Diagnostics;
using ThoughtWorks.CruiseControl.Core.Builder.Test;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Publishers.Test;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Core.Util.Test;
using ThoughtWorks.CruiseControl.Remote;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core.Builder;
using ThoughtWorks.CruiseControl.Core.Label;

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
		private TestTraceListener _listener;
		private const string PROJECT_NAME = "test";

		[SetUp]
		protected void SetUp()
		{
			_mockBuilder = new DynamicMock(typeof(IBuilder)); _mockBuilder.Strict = true;
			_mockSourceControl = new DynamicMock(typeof(ISourceControl)); _mockSourceControl.Strict = true;
			_mockStateManager = new DynamicMock(typeof(IStateManager)); _mockStateManager.Strict = true;
			_mockSchedule = new DynamicMock(typeof(ISchedule)); _mockSchedule.Strict = true;
			_mockLabeller = new DynamicMock(typeof(ILabeller)); _mockLabeller.Strict = true;
			_mockPublisher = new DynamicMock((typeof(PublisherBase))); _mockPublisher.Strict = true;

			_project = new Project();
			_project.Name = PROJECT_NAME;
			_project.Builder = (IBuilder) _mockBuilder.MockInstance;
			_project.SourceControl = (ISourceControl) _mockSourceControl.MockInstance;
			_project.StateManager = (IStateManager) _mockStateManager.MockInstance;
			_project.Schedule = (ISchedule) _mockSchedule.MockInstance;
			_project.Labeller = (ILabeller) _mockLabeller.MockInstance;
			_project.Publishers = new IIntegrationCompletedEventHandler [] { (IIntegrationCompletedEventHandler) _mockPublisher.MockInstance };

			_listener = new TestTraceListener();
			Trace.Listeners.Add(_listener);
		}

		[TearDown]
		protected void TearDown()
		{
			Trace.Listeners.Remove(_listener);

			_mockBuilder.Verify();
			_mockSourceControl.Verify();
			_mockStateManager.Verify();
			_mockSchedule.Verify();
			_mockLabeller.Verify();
			_mockPublisher.Verify();
		}

		[Test]
		public void LoadFullySpecifiedProjectFromConfiguration()
		{
			string xml = @"
<project name=""foo"" webURL=""http://localhost/ccnet"">
	<build type=""nant"" />
	<sourcecontrol type=""mock"" />
	<labeller type=""defaultlabeller"" />
	<state type=""state"" />
	<schedule type=""schedule"" sleepSeconds=""30"" />
	<publishers>
		<xmllogger logDir=""C:\temp"" />
	</publishers>
</project>";

			Project project = (Project) NetReflector.Read(xml);
			AssertEquals("foo", project.Name);
			AssertEquals("http://localhost/ccnet", project.WebURL);
			AssertEquals(typeof(NAntBuilder), project.Builder);
			AssertEquals(typeof(MockSourceControl), project.SourceControl);
			AssertEquals(typeof(DefaultLabeller), project.Labeller);
			AssertEquals(typeof(XmlLogPublisher), project.Publishers[0]);
			AssertEquals(typeof(IntegrationStateManager), project.StateManager);
			AssertEquals(typeof(Schedule), project.Schedule);
		}

		[Test]	//TODO: question: should state be saved after a poll with no mods and no build?? -- i think it should: implication for last build though
		public void RunningFirstIntegrationWithNoModificationsShouldNotBuildOrPublish()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false);				// running the first integration (no state file)
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything());		// generate new label
			_mockSourceControl.ExpectAndReturn("GetModifications", new Modification[0], new IsAnything(), new IsAnything());	// return no modifications found
			_mockBuilder.ExpectNoCall("Run", typeof(IntegrationResult));
			_mockPublisher.ExpectNoCall("PublishIntegrationResults", typeof(IProject), typeof(IntegrationResult));

			IntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			AssertEquals(PROJECT_NAME, result.ProjectName);
			AssertEquals(ProjectActivity.Sleeping, _project.CurrentActivity);
			AssertEquals(IntegrationStatus.Unknown, result.Status);
			AssertEquals(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			AssertEquals("label", result.Label);
			AssertFalse("unexpected modifications were returned", result.HasModifications());
			AssertEqualArrays(new Modification[0], result.Modifications);
			AssertNull("no output is expected as builder is not called", result.Output);
			Assert("end time should come after start time", result.EndTime >= result.StartTime);
		}

		[Test]
		public void RunningFirstIntegrationWithModificationsShouldBuildAndPublish()
		{
			Modification[] mods = new Modification[1] { new Modification()};

			_mockStateManager.ExpectAndReturn("StateFileExists", false);				// running the first integration (no state file)
			_mockStateManager.Expect("SaveState", new IsAnything());
			_mockLabeller.ExpectAndReturn("Generate", "label", new IsAnything());		// generate new label
			_mockSourceControl.ExpectAndReturn("GetModifications", mods, new IsAnything(), new IsAnything());
			_mockBuilder.Expect("Run", new IsAnything());
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything(), new IsAnything());

			IntegrationResult result = _project.RunIntegration(BuildCondition.IfModificationExists);

			AssertEquals(PROJECT_NAME, result.ProjectName);
			AssertEquals(ProjectActivity.Sleeping, _project.CurrentActivity);
			AssertEquals(IntegrationStatus.Unknown, result.Status);
			AssertEquals(IntegrationStatus.Unknown, result.LastIntegrationStatus);
			AssertEquals("label", result.Label);
			Assert("no modifications were returned", result.HasModifications());
			AssertEquals(mods, result.Modifications);
			Assert("end time should come after start time", result.EndTime >= result.StartTime);
		}

		// test: verify correct args are passed to sourcecontrol?  should use date of last modification from last successful build IMO

		[Test]
		public void GetLastIntegration_NoPreviousBuild()
		{
			_mockStateManager.ExpectAndReturn("StateFileExists", false, null);
			IntegrationResult last = _project.LastIntegrationResult;
			AssertEquals(new IntegrationResult(), last);

			AssertEquals(DateTime.Now.AddDays(-1).Date, last.LastModificationDate.Date);		// will load all modifications
		}

		[Test]
		public void GetLastIntegration_LoadLastState()
		{
			IntegrationResult expected = new IntegrationResult();
			expected.Label = "previous";
			expected.Output = "<foo>blah</foo>";
			expected.Status = IntegrationStatus.Success;

			DynamicMock mock = new DynamicMock(typeof(IStateManager));
			mock.ExpectAndReturn("StateFileExists", true, null);
			mock.ExpectAndReturn("LoadState", expected, null);
			_project.StateManager = (IStateManager)mock.MockInstance;

			AssertEquals(expected, _project.LastIntegrationResult);
			mock.Verify();
		}

		[Test]
		public void PreBuild_InitialBuild()
		{
			_mockLabeller.ExpectAndReturn("Generate", "1", new IsAnything());
			_mockStateManager.ExpectAndReturn("StateFileExists", false, null);
			
			IntegrationResult results;
			_project.CreateNewIntegrationResult(out results);

			AssertNotNull(_project.LastIntegrationResult);
			AssertNotNull(results);
			AssertEquals("1", results.Label);
			AssertEquals(_project.Name, results.ProjectName);
		}

		[Test]
		public void PostBuild()
		{
			IntegrationResult results = new IntegrationResult();

			CollectingConstraint constraint = new CollectingConstraint();
			_mockPublisher.Expect("PublishIntegrationResults", new IsAnything(), constraint);
			_mockStateManager.Expect("SaveState", results);

			_project.PostBuild(results);

			// verify event was sent
			AssertNotNull(constraint.Parameter);
			AssertEquals(results, (IntegrationResult)constraint.Parameter);
			AssertEquals("verify that current build has become last build", results, _project.LastIntegrationResult);
		}
 
		[Test]
		public void ShouldRunBuild() 
		{
			IntegrationResult results = new IntegrationResult();
			results.Modifications = new Modification[0];
			Assert("There are no modifications, project should not run", !_project.ShouldRunBuild(results, BuildCondition.IfModificationExists));
			Assert(_project.ShouldRunBuild(results, BuildCondition.ForceBuild));

			Modification mod = new Modification();
			mod.ModifiedTime = DateTime.Now;
			results.Modifications = new Modification[] { mod };

			Assert("There are modifications, project should run", _project.ShouldRunBuild(results, BuildCondition.IfModificationExists));
			Assert(_project.ShouldRunBuild(results, BuildCondition.ForceBuild));

			_project.ModificationDelaySeconds = 1000;
			Assert("There are modifications within ModificationDelay, project should not run", !_project.ShouldRunBuild(results, BuildCondition.IfModificationExists));
			Assert(_project.ShouldRunBuild(results, BuildCondition.ForceBuild));
			
			mod.ModifiedTime = DateTime.Now.AddMinutes(-1);
			Assert("There are no modifications within ModificationDelay, project should run", _project.ShouldRunBuild(results, BuildCondition.IfModificationExists));
			Assert(_project.ShouldRunBuild(results, BuildCondition.ForceBuild));
		}

		/*		[Test]
				[Ignore("too fragile")]
				public void SleepTime() 
				{
					_project.CurrentIntegration = _project.LastIntegration;
					Modification mod = new Modification();
					mod.ModifiedTime = DateTime.Now;
					_project.IntegrationTimeout = 100;
					_project.PreBuild();
					_project.CurrentIntegration.Modifications = new Modification[1];
					_project.CurrentIntegration.Modifications[0] = mod;
			
			
					DateTime start = DateTime.Now;
					_project.Sleep();
					TimeSpan diff = DateTime.Now - start;
					Assert("Didn't sleep long enough", !(diff.TotalMilliseconds < 100));
					_project.ModificationDelay = 50;
					mod.ModifiedTime = DateTime.Now.AddMilliseconds(-5);
					Assert("There are modifications within ModificationDelay, project should not run", !_project.ShouldRunBuild());
					mod.ModifiedTime = DateTime.Now.AddMilliseconds(-5);
					start = DateTime.Now;
					_project.Sleep();
					diff = DateTime.Now - start;
					Assert("Didn't sleep long enough", !(diff.TotalMilliseconds < 45));
					Assert("Slept too long", !(diff.TotalMilliseconds > 100));
				}
		*/

		[Test]
		[Ignore("Todo")]
		public void ActivityStateChange()
		{
			// TODO test valid state transitions
			// TODO test stopping at any point
			// TODO test state set to starting when started
			// TODO state set running when running
			// TODO state set to sleeping
			// TODO should tests be multithreaded?
			// TODO should delegate to schedule to determine when to run and how often
		}

		[Test]
		public void InitialActivityState()
		{
			AssertEquals(ProjectActivity.Unknown, _project.CurrentActivity);
		}		

		[Test]
		public void HandleStateManagerException()
		{
			MockPublisher publisher = new MockPublisher();
			Exception expectedException = new CruiseControlException("expected exception");
			_mockStateManager.ExpectAndThrow("StateFileExists", expectedException);
			_mockStateManager.Expect("SaveState", new IsAnything());
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;

			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			// if this fails (via NMock), it's because the last integration result was not set
			AssertEquals(results, _project.LastIntegrationResult);

			AssertEquals(expectedException, _project.LastIntegrationResult.ExceptionResult);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegrationResult.Status);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			AssertEquals("No messages logged.", 1, _listener.Traces.Count);
			Assert("Wrong message logged.", _listener.Traces[0].ToString().IndexOf(expectedException.ToString()) > 0);
		}

		[Test]
		public void HandleLabellerException()
		{
			MockPublisher publisher = new MockPublisher();
			Exception expectedException = new CruiseControlException("expected exception");
			_mockLabeller.ExpectAndThrow("Generate", expectedException, new NMock.Constraints.IsAnything());
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
			IMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.ExpectAndReturn("StateFileExists", false);
			_project.StateManager = (IStateManager)stateMock.MockInstance;
			
			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			stateMock.Expect("SaveState", results);

			AssertEquals(results, _project.LastIntegrationResult);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegrationResult.Status);
			AssertEquals(expectedException, _project.LastIntegrationResult.ExceptionResult);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			AssertEquals("No messages logged.", 1, _listener.Traces.Count);
			Assert("Wrong message logged.", _listener.Traces[0].ToString().IndexOf(expectedException.ToString()) > 0);
		}

		[Test]
		public void SourceControlLabeled()
		{
			//			SetMockSourceControl();
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			MockPublisher publisher = new MockPublisher();
			IMock mock = new DynamicMock(typeof(ILabeller));
			mock.ExpectAndReturn("Generate", "1.2.1", new IsAnything());
			_project.Labeller = (ILabeller)mock.MockInstance;
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
			IMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.ExpectAndReturn("StateFileExists", false);
			_project.StateManager = (IStateManager)stateMock.MockInstance;
			
			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			stateMock.Expect("SaveState", results);

			AssertEquals(results, _project.LastIntegrationResult);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			mock.Verify();
			AssertEquals("1.2.1", ((MockSourceControl)_project.SourceControl).Label);
		}

		[Test]
		public void HandleBuildResultSaveException()
		{
			_mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
			_mockStateManager.ExpectAndReturn("StateFileExists", false);
			Exception expectedException = new CruiseControlException("expected exception");
			_mockStateManager.ExpectAndThrow("SaveState", expectedException, new NMock.Constraints.IsAnything());
			MockPublisher publisher = new MockPublisher();
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			
			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			// failure to save the integration result will register as a failed project
			AssertEquals(results, _project.LastIntegrationResult);
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
			_mockStateManager.ExpectAndThrow("SaveState", expectedException, new NMock.Constraints.IsAnything());
			MockPublisher publisher = new MockPublisher();
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			
			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			// failure to save the integration result will register as a failed project
			AssertEquals(results, _project.LastIntegrationResult);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegrationResult.Status);
			AssertEquals(expectedException, _project.LastIntegrationResult.ExceptionResult);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			AssertEquals("No messages logged.", 1, _listener.Traces.Count);
			Assert("Wrong message logged.", _listener.Traces[0].ToString().IndexOf(expectedException.ToString()) > 0);
		}

		[Test]
		public void RunIntegration_NoModifications_DoesntCallBuilder()
		{
			_mockLabeller.ExpectAndReturn("Generate", "1.0", new IsAnything());
			_mockStateManager.ExpectAndReturn("StateFileExists", false);
			SourceControlMock sc = new SourceControlMock();
			sc.ExpectedModifications = new Modification[0];
			_project.SourceControl = sc;
			_project.Builder = new MockBuilder();

			MockBuilder builder = (MockBuilder)_project.Builder;

			Assert(!builder.HasRun);
			_project.RunIntegration(BuildCondition.IfModificationExists);
			Assert(!builder.HasRun);
		}

		[Test]
		[Ignore("Todo")]
		public void RunTwiceWithExceptionFirstTime()
		{
		}

		private Modification[] CreateModifications()
		{
			Modification[] mods = new Modification[1];
			mods[0] = new Modification();
			mods[0].ModifiedTime = MockSourceControl.LastModificationTime;
			return mods;
		}
	}
}
