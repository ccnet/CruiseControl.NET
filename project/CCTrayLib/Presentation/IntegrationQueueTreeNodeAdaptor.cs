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
				foreach (QueueSnapshot queue in queueSetSnapshot.Queues)
				{
				    string queueNodeTitle = queue.QueueName;
                    int queueImageIndex = IntegrationQueueNodeType.QueueEmpty.ImageIndex;
				    if (!queue.IsEmpty)
				    {
                        queueNodeTitle = string.Format("{0} ({1})", queue.QueueName, queue.Requests.Count);
                        queueImageIndex = IntegrationQueueNodeType.QueuePopulated.ImageIndex;
				    }
                    TreeNode queueNode = new TreeNode(queueNodeTitle, queueImageIndex, queueImageIndex);
					queueNode.Tag = new IntegrationQueueTreeNodeTag(this, queue);
					serverTreeNode.Nodes.Add(queueNode);

					for (int index = 0; index < queue.Requests.Count; index++)
					{
						QueuedRequestSnapshot queuedRequest = queue.Requests[index];
						TreeNode queuedItemNode = new TreeNode(queuedRequest.ProjectName);
						queuedItemNode.Tag = new IntegrationQueueTreeNodeTag(this, queue, queuedRequest, index);

                        int requestImageIndex = IntegrationQueueNodeType.PendingInQueue.ImageIndex;
                        if (queuedRequest.Activity == ProjectActivity.CheckingModifications)
                            requestImageIndex = IntegrationQueueNodeType.CheckingModifications.ImageIndex;
                        else if (queuedRequest.Activity == ProjectActivity.Building)
                            requestImageIndex = IntegrationQueueNodeType.Building.ImageIndex;

                        queuedItemNode.ImageIndex = queuedItemNode.SelectedImageIndex = requestImageIndex;
						queueNode.Nodes.Add(queuedItemNode);
					}
				}
			}

            serverTreeNode.Expand();
		    QueueTreeView queueTreeView = serverTreeNode.TreeView as QueueTreeView;
		    if (queueTreeView != null)
                queueTreeView.RestoreExpandedNodes(serverTreeNode);
		}
	}
}