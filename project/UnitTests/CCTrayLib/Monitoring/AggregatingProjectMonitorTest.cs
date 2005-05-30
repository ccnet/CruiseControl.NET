using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class AggregatingProjectMonitorTest
	{
		private DynamicMock monitor1;
		private DynamicMock monitor2;
		private DynamicMock monitor3;
		private IProjectMonitor[] monitors;
		private AggregatingProjectMonitor aggregator;

		[SetUp]
		public void SetUp()
		{
			monitor1 = new DynamicMock( typeof (IProjectMonitor) );
			monitor2 = new DynamicMock( typeof (IProjectMonitor) );
			monitor3 = new DynamicMock( typeof (IProjectMonitor) );
			monitor1.Strict = monitor2.Strict = monitor3.Strict = true;

			monitors = new IProjectMonitor[]
				{
					(IProjectMonitor) monitor1.MockInstance,
					(IProjectMonitor) monitor2.MockInstance,
					(IProjectMonitor) monitor3.MockInstance,
				};

			aggregator = new AggregatingProjectMonitor( this.monitors );

		}

		[TearDown]
		public void TearDown()
		{
			monitor1.Verify();
			monitor2.Verify();
			monitor3.Verify();

		}

		[Test]
		public void PollInvokesPollOnAllContainedProjects()
		{
			monitor1.Expect( "Poll" );
			monitor2.Expect( "Poll" );
			monitor3.Expect( "Poll" );
			aggregator.Poll();
		}

		[Test, ExpectedException( typeof (InvalidOperationException) )]
		public void ProjectNameThrows()
		{
			string name = aggregator.ProjectName;

			// this line just here to stop resharper complaining
			Assert.IsNotNull( name );
		}

		[Test, ExpectedException( typeof (InvalidOperationException) )]
		public void ProjectStatusThrows()
		{
			ProjectStatus status = aggregator.ProjectStatus;

			// this line just here to stop resharper complaining
			Assert.IsNotNull( status );
		}

		private int buildOccurredCount;
		private MonitorBuildOccurredEventArgs lastBuildOccurredEventArgs;

		[Test]
		public void BuildOccuredIsFiredWheneverAnyContainedProjectStatusFiresIt()
		{
			buildOccurredCount = 0;
			lastBuildOccurredEventArgs = null;

			TestingProjectMonitor testingProjectMonitor1 = new TestingProjectMonitor( "project1" );
			TestingProjectMonitor testingProjectMonitor2 = new TestingProjectMonitor( "project2" );

			aggregator = new AggregatingProjectMonitor( testingProjectMonitor1, testingProjectMonitor2 );
			aggregator.BuildOccurred += new MonitorBuildOccurredEventHandler( Aggregator_BuildOccurred );

			Assert.AreEqual(0, buildOccurredCount);
			ProjectStatus status = new ProjectStatus();
			testingProjectMonitor1.OnBuildOccurred(new MonitorBuildOccurredEventArgs(testingProjectMonitor1, BuildTransition.Fixed));

			Assert.AreEqual(1, buildOccurredCount);
			Assert.AreSame(testingProjectMonitor1, lastBuildOccurredEventArgs.ProjectMonitor);
			Assert.AreEqual(BuildTransition.Fixed, lastBuildOccurredEventArgs.BuildTransition);
		}

		private void Aggregator_BuildOccurred( object sauce, MonitorBuildOccurredEventArgs e )
		{
			buildOccurredCount++;
			lastBuildOccurredEventArgs = e;
		}

		private int pollCount;

		[Test]
		public void PolledIsFiredWheneverAnyContainedProjectStatusFiresIt()
		{
			pollCount = 0;

			TestingProjectMonitor testingProjectMonitor1 = new TestingProjectMonitor( "project1" );
			TestingProjectMonitor testingProjectMonitor2 = new TestingProjectMonitor( "project2" );

			aggregator = new AggregatingProjectMonitor( testingProjectMonitor1, testingProjectMonitor2 );
			aggregator.Polled += new MonitorPolledEventHandler(Aggregator_Polled);

			Assert.AreEqual(0, pollCount);
			testingProjectMonitor1.OnPolled(new MonitorPolledEventArgs(testingProjectMonitor1));

			Assert.AreEqual(1, pollCount);
		}

		private void Aggregator_Polled( object sauce, MonitorPolledEventArgs args )
		{
			pollCount++;
		}

		[Test]
		public void ProjectStateReturnsTheWorstStateOfAllMonitors()
		{
			// so the states, worst first, are:
			//  Broken
			//  NotConnected
			//  Building
			//  Success

			Assert.AreEqual(ProjectState.Success, CombinedState(ProjectState.Success, ProjectState.Success, ProjectState.Success));
			Assert.AreEqual(ProjectState.Building, CombinedState(ProjectState.Success, ProjectState.Building, ProjectState.Success));
			Assert.AreEqual(ProjectState.NotConnected, CombinedState(ProjectState.Success, ProjectState.Success, ProjectState.NotConnected));
			Assert.AreEqual(ProjectState.NotConnected, CombinedState(ProjectState.Building, ProjectState.Success, ProjectState.NotConnected));
			Assert.AreEqual(ProjectState.Broken, CombinedState(ProjectState.Broken, ProjectState.Success, ProjectState.Success));
			Assert.AreEqual(ProjectState.Broken, CombinedState(ProjectState.NotConnected, ProjectState.Success, ProjectState.Broken));
			Assert.AreEqual(ProjectState.Broken, CombinedState(ProjectState.Broken, ProjectState.Building, ProjectState.Success));
			Assert.AreEqual(ProjectState.Broken, CombinedState(ProjectState.Success, ProjectState.Broken, ProjectState.NotConnected));
		}

		private ProjectState CombinedState( ProjectState state1, ProjectState state2, ProjectState state3 )
		{
			monitor1.SetupResult("ProjectState", state1);
			monitor2.SetupResult("ProjectState", state2);
			monitor3.SetupResult("ProjectState", state3);

			return aggregator.ProjectState;
		}
	}
}