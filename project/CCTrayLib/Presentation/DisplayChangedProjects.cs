using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public partial class DisplayChangedProjects : Form
    {
        public DisplayChangedProjects(Dictionary<string, ServerSnapshotChangedEventArgs> servers)
        {
            InitializeComponent();

            foreach (var server in servers.Values)
            {
                PopulateProjectList(server, server.AddedProjects, addedProjectsList, "AddedProject");
                PopulateProjectList(server, server.DeletedProjects, deletedProjectsList, "DeletedProject");
            }
        }

        private void PopulateProjectList(ServerSnapshotChangedEventArgs args, IList<string> projects, ListView listView, string imageKey)
        {
            if (projects.Count > 0)
            {
                var group = new ListViewGroup(args.Server);
                group.Tag = args.Configuration;
                listView.Groups.Add(group);
                foreach (var project in projects)
                {
                    var item = new ListViewItem(project, group);
                    item.Checked = true;
                    item.ImageKey = imageKey;
                    listView.Items.Add(item);
                }
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (UpdateConfiguration != null)
            {
                var serverList = new Dictionary<string, BuildServer>();

                // Add all the new projects to be added
                var serversWithNewProjects = new Dictionary<string, List<string>>();
                foreach (ListViewGroup group in addedProjectsList.Groups)
                {
                    var list = new List<string>();
                    serversWithNewProjects.Add(group.Header, list);
                    serverList.Add(group.Header, group.Tag as BuildServer);
                    foreach (ListViewItem item in group.Items)
                    {
                        if (item.Checked) list.Add(item.Text);
                    }
                }

                // Add all the new projects to be removed
                var serversWithOldProjects = new Dictionary<string, List<string>>();
                foreach (ListViewGroup group in deletedProjectsList.Groups)
                {
                    var list = new List<string>();
                    serversWithOldProjects.Add(group.Header, list);
                    if (!serverList.ContainsKey(group.Header))
                    {
                        serverList.Add(group.Header, group.Tag as BuildServer);
                    }
                    foreach (ListViewItem item in group.Items)
                    {
                        if (item.Checked) list.Add(item.Text);
                    }
                }

                // Merge the two lists
                var args = new List<ServerSnapshotChangedEventArgs>();
                foreach (var server in serverList)
                {
                    var addedProjects = serversWithNewProjects.ContainsKey(server.Key) ? serversWithNewProjects[server.Key] : new List<string>();
                    var deletedProjects = serversWithOldProjects.ContainsKey(server.Key) ? serversWithOldProjects[server.Key] : new List<string>();
                    var serverArgs = new ServerSnapshotChangedEventArgs(server.Key, server.Value, addedProjects, deletedProjects);
                    args.Add(serverArgs);
                }
                UpdateConfiguration(args);
            }
        }

        public event Action<List<ServerSnapshotChangedEventArgs>> UpdateConfiguration;
    }
}
