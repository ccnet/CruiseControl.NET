using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Config;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class MainPanel : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.MenuItem openConfigurationMenuItem;
		private System.Windows.Forms.MenuItem closeConfigurationMenuItem;
		private System.Windows.Forms.MenuItem addProjectMenuItem;
		private System.Windows.Forms.MenuItem removeProjectMenuItem;
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.TreeView treeView;
		private System.Windows.Forms.Panel bodyPanel;
		private System.Windows.Forms.Splitter splitter1;
		private TreeNode baseNode;
		private TreeNode lastSelectedNode;
		private TreeNode lastClickedControl;

		public MainPanel()
		{
			InitializeComponent();

			baseNode = new	TreeNode("Cruise Control Installations");
			baseNode.Expand();
			treeView.Nodes.Add(baseNode);
			treeView.AfterSelect += new TreeViewEventHandler(treeView_AfterSelect);
			treeView.ContextMenu = new ContextMenu();
			treeView.ContextMenu.Popup += new EventHandler(ContextMenu_Popup);
			treeView.MouseDown += new MouseEventHandler(treeView_MouseDown);

			closeConfigurationMenuItem = new MenuItem("close configuration", new EventHandler(CloseConfiguration));
			addProjectMenuItem = new MenuItem("add project", new EventHandler(AddProject));
			removeProjectMenuItem = new MenuItem("remove project", new EventHandler(RemoveProject));
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.fileMenuItem = new System.Windows.Forms.MenuItem();
			this.openConfigurationMenuItem = new System.Windows.Forms.MenuItem();
			this.treeView = new System.Windows.Forms.TreeView();
			this.bodyPanel = new System.Windows.Forms.Panel();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.fileMenuItem});
			// 
			// fileMenuItem
			// 
			this.fileMenuItem.Index = 0;
			this.fileMenuItem.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.openConfigurationMenuItem});
			this.fileMenuItem.Text = "&File";
			// 
			// openConfigurationMenuItem
			// 
			this.openConfigurationMenuItem.Index = 0;
			this.openConfigurationMenuItem.Text = "&Open";
			this.openConfigurationMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView.ImageIndex = -1;
			this.treeView.Location = new System.Drawing.Point(0, 0);
			this.treeView.Name = "treeView";
			this.treeView.SelectedImageIndex = -1;
			this.treeView.Size = new System.Drawing.Size(232, 449);
			this.treeView.TabIndex = 0;
			// 
			// bodyPanel
			// 
			this.bodyPanel.AutoScroll = true;
			this.bodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.bodyPanel.Location = new System.Drawing.Point(232, 0);
			this.bodyPanel.Name = "bodyPanel";
			this.bodyPanel.Size = new System.Drawing.Size(600, 449);
			this.bodyPanel.TabIndex = 2;
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(232, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(3, 449);
			this.splitter1.TabIndex = 3;
			this.splitter1.TabStop = false;
			// 
			// MainPanel
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(832, 449);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.bodyPanel);
			this.Controls.Add(this.treeView);
			this.Menu = this.mainMenu1;
			this.Name = "MainPanel";
			this.Text = "Cruise Control - Control Panel";
			this.ResumeLayout(false);

		}
		#endregion

		public static void Main() 
		{
			MainPanel panel = new MainPanel();
			panel.Open(@"c:\tmp\ccnet.config");
//			new Prototype().Show();
			Application.Run(panel);
		}

		private void openMenuItem_Click(object sender, System.EventArgs e)
		{
			string filename = ChooseFileToOpen();
			if (filename != null) Open(filename);
		}

		private void Open(string filename) 
		{
			ConfigurationModel model = new ConfigurationModel();
			try 
			{
				model.Load(filename);
			}
			catch (ConfigurationException ex) 
			{
				MessageBox.Show("there was an error loading (" + filename + ") : \n" + ex.ToString(), "Error");
				return;
			}

			ConfigurationTreeNode configurationNode = model.GetNavigationTreeNodes();
			configurationNode.Expand();
			baseNode.Nodes.Add(configurationNode);
		}

		protected virtual string ChooseFileToOpen() 
		{
			OpenFileDialog dialog = new OpenFileDialog();
			return DialogResult.OK == dialog.ShowDialog(this) ? dialog.FileName : null;
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			if (e.Node == lastSelectedNode) return;

			if (lastSelectedNode is ConfigurationItemTreeNode)
			{
				try 
				{
					((ConfigurationItemTreeNode) lastSelectedNode).Project.Save();
				}
				catch (Exception ex) 
				{
					MessageBox.Show(ex.ToString());
					treeView.SelectedNode = lastSelectedNode;
					return;
				}
			}

			bodyPanel.Controls.Clear();

			if (e.Node is ConfigurationItemTreeNode) 
			{
				BunchOfConfigurationItemControls controls = new BunchOfConfigurationItemControls();
				controls.Bind(((ConfigurationItemTreeNode) e.Node).Items);
				
				bodyPanel.Controls.Add(controls);
			}

			lastSelectedNode = e.Node;
		}

		private void ContextMenu_Popup(object sender, EventArgs e)
		{
			ContextMenu menu = (ContextMenu) sender;
			menu.MenuItems.Clear();

			treeView.SelectedNode = lastClickedControl;
			if (treeView.SelectedNode != lastClickedControl) return;

			if (treeView.SelectedNode == baseNode)
			{
				menu.MenuItems.Add(openConfigurationMenuItem);
			}
			else if (treeView.SelectedNode is ConfigurationTreeNode)
			{
				menu.MenuItems.Add(closeConfigurationMenuItem);
				menu.MenuItems.Add(addProjectMenuItem);
			}
			else if (treeView.SelectedNode is ConfigurationItemTreeNode && 
				((ConfigurationItemTreeNode) treeView.SelectedNode).IsProject)
			{
				menu.MenuItems.Add(removeProjectMenuItem);
			}
		}

		private void treeView_MouseDown(object sender, MouseEventArgs e)
		{
			lastClickedControl = treeView.GetNodeAt(e.X, e.Y);
		}

		private void CloseConfiguration(object sender, EventArgs e) 
		{
			if (treeView.SelectedNode is ConfigurationTreeNode)
			{
				ConfigurationTreeNode fileNode = (ConfigurationTreeNode) treeView.SelectedNode;
				treeView.SelectedNode = baseNode;
				baseNode.Nodes.Remove(fileNode);
			}
		}

		private void AddProject(object sender, EventArgs e)
		{
		}
		
		private void RemoveProject(object sender, EventArgs e)
		{
		}
	}
}
