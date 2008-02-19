using System;
using NMock;
using NUnit.Framework;

using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ServerMonitorTest
	{
		private DynamicMock mockServerManager;
		private ServerMonitor monitor;
		private int pollCount;
		private int queueChangedCount;
        const string PROJECT_NAME = "projectName";

		[SetUp]
		public void SetUp()
		{
			queueChangedCount = pollCount = 0;
			mockServerManager = new DynamicMock(typeof (ICruiseServerManager));
			mockServerManager.Strict = true;
			monitor = new ServerMonitor((ICruiseServerManager) mockServerManager.MockInstance);
			monitor.Polled += new MonitorServerPolledEventHandler(Monitor_Polled);
			monitor.QueueChanged += new MonitorServerQueueChangedEventHandler(Monitor_QueueChanged);
		}

		[TearDown]
		public void TearDown()
		{
			mockServerManager.Verify();
		}

		[Test]
        public void WhenPollIsCalledRetrievesANewCopyOfTheCruiseServerSnapshot()
		{
            CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);

			monitor.Poll();

			// deliberately called twice: should not go back to server on 2nd call
            Assert.AreSame(snapshot, monitor.CruiseServerSnapshot);
            Assert.AreSame(snapshot, monitor.CruiseServerSnapshot);
		}

		[Test]
		public void ThePollEventIsFiredWhenPollIsInvoked()
		{
			Assert.AreEqual(0, pollCount);

            CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();
			Assert.AreEqual(1, pollCount);

            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();
			Assert.AreEqual(2, pollCount);
		}

		[Test]
		public void WhenPollingEncountersAnExceptionThePolledEventIsStillFired()
		{
			Assert.AreEqual(0, pollCount);
			Exception ex = new Exception("should be caught");
            mockServerManager.ExpectAndThrow("GetCruiseServerSnapshot", ex);
			monitor.Poll();
			Assert.AreEqual(1, pollCount);
			Assert.AreEqual(ex, monitor.ConnectException);
		}

		[Test]
		public void IfTheQueueTimeStampHasChangedAQueueChangedEventIsFired()
		{
			Assert.AreEqual(0, queueChangedCount);
            CruiseServerSnapshot snapshot = CreateCruiseServerSnapshot();

            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot",
			                                   CreateCruiseServerSnapshot2());
			monitor.Poll();

			Assert.AreEqual(2, queueChangedCount);

			mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot",
			                                   CreateCruiseServerSnapshot());
			monitor.Poll();

			Assert.AreEqual(3, queueChangedCount);
		}

		private void Monitor_Polled(object sauce, MonitorServerPolledEventArgs args)
		{
			pollCount++;
		}

		private void Monitor_QueueChanged(object sauce, MonitorServerQueueChangedEventArgs e)
		{
			queueChangedCount++;
		}

		private CruiseServerSnapshot CreateCruiseServerSnapshot()
		{
            return new CruiseServerSnapshot();
		}

        private CruiseServerSnapshot CreateCruiseServerSnapshot2()
        {
            CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
            snapshot.QueueSetSnapshot.Queues.Add(new QueueSnapshot("Test"));
            snapshot.QueueSetSnapshot.Queues[0].Requests.Add(new QueuedRequestSnapshot("Project", ProjectActivity.CheckingModifications));
            return snapshot;
        }

        [Test]
        public void ExposesTheCruiseServerSnapshotOfTheContainedServer()
		{
            CruiseServerSnapshot snapshot = new CruiseServerSnapshot();
            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);

			monitor.Poll();

            Assert.AreEqual(snapshot, monitor.CruiseServerSnapshot);
		}

		[Test]
        public void WhenNoConnectionHasBeenMadeToTheBuildServerTheCruiseServerSnapshotIsNull()
		{
            Assert.AreEqual(null, monitor.CruiseServerSnapshot);			
		}

        [Test]
        public void ProjectStatusNullIfServerNotYetPolled()
        {
            ProjectStatus projectStatus = monitor.GetProjectStatus(PROJECT_NAME);

            Assert.IsNull(projectStatus);
        }

        [Test]
        public void ProjectStatusReturnsTheStatusForTheNominatedProject()
        {
            ProjectStatus[] result = new ProjectStatus[]
				{
					CreateProjectStatus("a name"),
					CreateProjectStatus(PROJECT_NAME),
				};

            CruiseServerSnapshot snapshot = new CruiseServerSnapshot(result, null);
            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);

            monitor.Poll(); // Force the snapshot to be loaded
            ProjectStatus projectStatus = monitor.GetProjectStatus(PROJECT_NAME);

            Assert.AreSame(result[1], projectStatus);
        }

        [Test]
        [ExpectedException(typeof(ApplicationException), "Project 'projectName' not found on server")]
        public void ProjectStatusThrowsIfProjectNotFound()
        {
            ProjectStatus[] result = new ProjectStatus[]
				{
					CreateProjectStatus("a name"),
					CreateProjectStatus("another name"),
			};

            CruiseServerSnapshot snapshot = new CruiseServerSnapshot(result, null);
            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);

            monitor.Poll(); // Force the snapshot to be loaded
            ProjectStatus projectStatus = monitor.GetProjectStatus(PROJECT_NAME);

            Assert.AreSame(result[1], projectStatus);
        }

        private static ProjectStatus CreateProjectStatus(string projectName)
        {
            return ProjectStatusFixture.New(projectName);
        }
    }
}
