using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class SterileConfigurationItemControl : System.Windows.Forms.UserControl
	{
		private ConfigurationItem _item;
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.ComboBox comboBox;
		private System.Windows.Forms.TextBox textBox;

		private System.ComponentModel.Container components = null;

		public SterileConfigurationItemControl()
		{
			InitializeComponent();

			textBox.TextChanged += new EventHandler(ValueChanged);
			comboBox.SelectedValueChanged += new EventHandler(ValueChanged);
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.comboBox = new System.Windows.Forms.ComboBox();
			this.label = new System.Windows.Forms.Label();
			this.textBox = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// comboBox
			// 
			this.comboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox.Location = new System.Drawing.Point(152, 0);
			this.comboBox.Name = "comboBox";
			this.comboBox.Size = new System.Drawing.Size(448, 21);
			this.comboBox.TabIndex = 2;
			// 
			// label
			// 
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(152, 20);
			this.label.TabIndex = 1;
			this.label.Text = "Some Property:";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.Location = new System.Drawing.Point(152, 0);
			this.textBox.Name = "textBox";
			this.textBox.Size = new System.Drawing.Size(448, 20);
			this.textBox.TabIndex = 0;
			this.textBox.Text = "";
			// 
			// SterileConfigurationItemControl
			// 
			this.Controls.Add(this.comboBox);
			this.Controls.Add(this.label);
			this.Controls.Add(this.textBox);
			this.Name = "SterileConfigurationItemControl";
			this.Size = new System.Drawing.Size(600, 24);
			this.ResumeLayout(false);

		}
		#endregion

		public void Bind(ConfigurationItem item) 
		{
			_item = null;
			label.Text = item.Name + " : ";
			if (item.AvailableValues == null) 
			{
				comboBox.Visible = false;
				textBox.Visible = true;
				textBox.Text = item.ValueAsString;
			}
			else
			{
				textBox.Visible = false;
				comboBox.Visible = true;
				comboBox.Items.Clear();
				comboBox.Items.AddRange(item.AvailableValues);
				comboBox.SelectedItem = item.ValueAsString;
			}
			_item = item;
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			if (_item == null) return;
			_item.ValueAsString = ((Control) sender).Text;
		}
	}
}