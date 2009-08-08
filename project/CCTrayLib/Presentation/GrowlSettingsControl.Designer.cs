namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	partial class GrowlSettingsControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.checkBoxEnabled = new System.Windows.Forms.CheckBox();
			this.textBoxPassword = new System.Windows.Forms.TextBox();
			this.labelPassword = new System.Windows.Forms.Label();
			this.groupBoxRemoteInstance = new System.Windows.Forms.GroupBox();
			this.labelPort = new System.Windows.Forms.Label();
			this.textBoxPort = new System.Windows.Forms.TextBox();
			this.labelHostname = new System.Windows.Forms.Label();
			this.textBoxHostname = new System.Windows.Forms.TextBox();
			this.checkBoxRemoteGrowl = new System.Windows.Forms.CheckBox();
			this.comboMinNotificationLevel = new System.Windows.Forms.ComboBox();
			this.labelBalloonMinNotificationLevel = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBoxRemoteInstance.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkBoxEnabled
			// 
			this.checkBoxEnabled.AutoSize = true;
			this.checkBoxEnabled.Location = new System.Drawing.Point(17, 18);
			this.checkBoxEnabled.Name = "checkBoxEnabled";
			this.checkBoxEnabled.Size = new System.Drawing.Size(150, 17);
			this.checkBoxEnabled.TabIndex = 0;
			this.checkBoxEnabled.Text = "Enable Growl Notifications";
			this.checkBoxEnabled.UseVisualStyleBackColor = true;
			this.checkBoxEnabled.CheckedChanged += new System.EventHandler(this.checkBoxEnabled_CheckedChanged);
			// 
			// textBoxPassword
			// 
			this.textBoxPassword.Location = new System.Drawing.Point(105, 58);
			this.textBoxPassword.Name = "textBoxPassword";
			this.textBoxPassword.Size = new System.Drawing.Size(164, 20);
			this.textBoxPassword.TabIndex = 1;
			this.textBoxPassword.UseSystemPasswordChar = true;
			// 
			// labelPassword
			// 
			this.labelPassword.AutoSize = true;
			this.labelPassword.Location = new System.Drawing.Point(44, 123);
			this.labelPassword.Name = "labelPassword";
			this.labelPassword.Size = new System.Drawing.Size(56, 13);
			this.labelPassword.TabIndex = 2;
			this.labelPassword.Text = "Password:";
			// 
			// groupBoxRemoteInstance
			// 
			this.groupBoxRemoteInstance.Controls.Add(this.labelPort);
			this.groupBoxRemoteInstance.Controls.Add(this.textBoxPort);
			this.groupBoxRemoteInstance.Controls.Add(this.labelHostname);
			this.groupBoxRemoteInstance.Controls.Add(this.textBoxHostname);
			this.groupBoxRemoteInstance.Location = new System.Drawing.Point(297, 34);
			this.groupBoxRemoteInstance.Name = "groupBoxRemoteInstance";
			this.groupBoxRemoteInstance.Size = new System.Drawing.Size(246, 86);
			this.groupBoxRemoteInstance.TabIndex = 4;
			this.groupBoxRemoteInstance.TabStop = false;
			this.groupBoxRemoteInstance.Text = "Remote Growl Instance";
			// 
			// labelPort
			// 
			this.labelPort.AutoSize = true;
			this.labelPort.Location = new System.Drawing.Point(69, 53);
			this.labelPort.Name = "labelPort";
			this.labelPort.Size = new System.Drawing.Size(29, 13);
			this.labelPort.TabIndex = 6;
			this.labelPort.Text = "Port:";
			// 
			// textBoxPort
			// 
			this.textBoxPort.Location = new System.Drawing.Point(123, 50);
			this.textBoxPort.MaxLength = 5;
			this.textBoxPort.Name = "textBoxPort";
			this.textBoxPort.Size = new System.Drawing.Size(34, 20);
			this.textBoxPort.TabIndex = 5;
			this.textBoxPort.Text = "23053";
			this.textBoxPort.Leave += new System.EventHandler(this.textBoxPort_Leave);
			// 
			// labelHostname
			// 
			this.labelHostname.AutoSize = true;
			this.labelHostname.Location = new System.Drawing.Point(40, 27);
			this.labelHostname.Name = "labelHostname";
			this.labelHostname.Size = new System.Drawing.Size(58, 13);
			this.labelHostname.TabIndex = 4;
			this.labelHostname.Text = "Hostname:";
			// 
			// textBoxHostname
			// 
			this.textBoxHostname.Location = new System.Drawing.Point(123, 24);
			this.textBoxHostname.Name = "textBoxHostname";
			this.textBoxHostname.Size = new System.Drawing.Size(100, 20);
			this.textBoxHostname.TabIndex = 3;
			// 
			// checkBoxRemoteGrowl
			// 
			this.checkBoxRemoteGrowl.AutoSize = true;
			this.checkBoxRemoteGrowl.Location = new System.Drawing.Point(297, 11);
			this.checkBoxRemoteGrowl.Name = "checkBoxRemoteGrowl";
			this.checkBoxRemoteGrowl.Size = new System.Drawing.Size(246, 17);
			this.checkBoxRemoteGrowl.TabIndex = 5;
			this.checkBoxRemoteGrowl.Text = "Send Notifications to a Remote Growl instance";
			this.checkBoxRemoteGrowl.UseVisualStyleBackColor = true;
			this.checkBoxRemoteGrowl.CheckedChanged += new System.EventHandler(this.checkBoxRemoteGrowl_CheckedChanged);
			// 
			// comboMinNotificationLevel
			// 
			this.comboMinNotificationLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboMinNotificationLevel.FormattingEnabled = true;
			this.comboMinNotificationLevel.Location = new System.Drawing.Point(105, 9);
			this.comboMinNotificationLevel.Name = "comboMinNotificationLevel";
			this.comboMinNotificationLevel.Size = new System.Drawing.Size(164, 21);
			this.comboMinNotificationLevel.TabIndex = 6;
			// 
			// labelBalloonMinNotificationLevel
			// 
			this.labelBalloonMinNotificationLevel.AutoSize = true;
			this.labelBalloonMinNotificationLevel.Enabled = false;
			this.labelBalloonMinNotificationLevel.Location = new System.Drawing.Point(7, 12);
			this.labelBalloonMinNotificationLevel.Name = "labelBalloonMinNotificationLevel";
			this.labelBalloonMinNotificationLevel.Size = new System.Drawing.Size(92, 13);
			this.labelBalloonMinNotificationLevel.TabIndex = 7;
			this.labelBalloonMinNotificationLevel.Text = "Notification Level:";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupBoxRemoteInstance);
			this.panel1.Controls.Add(this.labelBalloonMinNotificationLevel);
			this.panel1.Controls.Add(this.checkBoxRemoteGrowl);
			this.panel1.Controls.Add(this.comboMinNotificationLevel);
			this.panel1.Controls.Add(this.textBoxPassword);
			this.panel1.Location = new System.Drawing.Point(37, 62);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(566, 171);
			this.panel1.TabIndex = 8;
			// 
			// GrowlSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelPassword);
			this.Controls.Add(this.checkBoxEnabled);
			this.Controls.Add(this.panel1);
			this.Name = "GrowlSettingsControl";
			this.Size = new System.Drawing.Size(667, 289);
			this.groupBoxRemoteInstance.ResumeLayout(false);
			this.groupBoxRemoteInstance.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBoxEnabled;
		private System.Windows.Forms.TextBox textBoxPassword;
		private System.Windows.Forms.Label labelPassword;
		private System.Windows.Forms.GroupBox groupBoxRemoteInstance;
		private System.Windows.Forms.CheckBox checkBoxRemoteGrowl;
		private System.Windows.Forms.Label labelPort;
		private System.Windows.Forms.TextBox textBoxPort;
		private System.Windows.Forms.Label labelHostname;
		private System.Windows.Forms.TextBox textBoxHostname;
		private System.Windows.Forms.ComboBox comboMinNotificationLevel;
		private System.Windows.Forms.Label labelBalloonMinNotificationLevel;
		private System.Windows.Forms.Panel panel1;
	}
}
