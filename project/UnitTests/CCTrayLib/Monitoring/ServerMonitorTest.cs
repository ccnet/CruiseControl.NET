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
		public void WhenPollIsCalledRetrivesANewCopyOfTheIntegrationQueueSnapshot()
		{
			IntegrationQueueSnapshot snapshot = new IntegrationQueueSnapshot();
			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);

			monitor.Poll();

			// deliberately called twice: should not go back to server on 2nd call
			Assert.AreSame(snapshot, monitor.IntegrationQueueSnapshot);
			Assert.AreSame(snapshot, monitor.IntegrationQueueSnapshot);
		}

		[Test]
		public void ThePollEventIsFiredWhenPollIsInvoked()
		{
			Assert.AreEqual(0, pollCount);

			IntegrationQueueSnapshot snapshot = new IntegrationQueueSnapshot();
			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
			monitor.Poll();
			Assert.AreEqual(1, pollCount);

			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
			monitor.Poll();
			Assert.AreEqual(2, pollCount);
		}

		[Test]
		public void WhenPollingEncountersAnExceptionThePolledEventIsStillFired()
		{
			Assert.AreEqual(0, pollCount);
			Exception ex = new Exception("should be caught");
			mockServerManager.ExpectAndThrow("GetIntegrationQueueSnapshot", ex);
			monitor.Poll();
			Assert.AreEqual(1, pollCount);
			Assert.AreEqual(ex, monitor.ConnectException);
		}

		[Test]
		public void IfTheQueueTimeStampHasChangedAQueueChangedEventIsFired()
		{
			Assert.AreEqual(0, queueChangedCount);
			IntegrationQueueSnapshot snapshot = CreateIntegrationQueueSnapshot(new DateTime(2004, 1, 1));

			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot",
			                                   CreateIntegrationQueueSnapshot(new DateTime(2004, 1, 2)));
			monitor.Poll();

			Assert.AreEqual(2, queueChangedCount);

			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot",
			                                   CreateIntegrationQueueSnapshot(new DateTime(2004, 1, 3)));
			monitor.Poll();

			Assert.AreEqual(3, queueChangedCount);
		}

		[Test]
		public void IfThePollIsStoppedAndStartedQueueChangedIsFiredRegardless()
		{
			Assert.AreEqual(0, queueChangedCount);
			IntegrationQueueSnapshot snapshot = CreateIntegrationQueueSnapshot(new DateTime(2004, 1, 1));

			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

			// Simulate the Poll being stopped and started which should call OnPollStarting.
			monitor.OnPollStarting();

			// Now we expect the snapshot to be republished.
			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
			monitor.Poll();

			Assert.AreEqual(2, queueChangedCount);

			// But this should be a one-off thing - the next poll will revert to normal behaviour
			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);
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

		private IntegrationQueueSnapshot CreateIntegrationQueueSnapshot(DateTime lastBuildDate)
		{
			return new IntegrationQueueSnapshot(lastBuildDate);
		}
		
		[Test]
		public void ExposesTheIntegrationQueueSnapshotOfTheContainedServer()
		{
			IntegrationQueueSnapshot snapshot = new IntegrationQueueSnapshot();
			mockServerManager.ExpectAndReturn("GetIntegrationQueueSnapshot", snapshot);

			monitor.Poll();
			
			Assert.AreEqual(snapshot, monitor.IntegrationQueueSnapshot);
		}

		[Test]
		public void WhenNoConnectionHasBeenMadeToTheBuildServerTheIntegrationQueueSnapshotIsNull()
		{
			Assert.AreEqual(null, monitor.IntegrationQueueSnapshot);			
		}
	}
}
