using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class ConfigurationItemControl : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ComboBox comboBox2;
		private System.Windows.Forms.TextBox textBox2;
		private ConfigurationItem _item;
		private System.Windows.Forms.Panel simplePanel;
		private BunchOfConfigurationItemControls childPanel;
		private int heightWithoutChildren;

		private System.ComponentModel.Container components = null;

		public ConfigurationItemControl()
		{
			InitializeComponent();

			textBox1.TextChanged += new EventHandler(ValueChanged);
			textBox2.TextChanged += new EventHandler(ValueChanged);
			
			comboBox1.SelectedValueChanged += new EventHandler(ValueChanged);
			comboBox2.SelectedValueChanged += new EventHandler(ValueChanged);

			heightWithoutChildren = Height - childPanel.Height;
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
			this.groupBox = new System.Windows.Forms.GroupBox();
			this.comboBox2 = new System.Windows.Forms.ComboBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.childPanel = new BunchOfConfigurationItemControls();
			this.simplePanel = new System.Windows.Forms.Panel();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.groupBox.SuspendLayout();
			this.simplePanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox
			// 
			this.groupBox.Controls.Add(this.comboBox2);
			this.groupBox.Controls.Add(this.textBox2);
			this.groupBox.Controls.Add(this.childPanel);
			this.groupBox.Location = new System.Drawing.Point(0, 0);
			this.groupBox.Name = "groupBox";
			this.groupBox.Size = new System.Drawing.Size(600, 358);
			this.groupBox.TabIndex = 3;
			this.groupBox.TabStop = false;
			this.groupBox.Text = "CVS";
			// 
			// comboBox2
			// 
			this.comboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox2.Location = new System.Drawing.Point(8, 16);
			this.comboBox2.Name = "comboBox2";
			this.comboBox2.Size = new System.Drawing.Size(448, 21);
			this.comboBox2.TabIndex = 2;
			// 
			// textBox2
			// 
			this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox2.Location = new System.Drawing.Point(8, 16);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(448, 20);
			this.textBox2.TabIndex = 0;
			this.textBox2.Text = "";
			// 
			// childPanel
			// 
			this.childPanel.Location = new System.Drawing.Point(8, 40);
			this.childPanel.Name = "childPanel";
			this.childPanel.Size = new System.Drawing.Size(584, 310);
			this.childPanel.TabIndex = 4;
			// 
			// simplePanel
			// 
			this.simplePanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.simplePanel.Controls.Add(this.comboBox1);
			this.simplePanel.Controls.Add(this.label1);
			this.simplePanel.Controls.Add(this.textBox1);
			this.simplePanel.Location = new System.Drawing.Point(0, 0);
			this.simplePanel.Name = "simplePanel";
			this.simplePanel.Size = new System.Drawing.Size(600, 24);
			this.simplePanel.TabIndex = 0;
			// 
			// comboBox1
			// 
			this.comboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Location = new System.Drawing.Point(152, 0);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(448, 21);
			this.comboBox1.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "Some Property:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBox1
			// 
			this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.textBox1.Location = new System.Drawing.Point(152, 0);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(448, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// ConfigurationItemControl
			// 
			this.Controls.Add(this.groupBox);
			this.Controls.Add(this.simplePanel);
			this.Name = "ConfigurationItemControl";
			this.Size = new System.Drawing.Size(600, 360);
			this.groupBox.ResumeLayout(false);
			this.simplePanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void Bind(ConfigurationItem item, ComboBox comboBox, TextBox textBox) 
		{
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
		}

		public void Bind(ConfigurationItem item) 
		{
			_item = null;

			bool hasChildren = item.Items.Count > 0;

			if (!hasChildren)
			{
				simplePanel.Visible = true;
				groupBox.Visible = false;
				Bind(item, comboBox1, textBox1);
				label1.Text = item.Name + " :";

				Height = comboBox1.Height + 2;
			}
			else
			{
				simplePanel.Visible = false;
				groupBox.Visible = true;
				Bind(item, comboBox2, textBox2);
				groupBox.Text = item.Name;

				childPanel.Bind(item.Items);

				Height = heightWithoutChildren + childPanel.Height;
			}

			_item = item;
		}

		public void Bind(ConfigurationItemCollection items) 
		{
			foreach (Control control in childPanel.Controls)
			{
				control.Resize -= new EventHandler(ChildControlResized);
			}
			childPanel.Controls.Clear();

			foreach (ConfigurationItem childItem in items)
			{
				ConfigurationItemControl childControl = new ConfigurationItemControl();
				childControl.Bind(childItem);
				childPanel.Controls.Add(childControl);
				childControl.Resize += new EventHandler(ChildControlResized);
			}
			ChildControlResized(null, EventArgs.Empty);
		}

		private void ChildControlResized(object sender, EventArgs e)
		{
			int y = 0;
			foreach (Control control in childPanel.Controls)
			{
				control.SetBounds(0, y, control.Width, control.Height);
				y += control.Height;
			}
			childPanel.Height = y;
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			if (_item == null) return;
			_item.ValueAsString = ((Control) sender).Text;
			Bind(_item);
		}
	}
}