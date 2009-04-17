using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using System.Diagnostics;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public partial class CurrentStatusWindow : Form
    {
        private IProjectMonitor projectToMonitor;
        private ProjectStatusSnapshot snapshot;
        private Exception currentError;
        private Stopwatch loadStopwatch = new Stopwatch();
        private List<string> oldKeys = new List<string>();
        private List<string> newKeys = new List<string>();

        public CurrentStatusWindow(IProjectMonitor projectToMonitor)
        {
            InitializeComponent();
            ProjectToMonitor = projectToMonitor;
        }

        public IProjectMonitor ProjectToMonitor
        {
            get { return projectToMonitor; }
            set
            {
                // Nice big logic check to make sure nothing is missed out
                bool changeMonitor = ((value != null) && (value.Detail != null) && (value.Detail.Configuration != null));
                if (changeMonitor)
                {
                    if ((projectToMonitor == null) ||
                        (projectToMonitor.Detail == null) ||
                        (projectToMonitor.Detail.Configuration == null))
                    {
                        changeMonitor = true;
                    }
                    else
                    {
                        changeMonitor = (projectToMonitor.Detail.Configuration != value.Detail.Configuration);
                    }
                }

                // Now we can do the actual monitor change
                if (changeMonitor)
                {
                    Text = string.Format("Current Status for {0} [{1}]",
                        value.Detail.ProjectName,
                        value.Detail.ServerName);
                    projectToMonitor = value;
                    statusExplorer.Nodes.Clear();
                    RefreshStatus();
                }
            }
        }

        private void RefreshStatus()
        {
            // If the background worker is not busy, then start the refresh, otherwise there is already
            // a refresh under way, so there is no need to start another
            if (!displayWorker.IsBusy)
            {
                // Update the display and then start the background worker
                refreshTimer.Enabled = false;
                currentStatus.Text = "Loading status...";
                currentError = null;
                loadStopwatch.Reset();
                loadStopwatch.Start();
                displayWorker.RunWorkerAsync();
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void refreshCommand_Click(object sender, EventArgs e)
        {
            RefreshStatus();
        }

        private void displayWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                ProjectStatusSnapshot newSnapshot = projectToMonitor.RetrieveSnapshot();
                snapshot = newSnapshot;
            }
            catch (Exception error)
            {
                currentError = error;
            }
        }

        private void displayWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            loadStopwatch.Stop();
            currentStatus.Text = string.Format("Status loaded ({0:0.00}s)",
                Convert.ToDouble(loadStopwatch.ElapsedMilliseconds) / 1000);
            if (currentError == null)
            {
                // Update the display
                AddToExplorer(snapshot, statusExplorer.Nodes, 0);

                // Remove any old items
                foreach (string key in oldKeys)
                {
                    if (!newKeys.Contains(key))
                    {
                        TreeNode[] oldNodes = statusExplorer.Nodes.Find(key, true);
                        foreach (TreeNode oldNode in oldNodes)
                        {
                            oldNode.Remove();
                        }
                    }
                }
                if (statusExplorer.SelectedNode == null)
                {
                    statusExplorer.SelectedNode = statusExplorer.Nodes[0];
                }
                else
                {
                    DisplayStatusItem(statusExplorer.SelectedNode.Tag as ItemStatus);
                }

                // Update the lists
                oldKeys = newKeys;
                newKeys = new List<string>();
            }
            else
            {
                // Tell the user an error occurred and what sort of error
                MessageBox.Show("Unable to refresh status: " + currentError.Message,
                    "Refresh Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            refreshTimer.Enabled = true;
        }

        private void AddToExplorer(ItemStatus value, TreeNodeCollection nodes, int position)
        {
            TreeNode node = null;

            // First attempt to find a node with a matching name
            foreach (TreeNode nodeToCheck in nodes)
            {
                if (object.Equals((nodeToCheck.Tag as ItemStatus), value))
                {
                    node = nodeToCheck;
                    break;
                }
            }

            string nodeKey = value.Identifier.ToString();
            bool newNode = false;
            if (node == null)
            {
                // Need to add a whole new node
                node = nodes.Insert(position, nodeKey, value.Name);
                newNode = true;
            }
            newKeys.Add(nodeKey);

            // Update the node
            node.Tag = value;
            node.Text = value.Name;
            node.ImageKey = value.Status.ToString();
            node.SelectedImageKey = node.ImageKey;

            // Update all the child items
            int childPosition = 0;
            foreach (ItemStatus childItem in value.ChildItems)
            {
                AddToExplorer(childItem, node.Nodes, childPosition++);
            }
            if (newNode) node.Expand();
        }

        private void statusExplorer_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ItemStatus status = e.Node.Tag as ItemStatus;
            DisplayStatusItem(status);
        }

        private void DisplayStatusItem(ItemStatus status)
        {
            statusDetails.SelectedObject = new StatusItemDisplay(status);
            int progress = 0;
            Color color = Color.Gray;

            // Calculate the progress bar
            switch (status.Status)
            {
                case ItemBuildStatus.CompletedSuccess:
                    progress = 100;
                    color = Color.Green;
                    break;
                case ItemBuildStatus.CompletedFailed:
                    progress = 100;
                    color = Color.Red;
                    break;
                case ItemBuildStatus.Cancelled:
                    progress = 100;
                    break;
                case ItemBuildStatus.Running:
                    // TODO: Caculdate the progress percentage
                    progress = 50;
                    break;
            }

            // Update the progress bar
            statusProgress.Value = (progress > 100) ? 100 : progress;
            statusProgress.ForeColor = color;
        }
    }
}
