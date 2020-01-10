using System;
using Moq;
using NUnit.Framework;

using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.Core;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
	[TestFixture]
	public class ServerMonitorTest
	{
		private Mock<ICruiseServerManager> mockServerManager;
		private ServerMonitor monitor;
		private int pollCount;
		private int queueChangedCount;
        const string PROJECT_NAME = "projectName";

		[SetUp]
		public void SetUp()
		{
			queueChangedCount = pollCount = 0;
			mockServerManager = new Mock<ICruiseServerManager>(MockBehavior.Strict);
			monitor = new ServerMonitor((ICruiseServerManager) mockServerManager.Object);
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
            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

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
            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();
			monitor.Poll();
			Assert.AreEqual(1, pollCount);

            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();
			monitor.Poll();
			Assert.AreEqual(2, pollCount);
		}

		[Test]
		public void WhenPollingEncountersAnExceptionThePolledEventIsStillFired()
		{
			Assert.AreEqual(0, pollCount);
			Exception ex = new Exception("should be caught");
            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Throws(ex).Verifiable();
			monitor.Poll();
			Assert.AreEqual(1, pollCount);
			Assert.AreEqual(ex, monitor.ConnectException);
		}

		[Test]
		public void IfTheQueueTimeStampHasChangedAQueueChangedEventIsFired()
		{
			Assert.AreEqual(0, queueChangedCount);
            CruiseServerSnapshot snapshot = CreateCruiseServerSnapshot();

            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();
			monitor.Poll();

			Assert.AreEqual(1, queueChangedCount);

            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(CreateCruiseServerSnapshot2()).Verifiable();
			monitor.Poll();

			Assert.AreEqual(2, queueChangedCount);

			mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(CreateCruiseServerSnapshot()).Verifiable();
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
            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

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
            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

            monitor.Poll(); // Force the snapshot to be loaded
            ProjectStatus projectStatus = monitor.GetProjectStatus(PROJECT_NAME);

            Assert.AreSame(result[1], projectStatus);
        }

        [Test]
        //[ExpectedException(typeof(ApplicationException), )]
        public void ProjectStatusThrowsIfProjectNotFound()
        {
            ProjectStatus[] result = new ProjectStatus[]
                {
                    CreateProjectStatus("a name"),
                    CreateProjectStatus("another name"),
            };

            CruiseServerSnapshot snapshot = new CruiseServerSnapshot(result, null);
            mockServerManager.Setup(manager => manager.GetCruiseServerSnapshot()).Returns(snapshot).Verifiable();

            monitor.Poll();// Force the snapshot to be loaded 

            Assert.That(delegate { monitor.GetProjectStatus(PROJECT_NAME); }, 
                        Throws.TypeOf<ApplicationException>().With.Message.EqualTo(
                            "Project 'projectName' not found on server"));
        }

        private static ProjectStatus CreateProjectStatus(string projectName)
        {
            return ProjectStatusFixture.New(projectName);
        }
    }
}
