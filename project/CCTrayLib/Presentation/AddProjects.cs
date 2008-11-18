using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class AddProjects: Form
	{
		private readonly ICruiseProjectManagerFactory cruiseProjectManagerFactory;
		private readonly IEnumerable<CCTrayProject> currentProjectList;

		private BuildServer selectedServer;

		public AddProjects(ICruiseProjectManagerFactory cruiseProjectManagerFactory, IEnumerable<CCTrayProject> currentProjectList)
		{
			this.cruiseProjectManagerFactory = cruiseProjectManagerFactory;
			this.currentProjectList = currentProjectList;

			InitializeComponent();
			PopulateServerListFromProjects();
		}

		private void PopulateServerListFromProjects()
		{
			foreach (CCTrayProject project in currentProjectList)
			{
				if (!lbServer.Items.Contains(project.BuildServer))
					lbServer.Items.Add(project.BuildServer);
			}
		}

		public IEnumerable<CCTrayProject> GetListOfNewProjects(IWin32Window owner)
		{
			if (ShowDialog(owner) != DialogResult.OK)
				return new CCTrayProject[] {};

			List<CCTrayProject> newProjects = new List<CCTrayProject>();

			foreach (string projectName in lbProject.SelectedItems)
				newProjects.Add(new CCTrayProject(selectedServer, projectName));

			return newProjects;
		}

		private void btnAddServer_Click(object sender, EventArgs e)
		{
			AddBuildServer addBuildServer = new AddBuildServer(cruiseProjectManagerFactory);

			BuildServer buildServer = addBuildServer.ChooseNewBuildServer(this);
			if (buildServer != null)
				lbServer.SelectedIndex = lbServer.Items.Add(buildServer);
		}

		private void lbServer_SelectedIndexChanged(object sender, EventArgs e)
		{
			selectedServer = (BuildServer) lbServer.SelectedItem;
			if (selectedServer != null)
				RetrieveListOfProjects(selectedServer);
		}

		private void RetrieveListOfProjects(BuildServer server)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				lbProject.Items.Clear();

				foreach (CCTrayProject project in cruiseProjectManagerFactory.GetProjectList(server))
				{
					if (!IsProjectAlreadyAdded(project))
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

		private void lbProject_KeyDown(object sender, KeyEventArgs e)
		{
			if (KeyUtils.PressedControlA(e))
			{
				for (int i = 0; i < lbProject.Items.Count; i++)
					lbProject.SetSelected(i, true);
			}
		}
	}
}
