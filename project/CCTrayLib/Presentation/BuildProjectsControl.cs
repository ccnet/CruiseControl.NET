using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class BuildProjectsControl : UserControl
	{
		private ProjectConfigurationListViewItemAdaptor selected;
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

			foreach (CCTrayProject p in addProjectDialog.GetListOfNewProjects(this))
				lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(p).Item);
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
			if(KeyUtils.PressedControlA(e))
			{
				// Select all items.
				foreach (ListViewItem item in lvProjects.Items)
					item.Selected = true;
			}
			else
			if (e.KeyCode == Keys.Delete)
			{
				// Delete selected items.
				lvProjects.BeginUpdate();
				foreach (ListViewItem item in lvProjects.SelectedItems)
					lvProjects.Items.Remove(item);

				lvProjects.EndUpdate();
			}
		}

		private void chkCheck_CheckedChanged(object sender, EventArgs e)
		{
			bool should_check = chkCheckAllProjects.Checked;
			foreach (ListViewItem item in lvProjects.Items)
				item.Checked = should_check;
		}

		public void PersistProjectTabSettings()
		{
			configuration.Projects = BuildProjectListFromListView();
		}
	}
}
