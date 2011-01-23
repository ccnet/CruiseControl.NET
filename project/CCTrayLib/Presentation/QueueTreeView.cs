using System.Collections.Generic;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    /// <summary>
    /// Subclass to ensure that selections are maintained when the treeview is refreshed.
    /// </summary>
    public class QueueTreeView : TreeView
    {
        private IList<string> expandedNodes;
        private bool inRestoreExpandedNodesMode;

        public QueueTreeView()
        {
            expandedNodes = new List<string>();
            inRestoreExpandedNodesMode = false;
        }

        protected override void OnAfterExpand(TreeViewEventArgs e)
        {
            if (!inRestoreExpandedNodesMode)
            {
                IntegrationQueueTreeNodeTag tag = e.Node.Tag as IntegrationQueueTreeNodeTag;
                if (tag != null && tag.IsQueueNode)
                    expandedNodes.Add(GetQueueId(e.Node, tag));
            }
            base.OnAfterExpand(e);
        }
        
        protected override void OnAfterCollapse(TreeViewEventArgs e)
        {
            if (!inRestoreExpandedNodesMode)
            {
                IntegrationQueueTreeNodeTag tag = e.Node.Tag as IntegrationQueueTreeNodeTag;
                if (tag != null && tag.IsQueueNode)
                    expandedNodes.Remove(GetQueueId(e.Node, tag));
            }
            base.OnAfterCollapse(e);
        }

        public void RestoreExpandedNodes(TreeNode parentNode)
        {
            inRestoreExpandedNodesMode = true;
            foreach (TreeNode treeNode in parentNode.Nodes)
            {
                IntegrationQueueTreeNodeTag tag = treeNode.Tag as IntegrationQueueTreeNodeTag;
                if (tag != null && tag.IsQueueNode)
                {
                    if (expandedNodes.Contains(GetQueueId(treeNode, tag)))
                    {
                        treeNode.Expand();
                    }
                }
            }
            inRestoreExpandedNodesMode = false;
        }

        private string GetQueueId(TreeNode treeNode, IntegrationQueueTreeNodeTag tag)
        {
            // Identify the queue using a combination of the server name, the server type and the queue name.
            return string.Format(System.Globalization.CultureInfo.CurrentCulture,"{0}:{1}:{2}", treeNode.Parent.Name, treeNode.Parent.ImageIndex, tag.QueueSnapshot.QueueName);
        }
    }
}
