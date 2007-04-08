using System;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Queues;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Queues
{
	[TestFixture]
	public class IntegrationQueueItemTest
	{
		[Test]
		public void HasAttributesAssignedCorrectly()
		{
			IProject project = new Project();
			IntegrationRequest integrationRequest = new IntegrationRequest(BuildCondition.NoBuild, "Test");
			IIntegrationQueueNotifier integrationQueueNotifier = new TestIntegrationQueueCallback();

			IIntegrationQueueItem integrationQueueItem = new IntegrationQueueItem(project, integrationRequest, integrationQueueNotifier);

			Assert.AreEqual(project, integrationQueueItem.Project);
			Assert.AreEqual(integrationRequest, integrationQueueItem.IntegrationRequest);
			Assert.AreEqual(integrationQueueNotifier, integrationQueueItem.IntegrationQueueNotifier);
		}

		private class TestIntegrationQueueCallback : IIntegrationQueueNotifier
		{
			public void NotifyEnteringIntegrationQueue()
			{
				throw new NotImplementedException();
			}

			public void NotifyExitingIntegrationQueue(bool isPendingItemCancelled)
			{
				throw new NotImplementedException();
			}
		}
	}
}
