using System;
using System.Collections;
using System.Diagnostics;

using NMock;
using NMock.Constraints;

using NUnit.Framework;

using ThoughtWorks.CruiseControl.Core.Builder.Test;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Publishers.Test;
using ThoughtWorks.CruiseControl.Core.Schedules;
using ThoughtWorks.CruiseControl.Core.Sourcecontrol.Test;
using ThoughtWorks.CruiseControl.Core.State;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.Core.Test
{
	[TestFixture]
	public class ProjectTest : CustomAssertion
	{
		private Project _project;
		private TestTraceListener _listener;

		[SetUp]
		protected void SetUp()
		{
			_project = new Project();
			_project.Name = "test";
			_listener = new TestTraceListener();
			Trace.Listeners.Add(_listener);
		}

		[TearDown]
		protected void TearDown()
		{
			Trace.Listeners.Remove(_listener);
		}

		[Test]
		public void GetLastIntegration_NoPreviousBuild()
		{
			Mock mock = SetMockStateManager(false, null);
			IntegrationResult last = _project.LastIntegrationResult;
			AssertNotNull(last);
			AssertEquals(DateTime.Now.AddDays(-1), last.LastModificationDate);		// will load all modifications
			mock.Verify();
		}

		[Test]
		public void GetLastIntegration_LoadLastState()
		{
			IntegrationResult expected = new IntegrationResult();
			expected.Label = "previous";
			expected.Output = "<foo>blah</foo>";

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
			SetMockStateManager(false, null);
			
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

			DynamicMock publisherMock = new DynamicMock(typeof(PublisherBase));
			CollectingConstraint constraint = new CollectingConstraint();
			publisherMock.Expect("PublishIntegrationResults", new IsAnything(), constraint);

			DynamicMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.Expect("SaveState", results);

			ArrayList publishers = new ArrayList();
			publishers.Add((PublisherBase)publisherMock.MockInstance);
			_project.Publishers = publishers;
			_project.StateManager = (IStateManager)stateMock.MockInstance;

			_project.PostBuild(results);

			// verify event was sent
			publisherMock.Verify();
			AssertNotNull(constraint.Parameter);
			AssertEquals(results, (IntegrationResult)constraint.Parameter);

			// verify build was written to state manager
			stateMock.Verify();

			AssertEquals("verify that current build has become last build", results, _project.LastIntegrationResult);
		}
 
		[Test]
		public void Build_WithModifications()
		{
			DynamicMock builderMock = new DynamicMock(typeof(IBuilder));
			builderMock.Expect("Run", new IsAnything());

			DynamicMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.ExpectAndReturn("StateFileExists", false);

			_project.SourceControl = new MockSourceControl();
			_project.Builder = (IBuilder)builderMock.MockInstance;
			_project.StateManager = (IStateManager)stateMock.MockInstance;

			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			AssertEquals(ProjectActivity.Sleeping, _project.CurrentActivity);

			AssertEquals(3, results.Modifications.Length);
			// verify that build was invoked
			builderMock.Verify();
			// verify postbuild invoked
			AssertEquals(results, _project.LastIntegrationResult);
			TimeSpan span = DateTime.Now - results.StartTime;
			Assert("start time is not set", results.StartTime != DateTime.MinValue);
			Assert("end time is not set", results.EndTime != DateTime.MinValue);
		}

		[Test]
		public void Build_NoModifications()
		{
			DynamicMock builderMock = new DynamicMock(typeof(IBuilder));
			builderMock.ExpectNoCall("Run");

			DynamicMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.ExpectAndReturn("StateFileExists", false);
			int buildTimeout = 1000;

			_project.LastIntegrationResult = new IntegrationResult();
			_project.LastIntegrationResult.Modifications = CreateModifications();
			_project.LastIntegrationResult.StartTime = MockSourceControl.LastModificationTime;
			_project.SourceControl = new MockSourceControl();
			_project.Builder = (IBuilder)builderMock.MockInstance;
			_project.StateManager = (IStateManager)stateMock.MockInstance;
			_project.Name = "Test";
			_project.Schedule = new Schedule(buildTimeout, 1);

			IntegrationResult originalLastResult = _project.LastIntegrationResult;

			Configuration configuration = new Configuration();
			configuration.AddProject(_project);

			IMock mockConfig = new DynamicMock(typeof(IConfigurationLoader));
			mockConfig.ExpectAndReturn("Load", configuration);

			CruiseServer control = new CruiseServer((IConfigurationLoader)mockConfig.MockInstance);

			DateTime start = DateTime.Now;
			control.Start(); // RunIntegration();
			control.WaitForExit();
			DateTime stop = DateTime.Now;

			// verify that build was NOT invoked and postbuild was NOT invoked
			builderMock.Verify();
			AssertSame(originalLastResult, _project.LastIntegrationResult);

			// verify that project slept
			Assert("The project should have slept", stop >= start);

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
		public void TestStateChange()
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
		public void HandleStateManagerException()
		{
			MockPublisher publisher = new MockPublisher();
			IMock stateMock = new DynamicMock(typeof(IStateManager));
			Exception expectedException = new CruiseControlException("expected exception");
			stateMock.ExpectAndThrow("StateFileExists", expectedException);
			_project.StateManager = (IStateManager)stateMock.MockInstance;
			_project.IntegrationCompleted += publisher.IntegrationCompletedEventHandler;

			IntegrationResult results = _project.RunIntegration(BuildCondition.IfModificationExists);

			// if this fails (via NMock), it's because the last integration result was not set
			AssertEquals(results, _project.LastIntegrationResult);

			AssertEquals(expectedException, _project.LastIntegrationResult.ExceptionResult);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegrationResult.Status);
			AssertNotNull(results.EndTime);
			Assert(publisher.Published);
			stateMock.Verify();
			AssertEquals("Number of messages logged.", 2, _listener.Traces.Count);
		}

		[Test]
		public void HandleLabellerException()
		{
			MockPublisher publisher = new MockPublisher();
			IMock mockLabeller = new DynamicMock(typeof(ILabeller));
			Exception expectedException = new CruiseControlException("expected exception");
			mockLabeller.ExpectAndThrow("Generate", expectedException, new NMock.Constraints.IsAnything());
			_project.Labeller = (ILabeller)mockLabeller.MockInstance;
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
			mockLabeller.Verify();
			AssertEquals("Number of messages logged.", 2, _listener.Traces.Count);
		}

		[Test]
		public void SourceControlLabeled()
		{
			//			SetMockSourceControl();
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			MockPublisher publisher = new MockPublisher();
			IMock mock = new DynamicMock(typeof(ILabeller));
			mock.ExpectAndReturn("Generate", "1.2.1", new NMock.Constraints.IsAnything());
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
			AssertEquals("Number of messages logged.", 4, _listener.Traces.Count);
		}

		[Test]
		public void HandleBuildResultSaveException()
		{
			IMock mock = new DynamicMock(typeof(IStateManager));
			mock.ExpectAndReturn("StateFileExists", false);
			Exception expectedException = new CruiseControlException("expected exception");
			mock.ExpectAndThrow("SaveState", expectedException, new NMock.Constraints.IsAnything());
			_project.StateManager = (IStateManager)mock.MockInstance;
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
			mock.Verify();
			AssertEquals("Number of messages logged.", 5, _listener.Traces.Count);
		}

		[Test]
		public void HandlePublisherException()
		{
			IMock mock = new DynamicMock(typeof(IStateManager));
			mock.ExpectAndReturn("StateFileExists", false);
			Exception expectedException = new CruiseControlException("expected exception");
			mock.ExpectAndThrow("SaveState", expectedException, new NMock.Constraints.IsAnything());
			_project.StateManager = (IStateManager)mock.MockInstance;
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
			mock.Verify();
			AssertEquals("Number of messages logged.", 5, _listener.Traces.Count);
		}

		[Test]
		public void RunIntegration_NoModifications_DoesntCallBuilder()
		{
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

		private Mock SetMockStateManager(object exists, object result)
		{
			DynamicMock mock = new DynamicMock(typeof(IStateManager));
			_project.StateManager = (IStateManager)mock.MockInstance;
			if (exists != null)	mock.ExpectAndReturn("StateFileExists", exists, null);
			if (result != null) mock.ExpectAndReturn("LoadRecent", result, null);
			return mock;
		}
	}
}
