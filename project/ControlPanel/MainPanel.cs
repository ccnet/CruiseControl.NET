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
	/// <summary>
	/// this is the MainPanel of the ControlPanel
	/// </summary>
	public class MainPanel : System.Windows.Forms.Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem fileMenuItem;
		private System.Windows.Forms.MenuItem openMenuItem;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox projectName;
		private System.ComponentModel.Container components = null;

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
			this.label1 = new System.Windows.Forms.Label();
			this.projectName = new System.Windows.Forms.TextBox();
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
																						 this.openMenuItem});
			this.fileMenuItem.Text = "&File";
			// 
			// openMenuItem
			// 
			this.openMenuItem.Index = 0;
			this.openMenuItem.Text = "&Open";
			this.openMenuItem.Click += new System.EventHandler(this.openMenuItem_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(192, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(88, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Project Name:";
			// 
			// projectName
			// 
			this.projectName.Location = new System.Drawing.Point(288, 32);
			this.projectName.Name = "projectName";
			this.projectName.Size = new System.Drawing.Size(248, 20);
			this.projectName.TabIndex = 1;
			this.projectName.Text = "";
			// 
			// MainPanel
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(824, 545);
			this.Controls.Add(this.projectName);
			this.Controls.Add(this.label1);
			this.Menu = this.mainMenu1;
			this.Name = "MainPanel";
			this.Text = "Cruise Control - Control Panel";
			this.ResumeLayout(false);

		}
		#endregion

		public static void Main() 
		{
			MainPanel panel = new MainPanel();
			Application.Run(panel);
		}

		private void openMenuItem_Click(object sender, System.EventArgs e)
		{
			string filename = ChooseFileToOpen();
			CruiseServer server = new CruiseServer(new ConfigurationLoader(filename));
			foreach (IProject project in server.Configuration) 
			{
				projectName.Text = project.Name;
			}
		}

		protected virtual string ChooseFileToOpen() 
		{
			OpenFileDialog dialog = new OpenFileDialog();
			return DialogResult.OK == dialog.ShowDialog(this) ? dialog.FileName : null;
		}
	}
}
