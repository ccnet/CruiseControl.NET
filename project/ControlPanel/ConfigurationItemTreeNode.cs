using System;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class ConfigurationItemTreeNode : TreeNode
	{
		private ProjectModel project;
		private IConfigurationItem item;
		private ConfigurationItem [] items;

		public ConfigurationItemTreeNode(ProjectModel project, IConfigurationItem item, ConfigurationItem [] items) : base(item.Name) 
		{
			this.project = project;
			this.item = item;
			this.items = items;
		}

		public bool IsProject
		{
			get { return item == project; }
		}

        public ProjectModel Project
		{
			get { return project; }
		}

		public ConfigurationItem [] Items
		{
			get { return items; }
		}
	}
}
