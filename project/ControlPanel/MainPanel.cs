using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

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
			// MainPanel
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Menu = this.mainMenu1;
			this.Name = "MainPanel";
			this.Text = "Cruise Control - Control Panel";

		}
		#endregion

		public static void Main() 
		{
			MainPanel panel = new MainPanel();
			Application.Run(panel);
		}

		private void openMenuItem_Click(object sender, System.EventArgs e)
		{		
		}

		protected virtual string ChooseFileToOpen() 
		{
			return null;
		}
	}
}
