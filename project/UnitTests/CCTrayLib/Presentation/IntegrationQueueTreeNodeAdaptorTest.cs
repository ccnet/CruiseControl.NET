using System.Windows.Forms;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Presentation;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Presentation
{
	[TestFixture]
	public class IntegrationQueueTreeNodeAdaptorTest
	{
		const string ServerUrl = @"tcp://blah:1000/";

		[Test]
		public void CanCreateListViewItem()
		{
			StubServerMonitor serverMonitor = new StubServerMonitor(ServerUrl);

			IntegrationQueueTreeNodeAdaptor adaptor = new IntegrationQueueTreeNodeAdaptor(serverMonitor);
			TreeNode item = adaptor.Create();

			Assert.AreEqual("blah:1000", item.Text);
			Assert.AreEqual(IntegrationQueueNodeType.RemotingServer.ImageIndex, item.ImageIndex);
		}

		[Test]
		public void CreateJustServerNodeWhenNoChildQueues()
		{
			StubServerMonitor serverMonitor = new StubServerMonitor(ServerUrl);
            serverMonitor.CruiseServerSnapshot = CreateNoQueuesSnapshot();

			IntegrationQueueTreeNodeAdaptor adaptor = new IntegrationQueueTreeNodeAdaptor(serverMonitor);
			TreeNode item = adaptor.Create();

			Assert.AreEqual(0, item.Nodes.Count);
		}

		[Test]
		public void WhenTheStateOfTheQueueChangesTheChildNodesOfTheServerNodeAreUpdated()
		{
			StubServerMonitor serverMonitor = new StubServerMonitor(ServerUrl);
            serverMonitor.CruiseServerSnapshot = CreateEmptyQueuesSnapshot();

			IntegrationQueueTreeNodeAdaptor adaptor = new IntegrationQueueTreeNodeAdaptor(serverMonitor);
			TreeNode item = adaptor.Create();

			Assert.AreEqual(2, item.Nodes.Count);
			TreeNode firstQueueNode = item.Nodes[0];
			TreeNode secondQueueNode = item.Nodes[1];

			Assert.AreEqual("Queue1", firstQueueNode.Text);
			Assert.AreEqual("Queue2", secondQueueNode.Text);

			Assert.AreEqual(0, firstQueueNode.Nodes.Count);
			Assert.AreEqual(0, secondQueueNode.Nodes.Count);
			Assert.AreEqual(IntegrationQueueNodeType.QueueEmpty.ImageIndex, firstQueueNode.ImageIndex);

			// Now lets add something to a queue.
            serverMonitor.CruiseServerSnapshot = CreatePopulatedQueuesSnapshot();

			serverMonitor.OnQueueChanged(new MonitorServerQueueChangedEventArgs(serverMonitor));

			firstQueueNode = item.Nodes[0];
			secondQueueNode = item.Nodes[1];
			Assert.AreEqual(2, firstQueueNode.Nodes.Count);
			Assert.AreEqual(0, secondQueueNode.Nodes.Count);

			TreeNode firstQueuedItemNode = firstQueueNode.Nodes[0];
			TreeNode secondQueuedItemNode = firstQueueNode.Nodes[1];

            Assert.AreEqual(IntegrationQueueNodeType.QueuePopulated.ImageIndex, firstQueueNode.ImageIndex);
			Assert.AreEqual(IntegrationQueueNodeType.CheckingModifications.ImageIndex, firstQueuedItemNode.ImageIndex);
            Assert.AreEqual(IntegrationQueueNodeType.PendingInQueue.ImageIndex, secondQueuedItemNode.ImageIndex);
		}

        private CruiseServerSnapshot CreateNoQueuesSnapshot()
		{
            return new CruiseServerSnapshot();
		}

        private CruiseServerSnapshot CreateEmptyQueuesSnapshot()
		{
            QueueSetSnapshot queueSetSnapshot = new QueueSetSnapshot();

			QueueSnapshot queueSnapshot1 = new QueueSnapshot("Queue1");
			queueSetSnapshot.Queues.Add(queueSnapshot1);

			QueueSnapshot queueSnapshot2 = new QueueSnapshot("Queue2");
			queueSetSnapshot.Queues.Add(queueSnapshot2);

            return new CruiseServerSnapshot(null, queueSetSnapshot);
        }

        private CruiseServerSnapshot CreatePopulatedQueuesSnapshot()
		{
            CruiseServerSnapshot cruiseServerSnapshot = CreateEmptyQueuesSnapshot();
            QueueSetSnapshot queueSetSnapshot = cruiseServerSnapshot.QueueSetSnapshot;

			QueueSnapshot queueSnapshot1 = queueSetSnapshot.Queues[0];

            QueuedRequestSnapshot queuedRequestSnapshot1 = new QueuedRequestSnapshot("Project1", ProjectActivity.CheckingModifications);
			queueSnapshot1.Requests.Add(queuedRequestSnapshot1);

            QueuedRequestSnapshot queuedRequestSnapshot2 = new QueuedRequestSnapshot("Project2", ProjectActivity.Pending);
			queueSnapshot1.Requests.Add(queuedRequestSnapshot2);

            return cruiseServerSnapshot;
        }
	}
}
