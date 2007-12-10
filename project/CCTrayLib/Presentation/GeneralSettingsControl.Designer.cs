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
			this.lblFixUserName = new System.Windows.Forms.Label();
			this.txtFixUserName = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.numPollPeriod)).BeginInit();
			this.SuspendLayout();
			// 
			// chkAlwaysOnTop
			// 
			this.chkAlwaysOnTop.AutoSize = true;
			this.chkAlwaysOnTop.Location = new System.Drawing.Point(270, 8);
			this.chkAlwaysOnTop.Name = "chkAlwaysOnTop";
			this.chkAlwaysOnTop.Size = new System.Drawing.Size(98, 17);
			this.chkAlwaysOnTop.TabIndex = 14;
			this.chkAlwaysOnTop.Text = "Always On Top";
			this.chkAlwaysOnTop.UseVisualStyleBackColor = true;
			// 
			// rdoWebPage
			// 
			this.rdoWebPage.Location = new System.Drawing.Point(184, 88);
			this.rdoWebPage.Name = "rdoWebPage";
			this.rdoWebPage.Size = new System.Drawing.Size(310, 20);
			this.rdoWebPage.TabIndex = 13;
			this.rdoWebPage.Text = "navigate to the web page of the first project on the list";
			// 
			// rdoStatusWindow
			// 
			this.rdoStatusWindow.Location = new System.Drawing.Point(184, 64);
			this.rdoStatusWindow.Name = "rdoStatusWindow";
			this.rdoStatusWindow.Size = new System.Drawing.Size(230, 20);
			this.rdoStatusWindow.TabIndex = 12;
			this.rdoStatusWindow.Text = "show the status window";
			// 
			// lblDoubleClickAction
			// 
			this.lblDoubleClickAction.Location = new System.Drawing.Point(8, 64);
			this.lblDoubleClickAction.Name = "lblDoubleClickAction";
			this.lblDoubleClickAction.Size = new System.Drawing.Size(185, 20);
			this.lblDoubleClickAction.TabIndex = 11;
			this.lblDoubleClickAction.Text = "When I double-click the tray icon,";
			this.lblDoubleClickAction.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// numPollPeriod
			// 
			this.numPollPeriod.Location = new System.Drawing.Point(104, 40);
			this.numPollPeriod.Name = "numPollPeriod";
			this.numPollPeriod.Size = new System.Drawing.Size(50, 20);
			this.numPollPeriod.TabIndex = 9;
			// 
			// lblSeconds
			// 
			this.lblSeconds.Location = new System.Drawing.Point(168, 40);
			this.lblSeconds.Name = "lblSeconds";
			this.lblSeconds.Size = new System.Drawing.Size(100, 20);
			this.lblSeconds.TabIndex = 10;
			this.lblSeconds.Text = "seconds";
			// 
			// lblPoll
			// 
			this.lblPoll.Location = new System.Drawing.Point(8, 40);
			this.lblPoll.Name = "lblPoll";
			this.lblPoll.Size = new System.Drawing.Size(100, 20);
			this.lblPoll.TabIndex = 8;
			this.lblPoll.Text = "Poll servers every";
			// 
			// chkShowBalloons
			// 
			this.chkShowBalloons.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkShowBalloons.Location = new System.Drawing.Point(8, 8);
			this.chkShowBalloons.Name = "chkShowBalloons";
			this.chkShowBalloons.Size = new System.Drawing.Size(248, 24);
			this.chkShowBalloons.TabIndex = 7;
			this.chkShowBalloons.Text = "Show balloon notifications";
			// 
			// lblFixUserName
			// 
			this.lblFixUserName.AutoSize = true;
			this.lblFixUserName.Location = new System.Drawing.Point(14, 134);
			this.lblFixUserName.Name = "lblFixUserName";
			this.lblFixUserName.Size = new System.Drawing.Size(148, 13);
			this.lblFixUserName.TabIndex = 15;
			this.lblFixUserName.Text = "User Name for Fixing the build";
			// 
			// txtFixUserName
			// 
			this.txtFixUserName.Location = new System.Drawing.Point(171, 133);
			this.txtFixUserName.Name = "txtFixUserName";
			this.txtFixUserName.Size = new System.Drawing.Size(133, 20);
			this.txtFixUserName.TabIndex = 16;
			// 
			// lblFixUserName
			// 
			this.lblFixUserName.AutoSize = true;
			this.lblFixUserName.Location = new System.Drawing.Point(14, 134);
			this.lblFixUserName.Name = "lblFixUserName";
			this.lblFixUserName.Size = new System.Drawing.Size(148, 13);
			this.lblFixUserName.TabIndex = 15;
			this.lblFixUserName.Text = "User Name for Fixing the build";
			// 
			// txtFixUserName
			// 
			this.txtFixUserName.Location = new System.Drawing.Point(171, 133);
			this.txtFixUserName.Name = "txtFixUserName";
			this.txtFixUserName.Size = new System.Drawing.Size(133, 20);
			this.txtFixUserName.TabIndex = 16;
			// 
			// GeneralSettingsControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.rdoWebPage);
			this.Controls.Add(this.rdoStatusWindow);
			this.Controls.Add(this.numPollPeriod);
			this.Controls.Add(this.lblDoubleClickAction);
			this.Controls.Add(this.lblSeconds);
			this.Controls.Add(this.lblPoll);
			this.Controls.Add(this.chkAlwaysOnTop);
			this.Controls.Add(this.chkShowBalloons);
			this.Controls.Add(this.lblFixUserName);
			this.Controls.Add(this.txtFixUserName);
			this.Name = "GeneralSettingsontrol";
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
		private Label lblFixUserName;
		private TextBox txtFixUserName;
	}
}
