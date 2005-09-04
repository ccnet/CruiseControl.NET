using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectConfigurationListViewItemAdaptor
	{
		private ListViewItem item;
		private Project project;

		public ProjectConfigurationListViewItemAdaptor(Project project)
		{
			this.project = project;
			item = new ListViewItem(new string[] {project.ServerDisplayName, project.ProjectName});
			item.Tag = this;
		}

		public ListViewItem Item
		{
			get { return item; }
		}

		public Project Project
		{
			get { return project; }
		}

		public void Rebind()
		{
			item.SubItems[0].Text = project.ServerDisplayName;
			item.SubItems[1].Text = project.ProjectName;
		}
	}
}