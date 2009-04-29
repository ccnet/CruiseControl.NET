using System;
using NMock;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.CCTrayLib;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation;
using System.Collections.Generic;

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
			monitor1 = new DynamicMock(typeof (IProjectMonitor));
			monitor2 = new DynamicMock(typeof (IProjectMonitor));
			monitor3 = new DynamicMock(typeof (IProjectMonitor));

			monitors = new IProjectMonitor[]
				{
					(IProjectMonitor) monitor1.MockInstance,
					(IProjectMonitor) monitor2.MockInstance,
					(IProjectMonitor) monitor3.MockInstance,
				};

			aggregator = new AggregatingProjectMonitor(monitors);
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
			monitor1.Expect("Poll");
			monitor2.Expect("Poll");
			monitor3.Expect("Poll");
			aggregator.Poll();
		}

		[Test, ExpectedException(typeof (InvalidOperationException))]
		public void ThrowsWhenAttemptingToRetrieveSingleProjectDetail()
		{
			ISingleProjectDetail detail = aggregator.Detail;

			// this line just here to stop resharper complaining
			Assert.IsNotNull(detail);
		}

		private int buildOccurredCount;
		private MonitorBuildOccurredEventArgs lastBuildOccurredEventArgs;

		[Test]
		public void BuildOccuredIsFiredWheneverAnyContainedProjectStatusFiresIt()
		{
			buildOccurredCount = 0;
			lastBuildOccurredEventArgs = null;

			StubProjectMonitor stubProjectMonitor1 = new StubProjectMonitor("project1");
			StubProjectMonitor stubProjectMonitor2 = new StubProjectMonitor("project2");

			aggregator = new AggregatingProjectMonitor(stubProjectMonitor1, stubProjectMonitor2);
			aggregator.BuildOccurred += new MonitorBuildOccurredEventHandler(Aggregator_BuildOccurred);

			Assert.AreEqual(0, buildOccurredCount);
			stubProjectMonitor1.OnBuildOccurred(new MonitorBuildOccurredEventArgs(stubProjectMonitor1, BuildTransition.Fixed));

			Assert.AreEqual(1, buildOccurredCount);
			Assert.AreSame(stubProjectMonitor1, lastBuildOccurredEventArgs.ProjectMonitor);
			Assert.AreEqual(BuildTransition.Fixed, lastBuildOccurredEventArgs.BuildTransition);
		}

		private void Aggregator_BuildOccurred(object sauce, MonitorBuildOccurredEventArgs e)
		{
			buildOccurredCount++;
			lastBuildOccurredEventArgs = e;
		}

		private int pollCount;
		private object lastPolledSource;
		private MonitorPolledEventArgs lastPolledArgs;

		[Test]
		public void PolledIsFiredWheneverAnyContainedProjectStatusFiresIt()
		{
			pollCount = 0;

			StubProjectMonitor stubProjectMonitor1 = new StubProjectMonitor("project1");
			StubProjectMonitor stubProjectMonitor2 = new StubProjectMonitor("project2");

			aggregator = new AggregatingProjectMonitor(stubProjectMonitor1, stubProjectMonitor2);
			aggregator.Polled += new MonitorPolledEventHandler(Aggregator_Polled);

			Assert.AreEqual(0, pollCount);
			stubProjectMonitor1.OnPolled(new MonitorPolledEventArgs(stubProjectMonitor1));

			Assert.AreEqual(1, pollCount);
		}

		private void Aggregator_Polled(object source, MonitorPolledEventArgs args)
		{
			pollCount++;
			lastPolledSource = source;
			lastPolledArgs = args;
		}


		[Test]
		public void WhenPolledIsFiredTheSourcePointToTheAggregatorNotTheFiringProject()
		{
			StubProjectMonitor stubProjectMonitor1 = new StubProjectMonitor("project1");

			aggregator = new AggregatingProjectMonitor(stubProjectMonitor1);
			aggregator.Polled += new MonitorPolledEventHandler(Aggregator_Polled);

			stubProjectMonitor1.OnPolled(new MonitorPolledEventArgs(stubProjectMonitor1));

			Assert.AreSame(lastPolledSource, aggregator);
			Assert.AreSame(lastPolledArgs.ProjectMonitor, stubProjectMonitor1);
		}


		[Test]
		public void ProjectStateReturnsTheWorstStateOfAllMonitors()
		{
			// so the states, most significant first, are:
			//  Broken
			//  Building
			//  NotConnected
			//  Success

			Assert.AreEqual(ProjectState.Success, CombinedState(ProjectState.Success, ProjectState.Success, ProjectState.Success));
			Assert.AreEqual(ProjectState.Building,
			                CombinedState(ProjectState.Success, ProjectState.Building, ProjectState.Success));
			Assert.AreEqual(ProjectState.Building,
			                CombinedState(ProjectState.Building, ProjectState.Success, ProjectState.NotConnected));
			Assert.AreEqual(ProjectState.NotConnected,
			                CombinedState(ProjectState.Success, ProjectState.Success, ProjectState.NotConnected));
			Assert.AreEqual(ProjectState.Broken,
			                CombinedState(ProjectState.NotConnected, ProjectState.Success, ProjectState.Broken));
			Assert.AreEqual(ProjectState.Broken, CombinedState(ProjectState.Broken, ProjectState.Building, ProjectState.Success));
			Assert.AreEqual(ProjectState.Broken,
			                CombinedState(ProjectState.Success, ProjectState.Broken, ProjectState.NotConnected));
		}

		private ProjectState CombinedState(ProjectState state1, ProjectState state2, ProjectState state3)
		{
			monitor1.SetupResult("ProjectState", state1);
			monitor2.SetupResult("ProjectState", state2);
			monitor3.SetupResult("ProjectState", state3);

			return aggregator.ProjectState;
		}

		[Test]
		public void ProjectSummaryStringCombinesAllStringsWithNewLinesBetween()
		{
			monitor1.ExpectAndReturn("SummaryStatusString", "hello from monitor1");
			monitor2.ExpectAndReturn("SummaryStatusString", "and from monitor2");
			monitor3.ExpectAndReturn("SummaryStatusString", "goodbye from monitor3");
			string statusString = aggregator.SummaryStatusString;

			Assert.AreEqual("hello from monitor1\nand from monitor2\ngoodbye from monitor3", statusString);
		}

		[Test]
		public void ProjectSummaryStringDoesNotIncludeBlankLinesWhenAProjectReturnsNothing()
		{
			monitor1.ExpectAndReturn("SummaryStatusString", "hello from monitor1");
			monitor2.ExpectAndReturn("SummaryStatusString", string.Empty);
			monitor3.ExpectAndReturn("SummaryStatusString", "goodbye from monitor3");
			string statusString = aggregator.SummaryStatusString;

			Assert.AreEqual("hello from monitor1\ngoodbye from monitor3", statusString);
		}

		[Test]
		public void ProjectSummaryStringReturnsADefaultMessageIfAllProjectsReturnEmptyString()
		{
			monitor1.ExpectAndReturn("SummaryStatusString", string.Empty);
			monitor2.ExpectAndReturn("SummaryStatusString", string.Empty);
			monitor3.ExpectAndReturn("SummaryStatusString", string.Empty);
			string statusString = aggregator.SummaryStatusString;

			Assert.AreEqual("All builds are good", statusString);
		}

		[Test]
		public void IntegrationResultReturnsTheWorstResultOfAllMonitors()
		{
			// so the states, most significant first, are:
			//  Failure
			//  Exception
			//  Unknown
			//  Success

			Assert.AreEqual(IntegrationStatus.Success,
			                CombinedIntegrationStatus(IntegrationStatus.Success, IntegrationStatus.Success, IntegrationStatus.Success));
			Assert.AreEqual(IntegrationStatus.Unknown,
			                CombinedIntegrationStatus(IntegrationStatus.Success, IntegrationStatus.Success, IntegrationStatus.Unknown));
			Assert.AreEqual(IntegrationStatus.Exception,
			                CombinedIntegrationStatus(IntegrationStatus.Success, IntegrationStatus.Exception, IntegrationStatus.Success));
			Assert.AreEqual(IntegrationStatus.Exception,
			                CombinedIntegrationStatus(IntegrationStatus.Success, IntegrationStatus.Exception, IntegrationStatus.Unknown));
			Assert.AreEqual(IntegrationStatus.Failure,
			                CombinedIntegrationStatus(IntegrationStatus.Failure, IntegrationStatus.Exception, IntegrationStatus.Success));
			Assert.AreEqual(IntegrationStatus.Failure,
			                CombinedIntegrationStatus(IntegrationStatus.Failure, IntegrationStatus.Success, IntegrationStatus.Success));
		}

		private IntegrationStatus CombinedIntegrationStatus(IntegrationStatus state1, IntegrationStatus state2, IntegrationStatus state3)
		{
			monitor1.SetupResult("IntegrationStatus", state1);
			monitor2.SetupResult("IntegrationStatus", state2);
			monitor3.SetupResult("IntegrationStatus", state3);

			return aggregator.IntegrationStatus;
		}

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void ForceBuildThrowsAnNotImplementedException()
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            aggregator.ForceBuild(parameters);
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void FixBuildThrowsAnNotImplementedException()
        {
            aggregator.FixBuild("JoeSmith");
        }

        [Test]
        [ExpectedException(typeof(NotImplementedException))]
        public void CancelPendingThrowsAnNotImplementedException()
        {
            aggregator.CancelPending();
        }        


    }
}
