using System;
using System.Diagnostics;
using NUnit.Framework;
using NMock;
using NMock.Constraints;
using tw.ccnet.core.sourcecontrol.test;
using tw.ccnet.core.builder.test;
using tw.ccnet.core.publishers;
using tw.ccnet.core.util;

namespace tw.ccnet.core.test
{
	[TestFixture]
	public class ProjectTest
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

		public void TestLastIntegration_NoPreviousBuildInBuildHistory()
		{
			Mock mock = SetMockBuildHistory(false, null);
			IntegrationResult last = _project.LastIntegration;
			Assertion.AssertNotNull(last);
			Assertion.AssertEquals(DateTime.Now.AddDays(-1), last.LastModificationDate);		// will load all modifications
			mock.Verify();
		}

		private Mock SetMockBuildHistory(object exists, object result)
		{
			DynamicMock mock = new DynamicMock(typeof(IBuildHistory));
			_project.BuildHistory = (IBuildHistory)mock.MockInstance;
			if (exists != null)	mock.ExpectAndReturn("Exists", exists, null);
			if (result != null) mock.ExpectAndReturn("LoadRecent", result, null);
			return mock;
		}

		public void TestLastIntegration_LoadFromBuildHistory()
		{
			IntegrationResult expected = new IntegrationResult();
			expected.Label = "previous";
			expected.Output = "<foo>blah</foo>";

			DynamicMock mock = new DynamicMock(typeof(IBuildHistory));
			mock.ExpectAndReturn("Exists", true, null);
			mock.ExpectAndReturn("Load", expected, null);
			_project.BuildHistory = (IBuildHistory)mock.MockInstance;

			Assertion.AssertEquals(expected, _project.LastIntegration);
			mock.Verify();
		}

		public void TestPreBuild_InitialBuild()
		{
			_project.PreBuild();
			Assertion.AssertNotNull(_project.LastIntegration);
			Assertion.AssertNotNull(_project.CurrentIntegration);
			Assertion.AssertEquals("1", _project.CurrentIntegration.Label);
			Assertion.AssertEquals(_project.Name, _project.CurrentIntegration.ProjectName);
		}

		public void TestPostBuild()
		{
			_project.CurrentIntegration = new IntegrationResult();

			DynamicMock publisherMock = new DynamicMock(typeof(PublisherBase));
			CollectingConstraint constraint = new CollectingConstraint();
			publisherMock.Expect("Publish", new IsAnything(), constraint);

			DynamicMock historyMock = new DynamicMock(typeof(IBuildHistory));
			historyMock.Expect("Save", _project.CurrentIntegration);

			_project.AddPublisher((PublisherBase)publisherMock.MockInstance);
			_project.BuildHistory = (IBuildHistory)historyMock.MockInstance;

			_project.PostBuild();

			// verify event was sent
			publisherMock.Verify();
			Assertion.AssertNotNull(constraint.Parameter);
			Assertion.AssertEquals(_project.CurrentIntegration, (IntegrationResult)constraint.Parameter);

			// verify build was written to history
			historyMock.Verify();

			Assertion.AssertEquals("verify that current build has become last build", _project.CurrentIntegration, _project.LastIntegration);
		}
 
		public void TestBuild_WithModifications()
		{
			DynamicMock builderMock = new DynamicMock(typeof(IBuilder));
			builderMock.Expect("Build", new IsAnything());

			DynamicMock historyMock = new DynamicMock(typeof(IBuildHistory));
			historyMock.ExpectAndReturn("Exists", false);

			_project.SourceControl = new MockSourceControl();
			_project.Builder = (IBuilder)builderMock.MockInstance;
			_project.BuildHistory = (IBuildHistory)historyMock.MockInstance;

			_project.RunIntegration();

			Assertion.AssertEquals(3, _project.CurrentIntegration.Modifications.Length);
			// verify that build was invoked
			builderMock.Verify();
			// verify postbuild invoked
			Assertion.AssertEquals(_project.CurrentIntegration, _project.LastIntegration);
			TimeSpan span = DateTime.Now - _project.CurrentIntegration.StartTime;
			Assertion.Assert("start time is not set", _project.CurrentIntegration.StartTime != DateTime.MinValue);
			Assertion.Assert("end time is not set", _project.CurrentIntegration.EndTime != DateTime.MinValue);
		}

