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
		private System.Windows.Forms.MenuItem openMenuItem;
		private System.Windows.Forms.TabControl tabControl;
		private System.Windows.Forms.MenuItem saveMenuItem;
		private System.ComponentModel.Container components = null;
		private ConfigurationModel _model;
		private string _filename;

		public MainPanel()
		{
			InitializeComponent();
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
			this.openMenuItem = new System.Windows.Forms.MenuItem();
			this.tabControl = new System.Windows.Forms.TabControl();
			this.saveMenuItem = new System.Windows.Forms.MenuItem();
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
																						 this.openMenuItem,
																						 this.saveMenuItem});
			this.fileMenuItem.Text = "&File";
			// 
			// openMenuItem
			// 
			this.openMenuItem.Index = 0;
			this.openMenuItem.Text = "&Open";
			this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
			// 
			// tabControl
			// 
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedIndex = 0;
			this.tabControl.Size = new System.Drawing.Size(992, 649);
			this.tabControl.TabIndex = 0;
			// 
			// saveMenuItem
			// 
			this.saveMenuItem.Index = 1;
			this.saveMenuItem.Text = "&Save";
			this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
			// 
			// MainPanel
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(992, 649);
			this.Controls.Add(this.tabControl);
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
			Application.Run(panel);
		}

		private void openMenuItem_Click(object sender, System.EventArgs e)
		{
			string filename = ChooseFileToOpen();
			if (filename != null) Open(filename);
		}

		private void Open(string filename) 
		{
			_filename = filename;
			_model = new ConfigurationModel();
			try 
			{
				_model.Load(filename);
			}
			catch (ConfigurationException ex) 
			{
				MessageBox.Show("there was an error loading (" + filename + ") : \n" + ex.ToString(), "Error");
				return;
			}

			foreach (ProjectItem project in _model.Projects)
			{
				BunchOfConfigurationItemControls controls = new BunchOfConfigurationItemControls();
				controls.Bind(project.Items);
				
				TabPage page = new TabPage(project.Name);
				page.Controls.Add(controls);
				
				tabControl.TabPages.Add(page);
			}

		}

		protected virtual string ChooseFileToOpen() 
		{
			OpenFileDialog dialog = new OpenFileDialog();
			return DialogResult.OK == dialog.ShowDialog(this) ? dialog.FileName : null;
		}

		private void saveMenuItem_Click(object sender, System.EventArgs e)
		{
			_model.Save(_filename);
		}
	}
}
