using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.ControlPanel
{
	public class FertileConfigurationItemControl : System.Windows.Forms.UserControl
	{
		private ConfigurationItem _item;
		private GroupByCombo groupByBox;
		private System.Windows.Forms.Label label;
		private BunchOfConfigurationItemControls childrenPanel;

		private System.ComponentModel.Container components = null;

		public FertileConfigurationItemControl()
		{
			InitializeComponent();

			groupByBox.comboBox.SelectedValueChanged += new EventHandler(ValueChanged);
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
			this.groupByBox = new ThoughtWorks.CruiseControl.ControlPanel.GroupByCombo();
			this.label = new System.Windows.Forms.Label();
			this.childrenPanel = new ThoughtWorks.CruiseControl.ControlPanel.BunchOfConfigurationItemControls();
			this.SuspendLayout();
			// 
			// groupByBox
			// 
			this.groupByBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.groupByBox.Location = new System.Drawing.Point(8, 32);
			this.groupByBox.Name = "groupByBox";
			this.groupByBox.Size = new System.Drawing.Size(584, 224);
			this.groupByBox.TabIndex = 0;
			// 
			// label
			// 
			this.label.BackColor = System.Drawing.Color.LightSteelBlue;
			this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(600, 24);
			this.label.TabIndex = 1;
			this.label.Text = "That\'s Too Bad";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// childrenPanel
			// 
			this.childrenPanel.Location = new System.Drawing.Point(24, 64);
			this.childrenPanel.Name = "childrenPanel";
			this.childrenPanel.Size = new System.Drawing.Size(560, 184);
			this.childrenPanel.TabIndex = 2;
			// 
			// FertileConfigurationItemControl
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.childrenPanel);
			this.Controls.Add(this.label);
			this.Controls.Add(this.groupByBox);
			this.Name = "FertileConfigurationItemControl";
			this.Size = new System.Drawing.Size(600, 264);
			this.ResumeLayout(false);

		}
		#endregion

		public void Bind(ConfigurationItem item) 
		{
			_item = null;

			label.Text = item.Name;

			groupByBox.comboBox.Items.Clear();
			groupByBox.comboBox.Items.AddRange(item.AvailableValues);
			groupByBox.comboBox.SelectedItem = item.ValueAsString;
			
			int childrenHeight = childrenPanel.Height;
			childrenPanel.Bind(item.Items.ToArray());
			Height = Height - childrenHeight + childrenPanel.Height;

			_item = item;
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			if (_item == null) return;
			_item.ValueAsString = ((Control) sender).Text;
			Bind(_item);
		}
	}
}