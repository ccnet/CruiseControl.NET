using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Queues
{
	[TestFixture]
	public class IntegrationQueueTest
	{
		private const string TestQueueName = "ProjectQueueOne";

		private LatchMock queueNotifier1Mock;
		private LatchMock queueNotifier2Mock;
		private LatchMock queueNotifier3Mock;
		private LatchMock project1Mock;
		private LatchMock project2Mock;
		private LatchMock project3Mock;
		private IntegrationQueueSet integrationQueues;
		private IIntegrationQueue integrationQueue;
		private IntegrationRequest integrationRequest;
		private IIntegrationQueueItem integrationQueueItem1;
		private IIntegrationQueueItem integrationQueueItem2;
		private IIntegrationQueueItem integrationQueueItem3;

		[SetUp]
		public void SetUp()
		{
			integrationQueues = new IntegrationQueueSet();
			integrationQueues.Add(TestQueueName);
			integrationQueue = integrationQueues[TestQueueName];

			integrationRequest = new IntegrationRequest(BuildCondition.ForceBuild, "Test");
			
			project1Mock = new LatchMock(typeof (IProject));
			project1Mock.Strict = true;
			project1Mock.SetupResult("Name", "ProjectOne");
			project1Mock.SetupResult("QueueName", TestQueueName);
			project1Mock.SetupResult("QueuePriority", 0);
			
			project2Mock = new LatchMock(typeof (IProject));
			project2Mock.Strict = true;
			project2Mock.SetupResult("Name", "ProjectTwo");
			project2Mock.SetupResult("QueueName", TestQueueName);
			project2Mock.SetupResult("QueuePriority", 0);
			
			project3Mock = new LatchMock(typeof (IProject));
			project3Mock.Strict = true;
			project3Mock.SetupResult("Name", "ProjectThree");
			project3Mock.SetupResult("QueueName", TestQueueName);
			project3Mock.SetupResult("QueuePriority", 1);

			queueNotifier1Mock = new LatchMock(typeof(IIntegrationQueueNotifier));
			queueNotifier1Mock.Strict = true;

			queueNotifier2Mock = new LatchMock(typeof(IIntegrationQueueNotifier));
			queueNotifier2Mock.Strict = true;

			queueNotifier3Mock = new LatchMock(typeof(IIntegrationQueueNotifier));
			queueNotifier3Mock.Strict = true;

			integrationQueueItem1 = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);

			integrationQueueItem2 = new IntegrationQueueItem((IProject)project2Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier2Mock.MockInstance);

			integrationQueueItem3 = new IntegrationQueueItem((IProject)project3Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier3Mock.MockInstance);
		}

		private void VerifyAll()
		{
			queueNotifier1Mock.Verify();
			queueNotifier2Mock.Verify();
			queueNotifier3Mock.Verify();
			project1Mock.Verify();
			project2Mock.Verify();
			project3Mock.Verify();
		}
	
		[Test]
		public void FirstProjectOnQueueShouldIntegrateImmediately()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem1);

			string[] queueNames = integrationQueues.GetQueueNames();
			Assert.AreEqual(1, queueNames.Length);
			Assert.AreEqual(TestQueueName, queueNames[0]);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(1, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);

			VerifyAll();
		}

		[Test]
		public void SecondIntegrationRequestForQueuedProjectShouldNotStartImmediately()
		{
			// Setup the first request
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem1);
			// Add a second request for same project
			IIntegrationQueueItem secondQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(secondQueueItem);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(2, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);
			Assert.AreSame(secondQueueItem, itemsOnQueue[1]);
			
			VerifyAll();
		}

		[Test]
		public void NoMoreThanOnePendingIntegrationForAProject()
		{
			// Setup the first request
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem1);
			// Add a second request for same project
			IIntegrationQueueItem secondQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(secondQueueItem);
			// Try to add a third request for same project
			IIntegrationQueueItem thirdQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);
			queueNotifier1Mock.ExpectNoCall("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(thirdQueueItem);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(2, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);
			Assert.AreSame(secondQueueItem, itemsOnQueue[1]);
			
			VerifyAll();
		}
	
		[Test]
		public void TwoProjectsWithSameQueueNameShouldShareQueue()
		{
			// Setup the first project request
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name
			project2Mock.SetupResult("QueueName", TestQueueName);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem2);

			string[] queueNames = integrationQueues.GetQueueNames();
			Assert.AreEqual(1, queueNames.Length);
			Assert.AreEqual(TestQueueName, queueNames[0]);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(2, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[1]);

			VerifyAll();
		}

		[Test]
		public void DequeueForSingleQueuedItemClearsQueue()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyExitingIntegrationQueue", false);
			integrationQueue.Enqueue(integrationQueueItem1);

			integrationQueue.Dequeue();

			string[] queueNames = integrationQueues.GetQueueNames();
			Assert.AreEqual(1, queueNames.Length);
			Assert.AreEqual(TestQueueName, queueNames[0]);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(0, itemsOnQueue.Length);

			VerifyAll();
		}

		[Test]
		public void DequeueForMultipleQueuedItemsActivatesNextItemOnQueue()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyExitingIntegrationQueue", false);
			integrationQueue.Enqueue(integrationQueueItem1);

			// Force second project to be on same queue as the first.
			project2Mock.SetupResult("QueueName", TestQueueName);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem2);

			integrationQueue.Dequeue();

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(1, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[0]);

			VerifyAll();
		}

		[Test]
		public void RemoveProjectClearsOnlyItemsThatAreForThisProject()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyExitingIntegrationQueue", false);
			integrationQueue.Enqueue(integrationQueueItem1);

			// Second item is different project but same queue
			project2Mock.SetupResult("QueueName", TestQueueName);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem2);

			// Third item is same project as the first.
			IIntegrationQueueItem thirdQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier3Mock.MockInstance);
			queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier3Mock.Expect("NotifyExitingIntegrationQueue", true);
			integrationQueue.Enqueue(thirdQueueItem);

			integrationQueue.RemoveProject((IProject)project1Mock.MockInstance);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(1, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[0]);

			VerifyAll();
		}

		[Test]
		public void RemovePendingRequestOnly()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem1);

			// Second item is same project
			IIntegrationQueueItem secondQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequest, (IIntegrationQueueNotifier)queueNotifier2Mock.MockInstance);

			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.Expect("NotifyExitingIntegrationQueue", true);
			integrationQueue.Enqueue(secondQueueItem);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(2, itemsOnQueue.Length);

			integrationQueue.RemovePendingRequest((IProject)project1Mock.MockInstance);

			itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(1, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);

			VerifyAll();
		}
	
		[Test]
		public void ProjectsWithSamePriorityShouldBeInEntryOrder()
		{
			// Setup the first project request
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name and priority
			project2Mock.SetupResult("QueueName", TestQueueName);
			project2Mock.SetupResult("QueuePriority", 1);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem2);

			// Add a third project request for different project with same queue name and priority
			project3Mock.SetupResult("QueueName", TestQueueName);
			project3Mock.SetupResult("QueuePriority", 1);
			queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier3Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem3);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(3, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[1]);
			Assert.AreSame(integrationQueueItem3, itemsOnQueue[2]);

			VerifyAll();
		}
	
		[Test]
		public void ProjectsWithNonZeroPriorityInFrontOfZeroPriority()
		{
			// Setup the first project request
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name
			project2Mock.SetupResult("QueueName", TestQueueName);
			project2Mock.SetupResult("QueuePriority", 0);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem2);

			// Add a third project request for different project with same queue name
			// Priority 1 should be before previous entry of priority 0
			project3Mock.SetupResult("QueueName", TestQueueName);
			project3Mock.SetupResult("QueuePriority", 1);
			queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier3Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueue.Enqueue(integrationQueueItem3);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueue.GetQueuedIntegrations();
			Assert.AreEqual(3, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);
			Assert.AreSame(integrationQueueItem3, itemsOnQueue[1]);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[2]);

			VerifyAll();
		}
	}
}