		[Test, Ignore("This test is ignored for time being...as there is problem with Thread.Sleep ")]
		public void TestBuild_NoModifications()
		{
			DynamicMock builderMock = new DynamicMock(typeof(IBuilder));
			// builderMock.ExpectNoCall("Build"); -- doesn't work in nmock right now

			DynamicMock historyMock = new DynamicMock(typeof(IBuildHistory));
			historyMock.ExpectAndReturn("Exists", false);
			int buildTimeout = 1000;

			_project.LastIntegration = new IntegrationResult();
			_project.LastIntegration.Modifications = CreateModifications();
			_project.SourceControl = new MockSourceControl();
			_project.Builder = (IBuilder)builderMock.MockInstance;
			_project.BuildHistory = (IBuildHistory)historyMock.MockInstance;
			_project.IntegrationTimeout = buildTimeout;
			_project.Name = "Test";

			CruiseControl control = new CruiseControl();
			control.AddProject(_project);
			DateTime start = DateTime.Now;
			control.RunIntegration();
			DateTime stop = DateTime.Now;

			Assertion.AssertEquals(0, _project.CurrentIntegration.Modifications.Length);
			// verify that build was NOT invoked
			builderMock.Verify();
			// verify that postbuild was NOT invoked
			Assertion.Assert(! _project.CurrentIntegration.Equals(_project.LastIntegration));

			TimeSpan delta = stop - start;
			TimeSpan expectedDelta = new TimeSpan(0, 0, 0, 0, buildTimeout);
			Assertion.Assert("The project did not sleep", delta >= expectedDelta);

		}

		[Test]
		public void ShouldRunIntegration() 
		{
			_project.CurrentIntegration = _project.LastIntegration;
			Assertion.Assert("There are no modifications, project should not run", !_project.ShouldRunIntegration());
			Modification mod = new Modification();
			mod.ModifiedTime = DateTime.Now;
			_project.CurrentIntegration.Modifications = new Modification[1];
			_project.CurrentIntegration.Modifications[0] = mod;
			Assertion.Assert("There are modifications, project should run", _project.ShouldRunIntegration());
			_project.ModificationDelay = 1000;
			Assertion.Assert("There are modifications within ModificationDelay, project should not run", !_project.ShouldRunIntegration());
			mod.ModifiedTime = DateTime.Now.AddMinutes(-1);
			Assertion.Assert("There are no modifications within ModificationDelay, project should run", _project.ShouldRunIntegration());
		}

		[Test]
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
			Assertion.Assert("Didn't sleep long enough", !(diff.TotalMilliseconds < 100));
			_project.ModificationDelay = 50;
			mod.ModifiedTime = DateTime.Now.AddMilliseconds(-5);
			Assertion.Assert("There are modifications within ModificationDelay, project should not run", !_project.ShouldRunIntegration());
			start = DateTime.Now;
			_project.Sleep();
			diff = DateTime.Now - start;
			Assertion.Assert("Didn't sleep long enough", !(diff.TotalMilliseconds < 45));
			Assertion.Assert("Slept too long", !(diff.TotalMilliseconds > 100));
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

		private Modification[] CreateModifications()
		{
			Modification[] mods = new Modification[1];
			mods[0] = new Modification();
			mods[0].ModifiedTime = MockSourceControl.LastModificationTime;
			return mods;
		}

		private void SetMockSourceControl()
		{
			// TODO: fix nmock bug
			// create mock source control that returns modifications
			//			DynamicMock sourceControlMock = new DynamicMock(typeof(ISourceControl));
			//			Modification[] expected = new Modification[2];
			//			expected[0] = new Modification();
			//			expected[1] = new Modification();
			//			sourceControlMock.ExpectAndReturn("GetModifications", expected, DateTime.MinValue, DateTime.MaxValue);
			//			_project.SourceControl = (ISourceControl)sourceControlMock.MockInstance;
			_project.SourceControl = new MockSourceControl();

			// verify that modifications were requested
			//			sourceControlMock.Verify();
		}
	}
}
