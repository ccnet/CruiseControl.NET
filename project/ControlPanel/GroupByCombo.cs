using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	/// <summary>
	/// Summary description for GroupByCombo.
	/// </summary>
	public class GroupByCombo : System.Windows.Forms.UserControl
	{
		private GroupBox subpanel;
		public System.Windows.Forms.ComboBox comboBox;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GroupByCombo()
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
			this.subpanel = new System.Windows.Forms.GroupBox();
			this.comboBox = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// subpanel
			// 
			this.subpanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.subpanel.Location = new System.Drawing.Point(0, 5);
			this.subpanel.Name = "subpanel";
			this.subpanel.Size = new System.Drawing.Size(432, 144);
			this.subpanel.TabIndex = 0;
			this.subpanel.TabStop = false;
			// 
			// comboBox
			// 
			this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox.Location = new System.Drawing.Point(16, 0);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new System.Drawing.Size(208, 21);
			this.comboBox.TabIndex = 1;
			// 
			// GroupByCombo
			// 
			this.Controls.Add(this.comboBox);
			this.Controls.Add(this.subpanel);
			this.Name = "GroupByCombo";
			this.Size = new System.Drawing.Size(432, 152);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
