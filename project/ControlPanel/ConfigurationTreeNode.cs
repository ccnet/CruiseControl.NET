using System;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class ConfigurationTreeNode : TreeNode
	{
		public ConfigurationModel Model;

		public ConfigurationTreeNode(string filename, ConfigurationModel model) : base(filename) 
		{
			this.Model = model;
		}
	}
}
