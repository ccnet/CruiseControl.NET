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
			// 
			// MainPanel
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Name = "MainPanel";
			this.Text = "Cruise Control - Control Panel";

		}
		#endregion

		public static void Main() 
		{
			MainPanel panel = new MainPanel();
			Application.Run(panel);
		}
	}
}
