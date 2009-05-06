using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Queues
{
	[TestFixture]
	public class IntegrationQueueSetTest
	{
		private IntegrationQueueSet set;

		[SetUp]
		protected void SetUp()
		{
			set = new IntegrationQueueSet();
		}

		[Test]
		public void AddQueueNameCreatesNewQueue()
		{
			set.Add("q1", new DefaultQueueConfiguration("q1"));
			IIntegrationQueue q = set["q1"];
			Assert.IsNotNull(q);
		}

		[Test]
		public void AddingSameQueueNameReturnsOriginalQueue()
		{
            set.Add("q1", new DefaultQueueConfiguration("q1"));
			IIntegrationQueue q = set["q1"];
            set.Add("q1", new DefaultQueueConfiguration("q1"));
			Assert.AreSame(q, set["q1"]);
		}

		[Test]
		public void RetrievingUnknownQueueNameReturnsNull()
		{
			Assert.IsNull(set["foo"]);
		}
	}

	[TestFixture]
	public class IntegrationQueueSetIntegrationTest
	{
		private const string TestQueueName = "ProjectQueueOne";
		private const string TestQueueName2 = "ProjectQueueTwo";

		private LatchMock queueNotifier1Mock;
		private LatchMock queueNotifier2Mock;
		private LatchMock project1Mock;
		private LatchMock project2Mock;
		private IntegrationQueueSet integrationQueues;
		private IIntegrationQueue integrationQueue1;
		private IIntegrationQueue integrationQueue2;
		private IntegrationRequest integrationRequest;
		private IIntegrationQueueItem integrationQueueItem1;
		private IIntegrationQueueItem integrationQueueItem2;

		[SetUp]
		public void SetUp()
		{
			integrationQueues = new IntegrationQueueSet();
            integrationQueues.Add(TestQueueName, new DefaultQueueConfiguration(TestQueueName));
            integrationQueues.Add(TestQueueName2, new DefaultQueueConfiguration(TestQueueName2));
			integrationQueue1 = integrationQueues[TestQueueName];
			integrationQueue2 = integrationQueues[TestQueueName2];

			integrationRequest = new IntegrationRequest(BuildCondition.ForceBuild, "Test");

			project1Mock = new LatchMock(typeof (IProject));
			project1Mock.Strict = true;
			project1Mock.SetupResult("Name", "ProjectOne");
			project1Mock.SetupResult("QueueName", TestQueueName);
			project1Mock.SetupResult("QueuePriority", 0);

			project2Mock = new LatchMock(typeof (IProject));
			project2Mock.Strict = true;
			project2Mock.SetupResult("Name", "ProjectTwo");
			project2Mock.SetupResult("QueueName", TestQueueName2);
			project2Mock.SetupResult("QueuePriority", 0);

			queueNotifier1Mock = new LatchMock(typeof (IIntegrationQueueNotifier));
			queueNotifier1Mock.Strict = true;

			queueNotifier2Mock = new LatchMock(typeof (IIntegrationQueueNotifier));
			queueNotifier2Mock.Strict = true;

			integrationQueueItem1 = new IntegrationQueueItem((IProject) project1Mock.MockInstance,
			                                                 integrationRequest, (IIntegrationQueueNotifier) queueNotifier1Mock.MockInstance);

			integrationQueueItem2 = new IntegrationQueueItem((IProject) project2Mock.MockInstance,
			                                                 integrationRequest, (IIntegrationQueueNotifier) queueNotifier2Mock.MockInstance);
		}

		private void VerifyAll()
		{
			queueNotifier1Mock.Verify();
			queueNotifier2Mock.Verify();
			project1Mock.Verify();
			project2Mock.Verify();
		}

		[Test]
		public void GetIntegrationQueueSnapshotForNoContent()
		{
			QueueSetSnapshot queueSetSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(queueSetSnapshot);
			Assert.AreEqual(2, queueSetSnapshot.Queues.Count);
            Assert.IsTrue(queueSetSnapshot.FindByName(TestQueueName).IsEmpty);
            Assert.IsTrue(queueSetSnapshot.FindByName(TestQueueName).IsEmpty);
		}

		[Test]
		public void GetIntegrationQueueSnapshotForNoProjectsStarted()
		{
			QueueSetSnapshot queueSetSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(queueSetSnapshot);
			Assert.AreEqual(2, queueSetSnapshot.Queues.Count);
		}

		[Test]
		public void GetIntegrationQueueSnapshotForProjectRegisteredButNotQueued()
		{
			QueueSetSnapshot queueSetSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(queueSetSnapshot);
			Assert.AreEqual(2, queueSetSnapshot.Queues.Count);
			VerifyAll();
		}

		[Test]
		public void GetIntegrationQueueSnapshotForSingleProjectOnSingleQueue()
		{
            project1Mock.SetupResult("CurrentActivity", ProjectActivity.CheckingModifications);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof (bool));
			integrationQueue1.Enqueue(integrationQueueItem1);

			QueueSetSnapshot queueSetSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(queueSetSnapshot);
			Assert.AreEqual(2, queueSetSnapshot.Queues.Count);

			QueueSnapshot queueSnapshot = queueSetSnapshot.Queues[0];
			Assert.IsNotNull(queueSnapshot);
            Assert.IsFalse(queueSnapshot.IsEmpty);
            Assert.AreEqual(TestQueueName, queueSnapshot.QueueName);
			Assert.AreEqual(1, queueSnapshot.Requests.Count);
            Assert.AreEqual(queueSnapshot, queueSetSnapshot.FindByName(TestQueueName));

			QueuedRequestSnapshot queuedRequestSnapshot = queueSnapshot.Requests[0];
            Assert.AreEqual("ProjectOne", queuedRequestSnapshot.ProjectName);
            Assert.AreEqual(ProjectActivity.CheckingModifications, queuedRequestSnapshot.Activity);

            QueueSnapshot queueSnapshot2 = queueSetSnapshot.Queues[1];
            Assert.IsNotNull(queueSnapshot2);
            Assert.IsTrue(queueSnapshot2.IsEmpty);

			VerifyAll();
		}

		[Test]
		public void GetIntegrationQueueSnapshotForMultipleProjectsOnSingleQueue()
		{
            project1Mock.SetupResult("CurrentActivity", ProjectActivity.CheckingModifications);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof (bool));
			integrationQueue1.Enqueue(integrationQueueItem1);

			// Second item is different project but same queue
			project2Mock.ExpectAndReturn("QueueName", TestQueueName);
            project2Mock.SetupResult("CurrentActivity", ProjectActivity.Pending);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof (bool));
			integrationQueue1.Enqueue(integrationQueueItem2);

			QueueSetSnapshot queueSetSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.AreEqual(2, queueSetSnapshot.Queues.Count);

			QueueSnapshot queueSnapshot = queueSetSnapshot.Queues[0];
			Assert.AreEqual(2, queueSnapshot.Requests.Count);

			QueuedRequestSnapshot firstQueuedRequestSnapshot = queueSnapshot.Requests[0];
			Assert.AreEqual("ProjectOne", firstQueuedRequestSnapshot.ProjectName);
            Assert.AreEqual(ProjectActivity.CheckingModifications, firstQueuedRequestSnapshot.Activity);

			QueuedRequestSnapshot secondQueuedRequestSnapshot = queueSnapshot.Requests[1];
			Assert.AreEqual("ProjectTwo", secondQueuedRequestSnapshot.ProjectName);
            Assert.AreEqual(ProjectActivity.Pending, secondQueuedRequestSnapshot.Activity);

			VerifyAll();
		}

		[Test]
		public void GetIntegrationQueueSnapshotForMultipleQueues()
		{
            project1Mock.SetupResult("CurrentActivity", ProjectActivity.CheckingModifications);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof (bool));
			integrationQueue1.Enqueue(integrationQueueItem1);

			// Second item is different project and different queue
            project2Mock.SetupResult("CurrentActivity", ProjectActivity.Pending);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof (bool));
			integrationQueue2.Enqueue(integrationQueueItem2);

			QueueSetSnapshot queueSetSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.AreEqual(2, queueSetSnapshot.Queues.Count);

			foreach (QueueSnapshot namedQueueSnapshot in queueSetSnapshot.Queues)
			{
				Assert.AreEqual(1, namedQueueSnapshot.Requests.Count);
			}

			QueueSnapshot firstQueueSnapshot = queueSetSnapshot.Queues[0];
			Assert.AreEqual(1, firstQueueSnapshot.Requests.Count);
			QueuedRequestSnapshot firstQueuedRequestSnapshot = firstQueueSnapshot.Requests[0];
			Assert.AreEqual("ProjectOne", firstQueuedRequestSnapshot.ProjectName);

			QueueSnapshot secondQueueSnapshot = queueSetSnapshot.Queues[1];
			Assert.AreEqual(1, secondQueueSnapshot.Requests.Count);
			QueuedRequestSnapshot secondQueuedRequestSnapshot = secondQueueSnapshot.Requests[0];
			Assert.AreEqual("ProjectTwo", secondQueuedRequestSnapshot.ProjectName);

			VerifyAll();
		}
	}
}
