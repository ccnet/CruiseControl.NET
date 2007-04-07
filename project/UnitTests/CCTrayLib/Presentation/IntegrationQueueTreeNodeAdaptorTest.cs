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
			serverMonitor.IntegrationQueueSnapshot = CreateNoQueuesSnapshot();

			IntegrationQueueTreeNodeAdaptor adaptor = new IntegrationQueueTreeNodeAdaptor(serverMonitor);
			TreeNode item = adaptor.Create();

			Assert.AreEqual(0, item.Nodes.Count);
		}

		[Test]
		public void WhenTheStateOfTheQueueChangesTheChildNodesOfTheServerNodeAreUpdated()
		{
			StubServerMonitor serverMonitor = new StubServerMonitor(ServerUrl);
			serverMonitor.IntegrationQueueSnapshot = CreateEmptyQueuesSnapshot();

			IntegrationQueueTreeNodeAdaptor adaptor = new IntegrationQueueTreeNodeAdaptor(serverMonitor);
			TreeNode item = adaptor.Create();

			Assert.AreEqual(2, item.Nodes.Count);
			TreeNode firstQueueNode = item.Nodes[0];
			TreeNode secondQueueNode = item.Nodes[1];

			Assert.AreEqual("Queue1", firstQueueNode.Text);
			Assert.AreEqual("Queue2", secondQueueNode.Text);

			Assert.AreEqual(0, firstQueueNode.Nodes.Count);
			Assert.AreEqual(0, secondQueueNode.Nodes.Count);
			Assert.AreEqual(IntegrationQueueNodeType.Queue.ImageIndex, firstQueueNode.ImageIndex);

			// Now lets add something to a queue.
			serverMonitor.IntegrationQueueSnapshot = CreatePopulatedQueuesSnapshot();

			serverMonitor.OnQueueChanged(new MonitorServerQueueChangedEventArgs(serverMonitor));

			firstQueueNode = item.Nodes[0];
			secondQueueNode = item.Nodes[1];
			Assert.AreEqual(2, firstQueueNode.Nodes.Count);
			Assert.AreEqual(0, secondQueueNode.Nodes.Count);

			TreeNode firstQueuedItemNode = firstQueueNode.Nodes[0];
			TreeNode secondQueuedItemNode = firstQueueNode.Nodes[1];

			Assert.AreEqual(IntegrationQueueNodeType.FirstInQueue.ImageIndex, firstQueuedItemNode.ImageIndex);
			Assert.AreEqual(IntegrationQueueNodeType.PendingInQueue.ImageIndex, secondQueuedItemNode.ImageIndex);
		}

		private IntegrationQueueSnapshot CreateNoQueuesSnapshot()
		{
			return new IntegrationQueueSnapshot();
		}

		private IntegrationQueueSnapshot CreateEmptyQueuesSnapshot()
		{
			IntegrationQueueSnapshot snapshot = CreateNoQueuesSnapshot();

			NamedQueueSnapshot namedQueueSnapshot1 = new NamedQueueSnapshot("Queue1");
			snapshot.Queues.Add(namedQueueSnapshot1);

			NamedQueueSnapshot namedQueueSnapshot2 = new NamedQueueSnapshot("Queue2");
			snapshot.Queues.Add(namedQueueSnapshot2);

			return snapshot;
		}

		private IntegrationQueueSnapshot CreatePopulatedQueuesSnapshot()
		{
			IntegrationQueueSnapshot snapshot = CreateEmptyQueuesSnapshot();

			NamedQueueSnapshot namedQueueSnapshot1 = snapshot.Queues[0];

			QueuedItemSnapshot queuedItemSnapshot1 = new QueuedItemSnapshot(
				namedQueueSnapshot1.QueueName, "Project1", 0, BuildCondition.ForceBuild, "Test");
			namedQueueSnapshot1.Items.Add(queuedItemSnapshot1);

			QueuedItemSnapshot queuedItemSnapshot2 = new QueuedItemSnapshot(
				namedQueueSnapshot1.QueueName, "Project2", 0, BuildCondition.ForceBuild, "Test");
			namedQueueSnapshot1.Items.Add(queuedItemSnapshot2);

			return snapshot;
		}
	}
}
