using System;
using System.Threading;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
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
			set.Add("q1");
			IIntegrationQueue q = set["q1"];
			Assert.IsNotNull(q);
		}	

		[Test]
		public void AddingSameQueueNameReturnsOriginalQueue()
		{
			set.Add("q1");
			IIntegrationQueue q = set["q1"];
			set.Add("q1");
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
			integrationQueues.Add(TestQueueName);
			integrationQueues.Add(TestQueueName2);
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
			
			queueNotifier1Mock = new LatchMock(typeof(IIntegrationQueueNotifier));
			queueNotifier1Mock.Strict = true;

			queueNotifier2Mock = new LatchMock(typeof(IIntegrationQueueNotifier));
			queueNotifier2Mock.Strict = true;

			integrationQueueItem1 = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);

			integrationQueueItem2 = new IntegrationQueueItem((IProject)project2Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier2Mock.MockInstance);
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
			IntegrationQueueSnapshot integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(integrationQueueSnapshot);
			Assert.AreEqual(0, integrationQueueSnapshot.Queues.Count);
			Assert.IsNull(integrationQueueSnapshot.Queues[TestQueueName]);
		}

		[Test, Ignore("owen: removing caching for now - apr 7,2007")]
		public void GetIntegrationQueueSnapshotIsCachedUntilChanges()
		{
			IntegrationQueueSnapshot integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			DateTime lastSnapshotTime = integrationQueueSnapshot.TimeStamp;

			Thread.Sleep(20);

			integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.AreEqual(lastSnapshotTime.Ticks, integrationQueueSnapshot.TimeStamp.Ticks);

			Thread.Sleep(20);

			// Change the queue contents by kicking off an integration.
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyIntegrationToCommence", integrationQueueItem1);
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue1.Enqueue(integrationQueueItem1);

			lastSnapshotTime = integrationQueueSnapshot.TimeStamp;
			integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.AreNotEqual(lastSnapshotTime.Ticks, (decimal)integrationQueueSnapshot.TimeStamp.Ticks);
			Assert.IsTrue(lastSnapshotTime != integrationQueueSnapshot.TimeStamp);
		}

		[Test]
		public void GetIntegrationQueueSnapshotForNoProjectsStarted()
		{
			IntegrationQueueSnapshot integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(integrationQueueSnapshot);
			Assert.AreEqual(0, integrationQueueSnapshot.Queues.Count);
		}

		[Test]
		public void GetIntegrationQueueSnapshotForProjectRegisteredButNotQueued()
		{
			project1Mock.ExpectAndReturn("Triggers", null);
			IProjectIntegrator projectIntegrator = new ProjectIntegrator((IProject)project1Mock.MockInstance, this.integrationQueue1);

			IntegrationQueueSnapshot integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(integrationQueueSnapshot);
			Assert.AreEqual(0, integrationQueueSnapshot.Queues.Count);

			VerifyAll();
		}

		[Test]
		public void GetIntegrationQueueSnapshotForSingleProjectOnSingleQueue()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyIntegrationToCommence", integrationQueueItem1);
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue1.Enqueue(integrationQueueItem1);

			IntegrationQueueSnapshot integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.IsNotNull(integrationQueueSnapshot);
			Assert.AreEqual(1, integrationQueueSnapshot.Queues.Count);
			
			NamedQueueSnapshot namedQueueSnapshot = integrationQueueSnapshot.Queues[0];
			Assert.IsNotNull(namedQueueSnapshot);
			Assert.AreEqual(TestQueueName, namedQueueSnapshot.QueueName);
			Assert.AreEqual(1, namedQueueSnapshot.Items.Count);
			Assert.AreEqual(namedQueueSnapshot, integrationQueueSnapshot.Queues[TestQueueName]);
			
			QueuedItemSnapshot queuedItemSnapshot = namedQueueSnapshot.Items[0];
			Assert.AreEqual(TestQueueName, queuedItemSnapshot.QueueName);
			Assert.AreEqual("ProjectOne", queuedItemSnapshot.ProjectName);
			Assert.AreEqual(0, queuedItemSnapshot.QueuePriority);
			Assert.AreEqual("Test", queuedItemSnapshot.RequestSource);
			Assert.AreEqual(BuildCondition.ForceBuild, queuedItemSnapshot.BuildCondition);

			VerifyAll();
		}

		[Test]
		public void GetIntegrationQueueSnapshotForMultipleProjectsOnSingleQueue()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyIntegrationToCommence", integrationQueueItem1);
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue1.Enqueue(integrationQueueItem1);

			// Second item is different project but same queue
			project2Mock.ExpectAndReturn("QueueName", TestQueueName);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyIntegrationToCommence", typeof(IntegrationQueueItem));
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue1.Enqueue(integrationQueueItem2);

			IntegrationQueueSnapshot integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.AreEqual(1, integrationQueueSnapshot.Queues.Count);
			
			NamedQueueSnapshot namedQueueSnapshot = integrationQueueSnapshot.Queues[0];
			Assert.AreEqual(2, namedQueueSnapshot.Items.Count);
			
			QueuedItemSnapshot firstQueuedItemSnapshot = namedQueueSnapshot.Items[0];
			Assert.AreEqual("ProjectOne", firstQueuedItemSnapshot.ProjectName);
			
			QueuedItemSnapshot secondQueuedItemSnapshot = namedQueueSnapshot.Items[1];
			Assert.AreEqual("ProjectTwo", secondQueuedItemSnapshot.ProjectName);

			foreach (QueuedItemSnapshot itemSnapshot in namedQueueSnapshot.Items)
			{
				Assert.AreEqual(TestQueueName, itemSnapshot.QueueName);
			}

			VerifyAll();
		}

		[Test]
		public void GetIntegrationQueueSnapshotForMultipleQueues()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyIntegrationToCommence", integrationQueueItem1);
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue1.Enqueue(integrationQueueItem1);

			// Second item is different project and different queue
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.Expect("NotifyIntegrationToCommence", integrationQueueItem2);
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue2.Enqueue(integrationQueueItem2);

			IntegrationQueueSnapshot integrationQueueSnapshot = integrationQueues.GetIntegrationQueueSnapshot();
			Assert.AreEqual(2, integrationQueueSnapshot.Queues.Count);
			
			foreach (NamedQueueSnapshot namedQueueSnapshot in integrationQueueSnapshot.Queues)
			{
				Assert.AreEqual(1, namedQueueSnapshot.Items.Count);
			}

			NamedQueueSnapshot firstNamedQueueSnapshot = integrationQueueSnapshot.Queues[0];
			Assert.AreEqual(1, firstNamedQueueSnapshot.Items.Count);
			QueuedItemSnapshot firstQueuedItemSnapshot = firstNamedQueueSnapshot.Items[0];
			Assert.AreEqual("ProjectOne", firstQueuedItemSnapshot.ProjectName);
			
			NamedQueueSnapshot secondNamedQueueSnapshot = integrationQueueSnapshot.Queues[1];
			Assert.AreEqual(1, secondNamedQueueSnapshot.Items.Count);
			QueuedItemSnapshot secondQueuedItemSnapshot = secondNamedQueueSnapshot.Items[0];
			Assert.AreEqual("ProjectTwo", secondQueuedItemSnapshot.ProjectName);
			
			VerifyAll();
		}
	}
}
