using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.UnitTests.UnitTestUtils;
using System;

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
		private IIntegrationQueue integrationQueueUseFirst;
        private IIntegrationQueue integrationQueueReAdd;
        private IIntegrationQueue integrationQueueReAddTop;
        private IIntegrationQueue integrationQueueReplace;
        private IntegrationRequest integrationRequestForceBuild;
        private IntegrationRequest integrationRequestIfModificationExists;
        private IIntegrationQueueItem integrationQueueItem1;
		private IIntegrationQueueItem integrationQueueItem2;
		private IIntegrationQueueItem integrationQueueItem3;
        private IIntegrationQueueItem integrationQueueItem4;

		[SetUp]
		public void SetUp()
		{
			integrationQueues = new IntegrationQueueSet();
            integrationQueues.Add(TestQueueName, new DefaultQueueConfiguration(TestQueueName));
			integrationQueueUseFirst = integrationQueues[TestQueueName];

            // Generate a queue to test re-adding
            string secondQueueName = "Test Queue #2";
            IQueueConfiguration readConfig = new DefaultQueueConfiguration(secondQueueName);
            readConfig.HandlingMode = QueueDuplicateHandlingMode.ApplyForceBuildsReAdd;
            integrationQueues.Add(secondQueueName, readConfig);
            integrationQueueReAdd = integrationQueues[secondQueueName];

            // Generate a queue to test re-adding to top
            var fourthQueueName = "Test Queue #4";
            var raddTopConfig = new DefaultQueueConfiguration(fourthQueueName);
            raddTopConfig.HandlingMode = QueueDuplicateHandlingMode.ApplyForceBuildsReAddTop;
            integrationQueues.Add(fourthQueueName, raddTopConfig);
            integrationQueueReAddTop = integrationQueues[fourthQueueName];

            // Generate a queue to test replacing
            string thirdQueueName = "Test Queue #3";
            IQueueConfiguration replaceConfig = new DefaultQueueConfiguration(thirdQueueName);
            replaceConfig.HandlingMode = QueueDuplicateHandlingMode.ApplyForceBuildsReplace;
            integrationQueues.Add(thirdQueueName, replaceConfig);
            integrationQueueReplace = integrationQueues[thirdQueueName];

            integrationRequestForceBuild = new IntegrationRequest(BuildCondition.ForceBuild, "Test", null);
            integrationRequestIfModificationExists = new IntegrationRequest(BuildCondition.IfModificationExists, "Test", null);
			
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
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);

			integrationQueueItem2 = new IntegrationQueueItem((IProject)project2Mock.MockInstance, 
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier2Mock.MockInstance);

			integrationQueueItem3 = new IntegrationQueueItem((IProject)project3Mock.MockInstance, 
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier3Mock.MockInstance);

            integrationQueueItem4 = new IntegrationQueueItem((IProject)project2Mock.MockInstance,
                integrationRequestIfModificationExists, (IIntegrationQueueNotifier)queueNotifier2Mock.MockInstance);
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
        public void HasConfig()
        {
            Assert.IsNotNull(integrationQueueUseFirst.Configuration);
        }
	
		[Test]
		public void FirstProjectOnQueueShouldIntegrateImmediately()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			string[] queueNames = integrationQueues.GetQueueNames();
			Assert.AreEqual(4, queueNames.Length);
			Assert.AreEqual(TestQueueName, queueNames[0]);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
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
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);
			// Add a second request for same project
			IIntegrationQueueItem secondQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(secondQueueItem);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
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
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);
			// Add a second request for same project
			IIntegrationQueueItem secondQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(secondQueueItem);
			// Try to add a third request for same project
			IIntegrationQueueItem thirdQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier1Mock.MockInstance);
			queueNotifier1Mock.ExpectNoCall("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(thirdQueueItem);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
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
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name
			project2Mock.SetupResult("QueueName", TestQueueName);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem2);

			string[] queueNames = integrationQueues.GetQueueNames();
			Assert.AreEqual(4, queueNames.Length);
			Assert.AreEqual(TestQueueName, queueNames[0]);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
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
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			integrationQueueUseFirst.Dequeue();

			string[] queueNames = integrationQueues.GetQueueNames();
			Assert.AreEqual(4, queueNames.Length);
			Assert.AreEqual(TestQueueName, queueNames[0]);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
			Assert.AreEqual(0, itemsOnQueue.Length);

			VerifyAll();
		}

		[Test]
		public void DequeueForMultipleQueuedItemsActivatesNextItemOnQueue()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyExitingIntegrationQueue", false);
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Force second project to be on same queue as the first.
			project2Mock.SetupResult("QueueName", TestQueueName);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem2);

			integrationQueueUseFirst.Dequeue();

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
			Assert.AreEqual(1, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[0]);

			VerifyAll();
		}

		[Test]
		public void RemoveProjectClearsOnlyItemsThatAreForThisProject()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.Expect("NotifyExitingIntegrationQueue", false);
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Second item is different project but same queue
			project2Mock.SetupResult("QueueName", TestQueueName);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem2);

			// Third item is same project as the first.
			IIntegrationQueueItem thirdQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier3Mock.MockInstance);
			queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier3Mock.Expect("NotifyExitingIntegrationQueue", true);
			integrationQueueUseFirst.Enqueue(thirdQueueItem);

			integrationQueueUseFirst.RemoveProject((IProject)project1Mock.MockInstance);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
			Assert.AreEqual(1, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[0]);

			VerifyAll();
		}

		[Test]
		public void RemovePendingRequestOnly()
		{
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier1Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Second item is same project
			IIntegrationQueueItem secondQueueItem = new IntegrationQueueItem((IProject)project1Mock.MockInstance, 
				integrationRequestForceBuild, (IIntegrationQueueNotifier)queueNotifier2Mock.MockInstance);

			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.Expect("NotifyExitingIntegrationQueue", true);
			integrationQueueUseFirst.Enqueue(secondQueueItem);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
			Assert.AreEqual(2, itemsOnQueue.Length);

			integrationQueueUseFirst.RemovePendingRequest((IProject)project1Mock.MockInstance);

			itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
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
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name and priority
			project2Mock.SetupResult("QueueName", TestQueueName);
			project2Mock.SetupResult("QueuePriority", 1);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem2);

			// Add a third project request for different project with same queue name and priority
			project3Mock.SetupResult("QueueName", TestQueueName);
			project3Mock.SetupResult("QueuePriority", 1);
			queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier3Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem3);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
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
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name
			project2Mock.SetupResult("QueueName", TestQueueName);
			project2Mock.SetupResult("QueuePriority", 0);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier2Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem2);

			// Add a third project request for different project with same queue name
			// Priority 1 should be before previous entry of priority 0
			project3Mock.SetupResult("QueueName", TestQueueName);
			project3Mock.SetupResult("QueuePriority", 1);
			queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
			queueNotifier3Mock.ExpectNoCall("NotifyExitingIntegrationQueue", typeof(bool));
			integrationQueueUseFirst.Enqueue(integrationQueueItem3);

			IIntegrationQueueItem[] itemsOnQueue = integrationQueueUseFirst.GetQueuedIntegrations();
			Assert.AreEqual(3, itemsOnQueue.Length);
			Assert.AreSame(integrationQueueItem1, itemsOnQueue[0]);
			Assert.AreSame(integrationQueueItem3, itemsOnQueue[1]);
			Assert.AreSame(integrationQueueItem2, itemsOnQueue[2]);

			VerifyAll();
		}

		[Test]
		public void GetNextRequestIsNullWithNothingOnQueue()
		{
			IntegrationRequest ir = integrationQueueUseFirst.GetNextRequest((IProject)project1Mock.MockInstance);
			Assert.IsNull(ir);
			VerifyAll();
		}

		[Test]
		public void GetNextRequestIsNullWhenFirstQueueItemIsDifferentProject()
		{
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			IntegrationRequest ir = integrationQueueUseFirst.GetNextRequest((IProject)project2Mock.MockInstance);
			Assert.IsNull(ir);
			VerifyAll();
		}

		[Test]
		public void GetNextRequestSucceedsWhenFirstQueueItemIsThisProject()
		{
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			IntegrationRequest ir = integrationQueueUseFirst.GetNextRequest((IProject)project1Mock.MockInstance);
			Assert.AreSame(ir, integrationRequestForceBuild);
			VerifyAll();
		}

		[Test]
		public void HasItemPendingOnQueueFalseWhenQueueIsEmpty()
		{
			bool hasItem = integrationQueueUseFirst.HasItemPendingOnQueue((IProject) project1Mock.MockInstance);
			Assert.IsFalse(hasItem);
			VerifyAll();
		}

		[Test]
		public void HasItemPendingOnQueueFalseWhenProjectNotOnQueue()
		{
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			bool hasItem = integrationQueueUseFirst.HasItemPendingOnQueue((IProject) project2Mock.MockInstance);
			Assert.IsFalse(hasItem);
			VerifyAll();
		}

		[Test]
		public void HasItemPendingOnQueueFalseWhenProjectIsJustIntegrating()
		{
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			bool hasItem = integrationQueueUseFirst.HasItemPendingOnQueue((IProject) project1Mock.MockInstance);
			Assert.IsFalse(hasItem);
			VerifyAll();
		}

		[Test]
		public void HasItemPendingOnQueueTrueWhenProjectIsQueued()
		{
			// Setup the first project request
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name
			project2Mock.SetupResult("QueueName", TestQueueName);
			project2Mock.SetupResult("QueuePriority", 0);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem2);

			bool hasItem = integrationQueueUseFirst.HasItemPendingOnQueue((IProject) project2Mock.MockInstance);
			Assert.IsTrue(hasItem);
			VerifyAll();
		}

		[Test]
		public void HasItemOnQueueFalseWhenQueueIsEmpty()
		{
			bool hasItem = integrationQueueUseFirst.HasItemOnQueue((IProject) project1Mock.MockInstance);
			Assert.IsFalse(hasItem);
			VerifyAll();
		}

		[Test]
		public void HasItemOnQueueFalseWhenProjectNotOnQueue()
		{
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			bool hasItem = integrationQueueUseFirst.HasItemOnQueue((IProject) project2Mock.MockInstance);
			Assert.IsFalse(hasItem);
			VerifyAll();
		}

		[Test]
		public void HasItemOnQueueTrueWhenProjectIsJustIntegrating()
		{
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			bool hasItem = integrationQueueUseFirst.HasItemOnQueue((IProject) project1Mock.MockInstance);
			Assert.IsTrue(hasItem);
			VerifyAll();
		}

		[Test]
		public void HasItemOnQueueTrueWhenProjectIsQueued()
		{
			// Setup the first project request
			project1Mock.SetupResult("QueuePriority", 1);
			queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem1);

			// Add a second project request for different project with same queue name
			project2Mock.SetupResult("QueueName", TestQueueName);
			project2Mock.SetupResult("QueuePriority", 0);
			queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
			integrationQueueUseFirst.Enqueue(integrationQueueItem2);

			bool hasItem = integrationQueueUseFirst.HasItemOnQueue((IProject) project2Mock.MockInstance);
			Assert.IsTrue(hasItem);
			VerifyAll();
		}

        [Test]
        public void AddingADuplicateProjectWithForceBuildReAdds()
        {
            // Setup the first project request
            project1Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project1Mock.SetupResult("QueuePriority", 1);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAdd.Enqueue(integrationQueueItem1);

            // Add a second project request for different project with same queue name
            project2Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAdd.Enqueue(integrationQueueItem4);

            // Add a third project request for different project with same queue name
            project3Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project3Mock.SetupResult("QueuePriority", 0);
            queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAdd.Enqueue(integrationQueueItem3);

            // Now add the second project request again, but with a force build
            project2Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            queueNotifier2Mock.Expect("NotifyExitingIntegrationQueue", true);
            integrationQueueReAdd.Enqueue(integrationQueueItem2);

            // Check the queued items
            IIntegrationQueueItem[] queuedItems = integrationQueueReAdd.GetQueuedIntegrations();
            Assert.AreEqual(integrationQueueItem3, queuedItems[1], "Integration item #1 is incorrect");
            Assert.AreEqual(integrationQueueItem2, queuedItems[2], "Integration item #2 is incorrect");
            VerifyAll();
        }

        [Test]
        public void AddingADuplicateProjectWithForceBuildReAddsToTop()
        {
            // Setup the first project request
            project1Mock.SetupResult("QueueName", integrationQueueReAddTop.Name);
            project1Mock.SetupResult("QueuePriority", 1);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAddTop.Enqueue(integrationQueueItem1);

            // Add a second project request for different project with same queue name
            project2Mock.SetupResult("QueueName", integrationQueueReAddTop.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAddTop.Enqueue(integrationQueueItem4);

            // Add a third project request for different project with same queue name
            project3Mock.SetupResult("QueueName", integrationQueueReAddTop.Name);
            project3Mock.SetupResult("QueuePriority", 0);
            queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAddTop.Enqueue(integrationQueueItem3);

            // Now add the second project request again, but with a force build
            project2Mock.SetupResult("QueueName", integrationQueueReAddTop.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            queueNotifier2Mock.Expect("NotifyExitingIntegrationQueue", true);
            integrationQueueReAddTop.Enqueue(integrationQueueItem2);

            // Check the queued items
            IIntegrationQueueItem[] queuedItems = integrationQueueReAddTop.GetQueuedIntegrations();
            Assert.AreEqual(integrationQueueItem2, queuedItems[1], "Integration item #1 is incorrect");
            Assert.AreEqual(integrationQueueItem3, queuedItems[2], "Integration item #2 is incorrect");
            VerifyAll();
        }

        [Test]
        public void AddingADuplicateProjectWithForceBuildReplaces()
        {
            // Setup the first project request
            project1Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project1Mock.SetupResult("QueuePriority", 1);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReplace.Enqueue(integrationQueueItem1);

            // Add a second project request for different project with same queue name
            project2Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReplace.Enqueue(integrationQueueItem4);

            // Add a third project request for different project with same queue name
            project3Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project3Mock.SetupResult("QueuePriority", 0);
            queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReplace.Enqueue(integrationQueueItem3);

            // Now add the second project request again, but with a force build
            project2Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            queueNotifier2Mock.Expect("NotifyExitingIntegrationQueue", true);
            integrationQueueReplace.Enqueue(integrationQueueItem2);

            // Check the queued items
            IIntegrationQueueItem[] queuedItems = integrationQueueReplace.GetQueuedIntegrations();
            Assert.AreEqual(integrationQueueItem2, queuedItems[1], "Integration item #1 is incorrect");
            Assert.AreEqual(integrationQueueItem3, queuedItems[2], "Integration item #2 is incorrect");
            VerifyAll();
        }

        [Test]
        public void AddingADuplicateProjectWithIfModificationExistsDoesNotReAdd()
        {
            // Setup the first project request
            project1Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project1Mock.SetupResult("QueuePriority", 1);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAdd.Enqueue(integrationQueueItem1);

            // Add a second project request for different project with same queue name
            project2Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAdd.Enqueue(integrationQueueItem4);

            // Add a third project request for different project with same queue name
            project3Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project3Mock.SetupResult("QueuePriority", 0);
            queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReAdd.Enqueue(integrationQueueItem3);

            // Now add the second project request again
            project2Mock.SetupResult("QueueName", integrationQueueReAdd.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            integrationQueueReAdd.Enqueue(integrationQueueItem4);

            // Check the queued items
            IIntegrationQueueItem[] queuedItems = integrationQueueReAdd.GetQueuedIntegrations();
            Assert.AreEqual(integrationQueueItem4, queuedItems[1], "Integration item #1 is incorrect");
            Assert.AreEqual(integrationQueueItem3, queuedItems[2], "Integration item #2 is incorrect");
            VerifyAll();
        }

        [Test]
        public void AddingADuplicateProjectWithIfModificationExistsDoesNotReplace()
        {
            // Setup the first project request
            project1Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project1Mock.SetupResult("QueuePriority", 1);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReplace.Enqueue(integrationQueueItem1);

            // Add a second project request for different project with same queue name
            project2Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            queueNotifier2Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReplace.Enqueue(integrationQueueItem4);

            // Add a third project request for different project with same queue name
            project3Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project3Mock.SetupResult("QueuePriority", 0);
            queueNotifier3Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReplace.Enqueue(integrationQueueItem3);

            // Now add the second project request again
            project2Mock.SetupResult("QueueName", integrationQueueReplace.Name);
            project2Mock.SetupResult("QueuePriority", 0);
            integrationQueueReplace.Enqueue(integrationQueueItem4);

            // Check the queued items
            IIntegrationQueueItem[] queuedItems = integrationQueueReplace.GetQueuedIntegrations();
            Assert.AreEqual(integrationQueueItem4, queuedItems[1], "Integration item #1 is incorrect");
            Assert.AreEqual(integrationQueueItem3, queuedItems[2], "Integration item #2 is incorrect");
            VerifyAll();
        }

        [Test]
        public void ProveThatQueueLocksBehaveCorrectlyWhenAcquiredAndReleased()
        {
            // configure and prove a basic scenario
            // queue 0 should lock queues 1 + 2 when buildling
            // queues 1 + 2 should lock queue 0 when building
            string[] queues = integrationQueues.GetQueueNames();
            integrationQueues[queues[0]].Configuration.LockQueueNames = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0},{1}", queues[1], queues[2]);
            integrationQueues[queues[1]].Configuration.LockQueueNames = queues[0];
            integrationQueues[queues[2]].Configuration.LockQueueNames = queues[0];

            // to test these locks before we involve projects, toggle locks on each in term and verify that the correct queues are locked

            // starting position
            Assert.IsFalse(integrationQueues[queues[0]].IsBlocked, "Initial state of Queue0 should be unblocked");
            Assert.IsFalse(integrationQueues[queues[1]].IsBlocked, "Initial state of Queue1 should be unblocked");
            Assert.IsFalse(integrationQueues[queues[2]].IsBlocked, "Initial state of Queue2 should be unblocked");

            // Toggle locks on #0 (acquire)
            IDisposable lock1, lock2;
            Assert.IsTrue(integrationQueues[queues[0]].TryLock(out lock1));
            Assert.IsFalse(integrationQueues[queues[0]].IsBlocked, "State of Queue0 should be unlocked");
            Assert.IsTrue(integrationQueues[queues[1]].IsBlocked, "State of Queue1 should be locked");
            Assert.IsTrue(integrationQueues[queues[2]].IsBlocked, "State of Queue2 should be locked");

            // Toggle locks on #0 (release)
            lock1.Dispose();
            Assert.IsFalse(integrationQueues[queues[0]].IsBlocked, "State of Queue0 should be unlocked");
            Assert.IsFalse(integrationQueues[queues[1]].IsBlocked, "State of Queue1 should be unlocked");
            Assert.IsFalse(integrationQueues[queues[2]].IsBlocked, "State of Queue2 should be unlocked");

            // Toggle locks on #1 (acquire)
            Assert.IsTrue(integrationQueues[queues[1]].TryLock(out lock1));
            Assert.IsTrue(integrationQueues[queues[0]].IsBlocked, "State of Queue0 should be locked");
            Assert.IsFalse(integrationQueues[queues[1]].IsBlocked, "State of Queue1 should be unlocked");
            Assert.IsFalse(integrationQueues[queues[2]].IsBlocked, "State of Queue2 should be unlocked");

            // Toggle locks on #2 (acquire) - this means 2 queues have a lock on #0, therefore both must be released
            Assert.IsTrue(integrationQueues[queues[2]].TryLock(out lock2));
            Assert.IsTrue(integrationQueues[queues[0]].IsBlocked, "State of Queue0 should be locked");
            Assert.IsFalse(integrationQueues[queues[1]].IsBlocked, "State of Queue1 should be unlocked");
            Assert.IsFalse(integrationQueues[queues[2]].IsBlocked, "State of Queue2 should be unlocked");

            // Toggle locks on #1 (release) - #0 should remain locked
            lock1.Dispose();
            Assert.IsTrue(integrationQueues[queues[0]].IsBlocked, "State of Queue0 should be locked");
            Assert.IsFalse(integrationQueues[queues[1]].IsBlocked, "State of Queue1 should be unlocked");
            Assert.IsFalse(integrationQueues[queues[2]].IsBlocked, "State of Queue2 should be unlocked");

            // Toggle locks on #2 (release) - #0 should remain locked
            lock2.Dispose();
            Assert.IsFalse(integrationQueues[queues[0]].IsBlocked, "State of Queue0 should be unlocked");
            Assert.IsFalse(integrationQueues[queues[1]].IsBlocked, "State of Queue1 should be unlocked");
            Assert.IsFalse(integrationQueues[queues[2]].IsBlocked, "State of Queue2 should be unlocked");
        }

        [Test]
        public void ProjectsInQueueShouldNotIntegrateIfQueueIsLocked()
        {
            // ensure we have the correct setup for testing
            string[] queues = integrationQueues.GetQueueNames();
            integrationQueues[queues[0]].Configuration.LockQueueNames = string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0},{1}", queues[1], queues[2]);
            integrationQueues[queues[1]].Configuration.LockQueueNames = queues[0];
            integrationQueues[queues[2]].Configuration.LockQueueNames = queues[0];

            // now lock one of the queues and add projects to it
            IDisposable lock1;
            Assert.IsTrue(integrationQueues[queues[0]].TryLock(out lock1));

            project1Mock.SetupResult("QueueName", queues[1]);
            project1Mock.SetupResult("QueuePriority", 1);
            queueNotifier1Mock.Expect("NotifyEnteringIntegrationQueue");
            integrationQueueReplace.Enqueue(integrationQueueItem1);

            Assert.IsNull(integrationQueueReplace.GetNextRequest((IProject)project1Mock.MockInstance), "Expected no next request, as queue is locked");

            lock1.Dispose();

            IntegrationRequest next = integrationQueueReplace.GetNextRequest((IProject)project1Mock.MockInstance);
            Assert.IsNotNull(next, "Expected next request as queue lock has been released");
        }
    }
}
