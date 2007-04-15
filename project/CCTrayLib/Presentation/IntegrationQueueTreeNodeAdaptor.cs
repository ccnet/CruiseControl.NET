using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class IntegrationQueueTreeNodeAdaptor
	{
		private readonly TreeNode serverTreeNode;
		private readonly ISingleServerMonitor serverMonitor;

		public IntegrationQueueTreeNodeAdaptor(ISingleServerMonitor serverMonitor)
		{
			this.serverMonitor = serverMonitor;
			serverTreeNode = new TreeNode();
			serverTreeNode.Tag = new IntegrationQueueTreeNodeTag(this);
		}

		public TreeNode Create()
		{
			serverMonitor.QueueChanged += new MonitorServerQueueChangedEventHandler(ServerMonitor_QueueChanged);

			serverTreeNode.Text = serverMonitor.DisplayName;
			if (serverMonitor.Transport == BuildServerTransport.Remoting)
			{
				serverTreeNode.ImageIndex = serverTreeNode.SelectedImageIndex = IntegrationQueueNodeType.RemotingServer.ImageIndex;
			}
			else
			{
				serverTreeNode.ImageIndex = serverTreeNode.SelectedImageIndex = IntegrationQueueNodeType.HttpServer.ImageIndex;
			}

			DisplayIntegrationQueueInTreeViewNode();

			return serverTreeNode;
		}

		public void Detach()
		{
			serverMonitor.QueueChanged -= new MonitorServerQueueChangedEventHandler(ServerMonitor_QueueChanged);
		}

		public ISingleServerMonitor Monitor
		{
			get { return serverMonitor; }
		}

		private void ServerMonitor_QueueChanged(object sauce, MonitorServerQueueChangedEventArgs args)
		{
			DisplayIntegrationQueueInTreeViewNode();
		}

		private void DisplayIntegrationQueueInTreeViewNode()
		{
			serverTreeNode.Nodes.Clear();

		    CruiseServerSnapshot cruiseServerSnapshot = serverMonitor.CruiseServerSnapshot;
            if (cruiseServerSnapshot == null)
                return;
            QueueSetSnapshot queueSetSnapshot = serverMonitor.CruiseServerSnapshot.QueueSetSnapshot;

            if (queueSetSnapshot != null && queueSetSnapshot.Queues.Count > 0)
			{
				foreach (QueueSnapshot namedQueueSnapshot in queueSetSnapshot.Queues)
				{
					TreeNode queueNode = new TreeNode(namedQueueSnapshot.QueueName, IntegrationQueueNodeType.Queue.ImageIndex, IntegrationQueueNodeType.Queue.ImageIndex);
					queueNode.Tag = new IntegrationQueueTreeNodeTag(this, namedQueueSnapshot);
					serverTreeNode.Nodes.Add(queueNode);

					for (int index = 0; index < namedQueueSnapshot.Requests.Count; index++)
					{
						QueuedRequestSnapshot queuedRequestSnapshot = namedQueueSnapshot.Requests[index];
						TreeNode queuedItemNode = new TreeNode(queuedRequestSnapshot.ProjectName);
						queuedItemNode.Tag = new IntegrationQueueTreeNodeTag(this, namedQueueSnapshot, queuedRequestSnapshot, index);
						if (index == 0)
						{
							queuedItemNode.ImageIndex = queuedItemNode.SelectedImageIndex = IntegrationQueueNodeType.FirstInQueue.ImageIndex;
						}
						else
						{
							queuedItemNode.ImageIndex = queuedItemNode.SelectedImageIndex = IntegrationQueueNodeType.PendingInQueue.ImageIndex;
						}
						queueNode.Nodes.Add(queuedItemNode);
					}
				}
			}
			serverTreeNode.ExpandAll();
		}
	}
}