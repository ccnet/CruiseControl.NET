using System;
using System.Diagnostics;
using NUnit.Framework;
using NMock;
using NMock.Constraints;
using tw.ccnet.remote;
using tw.ccnet.core.sourcecontrol.test;
using tw.ccnet.core.builder.test;
using tw.ccnet.core.publishers;
using tw.ccnet.core.publishers.test;
using tw.ccnet.core.util;

namespace tw.ccnet.core.test
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
			_project.IntegrationTimeout = 10;
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
			IntegrationResult last = _project.LastIntegration;
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
			mock.ExpectAndReturn("Exists", true, null);
			mock.ExpectAndReturn("Load", expected, null);
			_project.StateManager = (IStateManager)mock.MockInstance;

			AssertEquals(expected, _project.LastIntegration);
			mock.Verify();
		}

		[Test]
		public void PreBuild_InitialBuild()
		{
			_project.PreBuild();
			AssertNotNull(_project.LastIntegration);
			AssertNotNull(_project.CurrentIntegration);
			AssertEquals("1", _project.CurrentIntegration.Label);
			AssertEquals(_project.Name, _project.CurrentIntegration.ProjectName);
		}

		[Test]
		public void PostBuild()
		{
			_project.CurrentIntegration = new IntegrationResult();

			DynamicMock publisherMock = new DynamicMock(typeof(PublisherBase));
			CollectingConstraint constraint = new CollectingConstraint();
			publisherMock.Expect("Publish", new IsAnything(), constraint);

			DynamicMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.Expect("Save", _project.CurrentIntegration);

			_project.AddPublisher((PublisherBase)publisherMock.MockInstance);
			_project.StateManager = (IStateManager)stateMock.MockInstance;

			_project.PostBuild();

			// verify event was sent
			publisherMock.Verify();
			AssertNotNull(constraint.Parameter);
			AssertEquals(_project.CurrentIntegration, (IntegrationResult)constraint.Parameter);

			// verify build was written to state manager
			stateMock.Verify();

			AssertEquals("verify that current build has become last build", _project.CurrentIntegration, _project.LastIntegration);
		}
 
		[Test]
		public void Build_WithModifications()
		{
			DynamicMock builderMock = new DynamicMock(typeof(IBuilder));
			builderMock.Expect("Build", new IsAnything());

			DynamicMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.ExpectAndReturn("Exists", false);

			_project.SourceControl = new MockSourceControl();
			_project.Builder = (IBuilder)builderMock.MockInstance;
			_project.StateManager = (IStateManager)stateMock.MockInstance;

			_project.Run();

			AssertEquals(3, _project.CurrentIntegration.Modifications.Length);
			// verify that build was invoked
			builderMock.Verify();
			// verify postbuild invoked
			AssertEquals(_project.CurrentIntegration, _project.LastIntegration);
			TimeSpan span = DateTime.Now - _project.CurrentIntegration.StartTime;
			Assert("start time is not set", _project.CurrentIntegration.StartTime != DateTime.MinValue);
			Assert("end time is not set", _project.CurrentIntegration.EndTime != DateTime.MinValue);
		}

		[Test]
		public void Build_NoModifications()
		{
			DynamicMock builderMock = new DynamicMock(typeof(IBuilder));
			builderMock.ExpectNoCall("Build");

			DynamicMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.ExpectAndReturn("Exists", false);
			int buildTimeout = 1000;

			_project.LastIntegration = new IntegrationResult();
			_project.LastIntegration.Modifications = CreateModifications();
			_project.SourceControl = new MockSourceControl();
			_project.Builder = (IBuilder)builderMock.MockInstance;
			_project.StateManager = (IStateManager)stateMock.MockInstance;
			_project.IntegrationTimeout = buildTimeout;
			_project.Name = "Test";

			CruiseControl control = new CruiseControl();
			control.AddProject(_project);
			DateTime start = DateTime.Now;
			control.RunIntegration();
			DateTime stop = DateTime.Now;

			AssertEquals(0, _project.CurrentIntegration.Modifications.Length);

			// verify that build was NOT invoked and postbuild was NOT invoked
			builderMock.Verify();
			AssertNotEquals(_project.CurrentIntegration, _project.LastIntegration);

			// verify that project slept
			Assert("The project did not sleep", stop >= start);

		}

		[Test]
		public void ShouldRunIntegration() 
		{
			_project.CurrentIntegration = _project.LastIntegration;
			Assert("There are no modifications, project should not run", !_project.ShouldRunBuild());
			Modification mod = new Modification();
			mod.ModifiedTime = DateTime.Now;
			_project.CurrentIntegration.Modifications = new Modification[1];
			_project.CurrentIntegration.Modifications[0] = mod;
			Assert("There are modifications, project should run", _project.ShouldRunBuild());
			_project.ModificationDelay = 1000;
			Assert("There are modifications within ModificationDelay, project should not run", !_project.ShouldRunBuild());
			mod.ModifiedTime = DateTime.Now.AddMinutes(-1);
			Assert("There are no modifications within ModificationDelay, project should run", _project.ShouldRunBuild());
		}

		[Test]
