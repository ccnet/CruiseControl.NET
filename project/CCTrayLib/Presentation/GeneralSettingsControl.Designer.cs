using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	partial class GeneralSettingsControl
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
			this.rdoWebPage = new System.Windows.Forms.RadioButton();
			this.rdoStatusWindow = new System.Windows.Forms.RadioButton();
			this.numPollPeriod = new System.Windows.Forms.NumericUpDown();
			this.lblDoubleClickAction = new System.Windows.Forms.Label();
			this.lblSeconds = new System.Windows.Forms.Label();
			this.lblPoll = new System.Windows.Forms.Label();
			this.chkAlwaysOnTop = new System.Windows.Forms.CheckBox();
			this.chkShowBalloons = new System.Windows.Forms.CheckBox();
			this.chkShowInTaskbar = new System.Windows.Forms.CheckBox();
			this.lblFixUserName = new System.Windows.Forms.Label();
			this.txtFixUserName = new System.Windows.Forms.TextBox();
			this.labelBalloonMinNotificationLevel = new System.Windows.Forms.Label();
			this.comboBalloonMinNotificationLevel = new System.Windows.Forms.ComboBox();
			((System.ComponentModel.ISupportInitialize)(this.numPollPeriod)).BeginInit();
			this.SuspendLayout();
			// 
			// rdoWebPage
			// 
			this.rdoWebPage.Location = new System.Drawing.Point(184, 90);
			this.rdoWebPage.Name = "rdoWebPage";
			this.rdoWebPage.Size = new System.Drawing.Size(310, 20);
			this.rdoWebPage.TabIndex = 8;
			this.rdoWebPage.Text = "navigate to the web page of the first project on the list";
			// 
			// rdoStatusWindow
			// 
			this.rdoStatusWindow.Location = new System.Drawing.Point(184, 64);
			this.rdoStatusWindow.Name = "rdoStatusWindow";
			this.rdoStatusWindow.Size = new System.Drawing.Size(230, 20);
			this.rdoStatusWindow.TabIndex = 7;
			this.rdoStatusWindow.Text = "show the status window";
			// 
			// numPollPeriod
			// 
			this.numPollPeriod.Location = new System.Drawing.Point(106, 38);
			this.numPollPeriod.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
			this.numPollPeriod.Name = "numPollPeriod";
			this.numPollPeriod.Size = new System.Drawing.Size(50, 20);
			this.numPollPeriod.TabIndex = 4;
			this.numPollPeriod.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
			// 
			// lblDoubleClickAction
			// 
			this.lblDoubleClickAction.Location = new System.Drawing.Point(5, 64);
			this.lblDoubleClickAction.Name = "lblDoubleClickAction";
			this.lblDoubleClickAction.Size = new System.Drawing.Size(180, 20);
			this.lblDoubleClickAction.TabIndex = 6;
			this.lblDoubleClickAction.Text = "When I double-click the tray icon,";
			this.lblDoubleClickAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblSeconds
			// 
			this.lblSeconds.Location = new System.Drawing.Point(168, 40);
			this.lblSeconds.Name = "lblSeconds";
			this.lblSeconds.Size = new System.Drawing.Size(100, 20);
			this.lblSeconds.TabIndex = 5;
			this.lblSeconds.Text = "seconds";
			// 
			// lblPoll
			// 
			this.lblPoll.Location = new System.Drawing.Point(8, 40);
			this.lblPoll.Name = "lblPoll";
			this.lblPoll.Size = new System.Drawing.Size(100, 20);
			this.lblPoll.TabIndex = 3;
			this.lblPoll.Text = "Poll servers every";
			// 
			// chkAlwaysOnTop
			// 
			this.chkAlwaysOnTop.AutoSize = true;
			this.chkAlwaysOnTop.Location = new System.Drawing.Point(8, 184);
			this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
			this.chkAlwaysOnTop.Size = new System.Drawing.Size(98, 17);
			this.chkAlwaysOnTop.TabIndex = 11;
			this.chkAlwaysOnTop.Text = "Always On Top";
			this.chkAlwaysOnTop.UseVisualStyleBackColor = true;
			// 
			// chkShowBalloons
			// 
			this.chkShowBalloons.AutoSize = true;
			this.chkShowBalloons.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkShowBalloons.Location = new System.Drawing.Point(7, 11);
			this.chkShowBalloons.Name = "chkShowBalloons";
			this.chkShowBalloons.Size = new System.Drawing.Size(155, 18);
			this.chkShowBalloons.TabIndex = 0;
			this.chkShowBalloons.Text = "Show balloon notifications";
			this.chkShowBalloons.UseVisualStyleBackColor = true;
			this.chkShowBalloons.CheckedChanged += new System.EventHandler(this.chkShowBalloons_CheckedChanged);
			// 
			// chkShowInTaskbar
			// 
			this.chkShowInTaskbar.AutoSize = true;
			this.chkShowInTaskbar.Location = new System.Drawing.Point(8, 207);
			this.chkShowInTaskbar.Name = "chkShowInTaskbar";
			this.chkShowInTaskbar.Size = new System.Drawing.Size(106, 17);
			this.chkShowInTaskbar.TabIndex = 12;
			this.chkShowInTaskbar.Text = "Show in Taskbar";
			this.chkShowInTaskbar.UseVisualStyleBackColor = true;
			// 
			// lblFixUserName
			// 
			this.lblFixUserName.AutoSize = true;
			this.lblFixUserName.Location = new System.Drawing.Point(8, 134);
			this.lblFixUserName.Name = "lblFixUserName";
			this.lblFixUserName.Size = new System.Drawing.Size(148, 13);
			this.lblFixUserName.TabIndex = 9;
			this.lblFixUserName.Text = "User Name for Fixing the build";
			// 
			// txtFixUserName
			// 
			this.txtFixUserName.Location = new System.Drawing.Point(165, 131);
			this.txtFixUserName.Name = "txtFixUserName";
			this.txtFixUserName.Size = new System.Drawing.Size(133, 20);
			this.txtFixUserName.TabIndex = 10;
			// 
			// labelBalloonMinNotificationLevel
			// 
			this.labelBalloonMinNotificationLevel.AutoSize = true;
			this.labelBalloonMinNotificationLevel.Enabled = false;
			this.labelBalloonMinNotificationLevel.Location = new System.Drawing.Point(168, 13);
			this.labelBalloonMinNotificationLevel.Name = "labelBalloonMinNotificationLevel";
			this.labelBalloonMinNotificationLevel.Size = new System.Drawing.Size(92, 13);
			this.labelBalloonMinNotificationLevel.TabIndex = 1;
			this.labelBalloonMinNotificationLevel.Text = "Notification Level:";
			// 
			// comboBalloonMinNotificationLevel
			// 
			this.comboBalloonMinNotificationLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBalloonMinNotificationLevel.Enabled = false;
			this.comboBalloonMinNotificationLevel.FormattingEnabled = true;
			this.comboBalloonMinNotificationLevel.Location = new System.Drawing.Point(266, 10);
			this.comboBalloonMinNotificationLevel.Name = "comboBalloonMinNotificationLevel";
			this.comboBalloonMinNotificationLevel.Size = new System.Drawing.Size(164, 21);
			this.comboBalloonMinNotificationLevel.TabIndex = 2;
			// 
			// GeneralSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.comboBalloonMinNotificationLevel);
			this.Controls.Add(this.labelBalloonMinNotificationLevel);
			this.Controls.Add(this.rdoWebPage);
			this.Controls.Add(this.rdoStatusWindow);
			this.Controls.Add(this.numPollPeriod);
			this.Controls.Add(this.lblDoubleClickAction);
			this.Controls.Add(this.lblSeconds);
			this.Controls.Add(this.lblPoll);
			this.Controls.Add(this.chkAlwaysOnTop);
			this.Controls.Add(this.chkShowBalloons);
			this.Controls.Add(this.chkShowInTaskbar);
			this.Controls.Add(this.lblFixUserName);
			this.Controls.Add(this.txtFixUserName);
			this.Name = "GeneralSettingsControl";
			this.Size = new System.Drawing.Size(667, 289);
			((System.ComponentModel.ISupportInitialize)(this.numPollPeriod)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private RadioButton rdoWebPage;
		private RadioButton rdoStatusWindow;
		protected NumericUpDown numPollPeriod;
		private Label lblDoubleClickAction;
		private Label lblSeconds;
		private Label lblPoll;
		private CheckBox chkAlwaysOnTop;
		private CheckBox chkShowBalloons;
		private CheckBox chkShowInTaskbar;
		private Label lblFixUserName;
		private TextBox txtFixUserName;
        private Label labelBalloonMinNotificationLevel;
        private ComboBox comboBalloonMinNotificationLevel;
	}
}
