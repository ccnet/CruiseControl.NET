using System;
using NMock;
using NUnit.Framework;

using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ServerMonitorTest
	{
		private DynamicMock mockServerManager;
		private ServerMonitor monitor;
		private int pollCount;
		private int queueChangedCount;

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

		[Test]
		public void IfThePollIsStoppedAndStartedQueueChangedIsFiredRegardless()
		{
			Assert.AreEqual(0, queueChangedCount);
            CruiseServerSnapshot snapshot = CreateCruiseServerSnapshot();

            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

			// Simulate the Poll being stopped and started which should call OnPollStarting.
			monitor.OnPollStarting();

			// Now we expect the snapshot to be republished.
            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(2, queueChangedCount);

			// But this should be a one-off thing - the next poll will revert to normal behaviour
            mockServerManager.ExpectAndReturn("GetCruiseServerSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(2, queueChangedCount);
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
            snapshot.QueueSetSnapshot.Queues[0].Requests.Add(new QueuedRequestSnapshot("Project"));
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
	}
}
