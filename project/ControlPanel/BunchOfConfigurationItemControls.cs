using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	/// <summary>
	/// Summary description for BunchOfConfigurationItemControls.
	/// </summary>
	public class BunchOfConfigurationItemControls : System.Windows.Forms.UserControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BunchOfConfigurationItemControls()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// BunchOfConfigurationItemControls
			// 
			this.Name = "BunchOfConfigurationItemControls";
			this.Size = new System.Drawing.Size(600, 150);

		}
		#endregion

		public void Bind(ConfigurationItemCollection items) 
		{
			foreach (Control control in Controls)
			{
				control.Resize -= new EventHandler(ChildControlResized);
			}
			Controls.Clear();

			foreach (ConfigurationItem childItem in items)
			{
				ConfigurationItemControl childControl = new ConfigurationItemControl();
				childControl.Bind(childItem);
				Controls.Add(childControl);
				childControl.Resize += new EventHandler(ChildControlResized);
			}
			ChildControlResized(null, EventArgs.Empty);
		}

		private void ChildControlResized(object sender, EventArgs e)
		{
			int y = 0;
			foreach (Control control in Controls)
			{
				control.SetBounds(0, y, control.Width, control.Height);
				y += control.Height;
			}
			Height = y;
		}
	}
}