//			[Ignore("too fragile")]
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

		public void TestStateChange()
		{
			// test valid state transitions
			// test stopping at any point
			// test state set to starting when started
			// state set running when running
			// state set to sleeping
			// should tests be multithreaded?
			// should delegate to schedule to determine when to run and how often
		}

		[Test]
		public void NotRunWhenStopped()
		{
			_project.Stopped = true;
			_project.Run();
			AssertNull(_project.CurrentIntegration);
		}

		[Test]
		public void HandleStateManagerException()
		{
			MockPublisher publisher = new MockPublisher();
			IMock stateMock = new DynamicMock(typeof(IStateManager));
			Exception expectedException = new CruiseControlException("expected exception");
			stateMock.ExpectAndThrow("Exists", expectedException);
			_project.StateManager = (IStateManager)stateMock.MockInstance;
			_project.AddIntegrationEventHandler(publisher.IntegrationEventHandler);

			_project.Run();

			AssertEquals(_project.CurrentIntegration, _project.LastIntegration);
			AssertEquals(expectedException, _project.LastIntegration.ExceptionResult);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegration.Status);
			AssertNotNull(_project.CurrentIntegration.EndTime);
			Assert(publisher.Published);
			stateMock.Verify();
			AssertEquals(2, _listener.Traces.Count);
		}

		[Test]
		public void HandleLabellerException()
		{
			MockPublisher publisher = new MockPublisher();
			IMock mock = new DynamicMock(typeof(ILabeller));
			Exception expectedException = new CruiseControlException("expected exception");
			mock.ExpectAndThrow("Generate", expectedException, new NMock.Constraints.IsAnything());
			_project.Labeller = (ILabeller)mock.MockInstance;
			_project.AddIntegrationEventHandler(publisher.IntegrationEventHandler);
			IMock stateMock = new DynamicMock(typeof(IStateManager));
			stateMock.ExpectAndReturn("Exists", false);
			stateMock.Expect("Save", _project.CurrentIntegration);
			_project.StateManager = (IStateManager)stateMock.MockInstance;
			
			_project.Run();

			AssertEquals(_project.CurrentIntegration, _project.LastIntegration);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegration.Status);
			AssertEquals(expectedException, _project.LastIntegration.ExceptionResult);
			AssertNotNull(_project.CurrentIntegration.EndTime);
			Assert(publisher.Published);
			mock.Verify();
			AssertEquals(2, _listener.Traces.Count);
		}

		[Test]
		public void HandleBuildResultSaveException()
		{
			IMock mock = new DynamicMock(typeof(IStateManager));
			mock.ExpectAndReturn("Exists", false);
			Exception expectedException = new CruiseControlException("expected exception");
			mock.ExpectAndThrow("Save", expectedException, new NMock.Constraints.IsAnything());
			_project.StateManager = (IStateManager)mock.MockInstance;
			MockPublisher publisher = new MockPublisher();
			_project.AddIntegrationEventHandler(publisher.IntegrationEventHandler);
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			
			_project.Run();

			// failure to save the integration result will register as a failed project
			AssertEquals(_project.CurrentIntegration, _project.LastIntegration);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegration.Status);
			AssertEquals(expectedException, _project.LastIntegration.ExceptionResult);
			AssertNotNull(_project.CurrentIntegration.EndTime);
			Assert(publisher.Published);
			mock.Verify();
			AssertEquals(4, _listener.Traces.Count);
		}

		[Test]
		public void HandlePublisherException()
		{
			IMock mock = new DynamicMock(typeof(IStateManager));
			mock.ExpectAndReturn("Exists", false);
			Exception expectedException = new CruiseControlException("expected exception");
			mock.ExpectAndThrow("Save", expectedException, new NMock.Constraints.IsAnything());
			_project.StateManager = (IStateManager)mock.MockInstance;
			MockPublisher publisher = new MockPublisher();
			_project.AddIntegrationEventHandler(publisher.IntegrationEventHandler);
			_project.SourceControl = new MockSourceControl();
			_project.Builder = new MockBuilder();
			
			_project.Run();

			// failure to save the integration result will register as a failed project
			AssertEquals(_project.CurrentIntegration, _project.LastIntegration);
			AssertEquals(IntegrationStatus.Exception, _project.LastIntegration.Status);
			AssertEquals(expectedException, _project.LastIntegration.ExceptionResult);
			AssertNotNull(_project.CurrentIntegration.EndTime);
			Assert(publisher.Published);
			mock.Verify();
			AssertEquals(4, _listener.Traces.Count);
		}

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
			if (exists != null)	mock.ExpectAndReturn("Exists", exists, null);
			if (result != null) mock.ExpectAndReturn("LoadRecent", result, null);
			return mock;
		}
	}
}
