using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class BuildProjectsControl : UserControl
	{
		private ProjectConfigurationListViewItemAdaptor selected = null;
		private int selectedIndex = -1;
		private ICCTrayMultiConfiguration configuration;

		public BuildProjectsControl()
		{
			InitializeComponent();
		}

		public void BindListView(ICCTrayMultiConfiguration configuration)
		{
			this.configuration = configuration;

			lvProjects.Items.Clear();

			foreach (CCTrayProject project in configuration.Projects)
			{
				lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(project).Item);
			}

			UpdateButtons();
		}

		private void lvProjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lvProjects.SelectedItems.Count == 0)
			{
				selected = null;
				selectedIndex = -1;
			}
			else
			{
				selected = (ProjectConfigurationListViewItemAdaptor)lvProjects.SelectedItems[0].Tag;
				selectedIndex = lvProjects.SelectedIndices[0];
			}

			UpdateButtons();
		}

		private void UpdateButtons()
		{
			btnRemove.Enabled = selected != null;

			if (selected != null)
			{
				btnMoveDown.Enabled = selectedIndex < (lvProjects.Items.Count - 1);
				btnMoveUp.Enabled = selectedIndex != 0;
			}
			else
			{
				btnMoveDown.Enabled = btnMoveUp.Enabled = false;
			}
		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			lvProjects.BeginUpdate();
			foreach (ListViewItem item in lvProjects.SelectedItems)
			{
				lvProjects.Items.Remove(item);
			}
			lvProjects.EndUpdate();
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(-1);
		}

		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(+1);
		}

		private void MoveSelectedItem(int delta)
		{
			ProjectConfigurationListViewItemAdaptor currentlySelected = selected;
			int reinsertIndex = selectedIndex + delta;

			lvProjects.Items.RemoveAt(selectedIndex);
			lvProjects.Items.Insert(reinsertIndex, currentlySelected.Item);
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			AddProjects addProjectDialog =
				new AddProjects(configuration.CruiseProjectManagerFactory, BuildProjectListFromListView());
			CCTrayProject[] projects = addProjectDialog.GetListOfNewProjects(this);

			if (projects != null)
			{
				foreach (CCTrayProject newProject in projects)
				{
					lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(newProject).Item);
				}
			}
		}

		private CCTrayProject[] BuildProjectListFromListView()
		{
			CCTrayProject[] newProjectList = new CCTrayProject[lvProjects.Items.Count];

			for (int i = 0; i < lvProjects.Items.Count; i++)
			{
				ProjectConfigurationListViewItemAdaptor adaptor = (ProjectConfigurationListViewItemAdaptor)lvProjects.Items[i].Tag;
				newProjectList[i] = adaptor.Project;
			}
			return newProjectList;
		}

		private void lvProjects_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			foreach (CCTrayProject project in configuration.Projects)
			{
				if (e.Item.SubItems[2].Text == project.ProjectName)
				{
					project.ShowProject = e.Item.Checked;
				}
			}
		}

		private void lvProjects_KeyDown(object sender, KeyEventArgs e)
		{
			if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
			{
				foreach (ListViewItem item in lvProjects.Items)
				{
					item.Selected = true;
				}
			}

			if (e.KeyCode == Keys.Delete)
			{
				lvProjects.BeginUpdate();
				foreach (ListViewItem item in lvProjects.SelectedItems)
				{
					lvProjects.Items.Remove(item);
				}
				lvProjects.EndUpdate();
			}
		}

		private void chkCheck_CheckedChanged(object sender, EventArgs e)
		{
			if (chkCheckAllProjects.Checked)
			{
				foreach (ListViewItem item in lvProjects.Items)
				{
					item.Checked = true;
				}
			}
			else
			{
				foreach (ListViewItem item in lvProjects.Items)
				{
					item.Checked = false;
				}
			}
		}

		public void PersistProjectTabSettings()
		{
			CCTrayProject[] newProjectList = BuildProjectListFromListView();
			configuration.Projects = newProjectList;
		}

	}
}
