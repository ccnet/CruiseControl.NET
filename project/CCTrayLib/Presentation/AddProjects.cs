using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class AddProjects : Form
	{
		private BuildServer selectedServer;

		private readonly ICruiseProjectManagerFactory cruiseProjectManagerFactory;
        private readonly ICruiseServerManagerFactory cruiseServerManagerFactory;
        private Button butConfigure;
        private ColumnHeader columnHeader1;
        private ImageList imlSmall;
		private readonly CCTrayProject[] currentProjectList;

		public AddProjects(ICruiseProjectManagerFactory cruiseProjectManagerFactory,
            ICruiseServerManagerFactory cruiseServerManagerFactory,
            CCTrayProject[] currentProjectList)
		{
			this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
            this.cruiseServerManagerFactory = cruiseServerManagerFactory;
			this.currentProjectList = currentProjectList;

			InitializeComponent();

            List<BuildServer> serverList = new List<BuildServer>();
			foreach (CCTrayProject project in currentProjectList)
			{
                if (!serverList.Contains(project.BuildServer)) serverList.Add(project.BuildServer);
		}

            foreach (BuildServer server in serverList)
		{
                AddServer(server);
			}
		}

		public CCTrayProject[] GetListOfNewProjects(IWin32Window owner)
		{
			if (ShowDialog(owner) == DialogResult.OK)
			{
				ArrayList newProjects = new ArrayList();
			foreach (string projectName in lbProject.SelectedItems)
				{
				newProjects.Add(new CCTrayProject(selectedServer, projectName));
				}
				return (CCTrayProject[]) newProjects.ToArray(typeof (CCTrayProject));
			}
			return null;
		}

		private void btnAddServer_Click(object sender, EventArgs e)
		{
			AddBuildServer addBuildServer = new AddBuildServer(cruiseProjectManagerFactory);

			BuildServer buildServer = addBuildServer.ChooseNewBuildServer(this);
			if (buildServer != null)
			{
                AddServer(buildServer).Selected = true;
		}
		}

		private void RetrieveListOfProjects(BuildServer server)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				lbProject.Items.Clear();

				CCTrayProject[] projectList = cruiseProjectManagerFactory.GetProjectList(server, false);

				foreach (CCTrayProject project in projectList)
				{
					if (! IsProjectAlreadyAdded(project))
						lbProject.Items.Add(project.ProjectName);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(this, "Unable to connect to server " + server.DisplayName + ": " + ex.Message, "Error");
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private bool IsProjectAlreadyAdded(CCTrayProject project)
		{
			foreach (CCTrayProject currentProject in currentProjectList)
			{
				if (project.ServerUrl == currentProject.ServerUrl && project.ProjectName == currentProject.ProjectName)
					return true;
			}
			return false;
		}

		private void KeyEventHelper(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Enter)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				this.Close();
			}
			
			if (e.KeyCode == Keys.Escape)
			{
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				this.Close();
			}
		}
		
		private void lbProject_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
			{
				for (int i = 0; i < lbProject.Items.Count; i++)
				{
					lbProject.SetSelected(i, true);
			}
		}
			
			KeyEventHelper(sender, e);
		}
		
		private void lbServer_KeyDown(object sender, KeyEventArgs e)
		{
			KeyEventHelper(sender, e);
		}
		
		private void AddProjects_KeyDown(object sender, KeyEventArgs e)
		{
			KeyEventHelper(sender, e);
		}

        private void lbServer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbServer.SelectedItems.Count > 0)
            {
                selectedServer = (BuildServer)lbServer.SelectedItems[0].Tag;
            }
            else
            {
                selectedServer = null;
            }
            if (selectedServer != null) RetrieveListOfProjects(selectedServer);
        }

        private ListViewItem AddServer(BuildServer server)
        {
            ListViewItem newServerItem = lbServer.Items.Add(server.DisplayName, "NonSecure");
            newServerItem.Tag = server;
            SetSecurityIcon(newServerItem, server);
            return newServerItem;
        }

        private void SetSecurityIcon(ListViewItem item, BuildServer server)
        {
            if (server.SecurityType != null)
            {
                item.ImageKey = "Secure";
            }
            else
            {
                item.ImageKey = "NonSecure";
            }
        }

        private void butConfigure_Click(object sender, EventArgs e)
        {
            if (selectedServer == null)
            {
                MessageBox.Show(this, "Please select a server first", "Unable to configure", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                if (ConfigureServer.Configure(this, selectedServer))
                {
                    var server = this.cruiseServerManagerFactory.Create(selectedServer);
                    server.Logout();
                    RetrieveListOfProjects(selectedServer);
                    SetSecurityIcon(lbServer.SelectedItems[0], selectedServer);
                }
            }
        }
	}
}
