using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class ProjectConfigurationListViewItemAdaptor
	{
		private ListViewItem item;
		private CCTrayProject project;

		public ProjectConfigurationListViewItemAdaptor(CCTrayProject project)
		{
			this.project = project;
			item = new ListViewItem(new string[] {project.BuildServer.DisplayName, project.BuildServer.Transport.ToString(), project.ProjectName});
			item.Checked = project.ShowProject;
			item.Tag = this;
		}

		public ListViewItem Item
		{
			get { return item; }
		}

		public CCTrayProject Project
		{
			get { return project; }
		}

		public void Rebind()
		{
			item.SubItems[0].Text = project.BuildServer.DisplayName;
			item.SubItems[1].Text = project.BuildServer.Transport.ToString();
			item.SubItems[2].Text = project.ProjectName;
			item.Checked = project.ShowProject;
		}
	}
}
