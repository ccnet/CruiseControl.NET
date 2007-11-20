using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	partial class AudioSettingsControl
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
			this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.chkAudioSuccessful = new System.Windows.Forms.CheckBox();
			this.btnStillFailingPlay = new System.Windows.Forms.Button();
			this.txtAudioFileSuccess = new System.Windows.Forms.TextBox();
			this.btnFixedPlay = new System.Windows.Forms.Button();
			this.chkAudioFixed = new System.Windows.Forms.CheckBox();
			this.btnFixedBrowse = new System.Windows.Forms.Button();
			this.btnStillFailingBrowse = new System.Windows.Forms.Button();
			this.btnBrokenPlay = new System.Windows.Forms.Button();
			this.btnBrokenBrowse = new System.Windows.Forms.Button();
			this.btnSuccessfulPlay = new System.Windows.Forms.Button();
			this.btnSuccessfulBrowse = new System.Windows.Forms.Button();
			this.chkAudioBroken = new System.Windows.Forms.CheckBox();
			this.chkAudioStillFailing = new System.Windows.Forms.CheckBox();
			this.txtAudioFileFixed = new System.Windows.Forms.TextBox();
			this.txtAudioFileBroken = new System.Windows.Forms.TextBox();
			this.txtAudioFileFailing = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// dlgOpenFile
			// 
			this.dlgOpenFile.DefaultExt = "wav";
			this.dlgOpenFile.Filter = "Wave Files|*.wav|All Files|*.*";
			this.dlgOpenFile.Title = "Select wave file";
			// 
			// chkAudioSuccessful
			// 
			this.chkAudioSuccessful.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioSuccessful.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioSuccessful.Location = new System.Drawing.Point(16, 16);
			this.chkAudioSuccessful.Name = "chkAudioSuccessful";
			this.chkAudioSuccessful.Size = new System.Drawing.Size(96, 16);
			this.chkAudioSuccessful.TabIndex = 0;
			this.chkAudioSuccessful.Text = "Successful";
			this.chkAudioSuccessful.UseVisualStyleBackColor = false;
			// 
			// btnStillFailingPlay
			// 
			this.btnStillFailingPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStillFailingPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnStillFailingPlay.Location = new System.Drawing.Point(560, 88);
			this.btnStillFailingPlay.Name = "btnStillFailingPlay";
			this.btnStillFailingPlay.Size = new System.Drawing.Size(75, 23);
			this.btnStillFailingPlay.TabIndex = 15;
			this.btnStillFailingPlay.Text = "Play!";
			// 
			// txtAudioFileSuccess
			// 
			this.txtAudioFileSuccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileSuccess.Location = new System.Drawing.Point(112, 16);
			this.txtAudioFileSuccess.Name = "txtAudioFileSuccess";
			this.txtAudioFileSuccess.Size = new System.Drawing.Size(353, 20);
			this.txtAudioFileSuccess.TabIndex = 1;
			// 
			// btnFixedPlay
			// 
			this.btnFixedPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFixedPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnFixedPlay.Location = new System.Drawing.Point(560, 40);
			this.btnFixedPlay.Name = "btnFixedPlay";
			this.btnFixedPlay.Size = new System.Drawing.Size(75, 23);
			this.btnFixedPlay.TabIndex = 7;
			this.btnFixedPlay.Text = "Play!";
			// 
			// chkAudioFixed
			// 
			this.chkAudioFixed.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioFixed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioFixed.Location = new System.Drawing.Point(16, 40);
			this.chkAudioFixed.Name = "chkAudioFixed";
			this.chkAudioFixed.Size = new System.Drawing.Size(96, 16);
			this.chkAudioFixed.TabIndex = 4;
			this.chkAudioFixed.Text = "Fixed";
			this.chkAudioFixed.UseVisualStyleBackColor = false;
			// 
			// btnFixedBrowse
			// 
			this.btnFixedBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFixedBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnFixedBrowse.Location = new System.Drawing.Point(472, 40);
			this.btnFixedBrowse.Name = "btnFixedBrowse";
			this.btnFixedBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnFixedBrowse.TabIndex = 6;
			this.btnFixedBrowse.Text = "Browse...";
			// 
			// btnStillFailingBrowse
			// 
			this.btnStillFailingBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStillFailingBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnStillFailingBrowse.Location = new System.Drawing.Point(472, 88);
			this.btnStillFailingBrowse.Name = "btnStillFailingBrowse";
			this.btnStillFailingBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnStillFailingBrowse.TabIndex = 14;
			this.btnStillFailingBrowse.Text = "Browse...";
			// 
			// btnBrokenPlay
			// 
			this.btnBrokenPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrokenPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrokenPlay.Location = new System.Drawing.Point(560, 64);
			this.btnBrokenPlay.Name = "btnBrokenPlay";
			this.btnBrokenPlay.Size = new System.Drawing.Size(75, 23);
			this.btnBrokenPlay.TabIndex = 11;
			this.btnBrokenPlay.Text = "Play!";
			// 
			// btnBrokenBrowse
			// 
			this.btnBrokenBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrokenBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrokenBrowse.Location = new System.Drawing.Point(472, 62);
			this.btnBrokenBrowse.Name = "btnBrokenBrowse";
			this.btnBrokenBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnBrokenBrowse.TabIndex = 10;
			this.btnBrokenBrowse.Text = "Browse...";
			// 
			// btnSuccessfulPlay
			// 
			this.btnSuccessfulPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSuccessfulPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSuccessfulPlay.Location = new System.Drawing.Point(560, 16);
			this.btnSuccessfulPlay.Name = "btnSuccessfulPlay";
			this.btnSuccessfulPlay.Size = new System.Drawing.Size(75, 23);
			this.btnSuccessfulPlay.TabIndex = 3;
			this.btnSuccessfulPlay.Text = "Play!";
			// 
			// btnSuccessfulBrowse
			// 
			this.btnSuccessfulBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSuccessfulBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSuccessfulBrowse.Location = new System.Drawing.Point(472, 16);
			this.btnSuccessfulBrowse.Name = "btnSuccessfulBrowse";
			this.btnSuccessfulBrowse.Size = new System.Drawing.Size(75, 23);
			this.btnSuccessfulBrowse.TabIndex = 2;
			this.btnSuccessfulBrowse.Text = "Browse...";
			// 
			// chkAudioBroken
			// 
			this.chkAudioBroken.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioBroken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioBroken.Location = new System.Drawing.Point(16, 64);
			this.chkAudioBroken.Name = "chkAudioBroken";
			this.chkAudioBroken.Size = new System.Drawing.Size(96, 16);
			this.chkAudioBroken.TabIndex = 8;
			this.chkAudioBroken.Text = "Broken";
			this.chkAudioBroken.UseVisualStyleBackColor = false;
			// 
			// chkAudioStillFailing
			// 
			this.chkAudioStillFailing.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioStillFailing.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioStillFailing.Location = new System.Drawing.Point(16, 88);
			this.chkAudioStillFailing.Name = "chkAudioStillFailing";
			this.chkAudioStillFailing.Size = new System.Drawing.Size(96, 16);
			this.chkAudioStillFailing.TabIndex = 12;
			this.chkAudioStillFailing.Text = "Still failing";
			this.chkAudioStillFailing.UseVisualStyleBackColor = false;
			// 
			// txtAudioFileFixed
			// 
			this.txtAudioFileFixed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileFixed.Location = new System.Drawing.Point(112, 40);
			this.txtAudioFileFixed.Name = "txtAudioFileFixed";
			this.txtAudioFileFixed.Size = new System.Drawing.Size(353, 20);
			this.txtAudioFileFixed.TabIndex = 5;
			// 
			// txtAudioFileBroken
			// 
			this.txtAudioFileBroken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileBroken.Location = new System.Drawing.Point(112, 64);
			this.txtAudioFileBroken.Name = "txtAudioFileBroken";
			this.txtAudioFileBroken.Size = new System.Drawing.Size(353, 20);
			this.txtAudioFileBroken.TabIndex = 9;
			// 
			// txtAudioFileFailing
			// 
			this.txtAudioFileFailing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileFailing.Location = new System.Drawing.Point(112, 88);
			this.txtAudioFileFailing.Name = "txtAudioFileFailing";
			this.txtAudioFileFailing.Size = new System.Drawing.Size(353, 20);
			this.txtAudioFileFailing.TabIndex = 13;
			// 
			// AudioSettingsControl
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.Controls.Add(this.chkAudioSuccessful);
			this.Controls.Add(this.btnStillFailingPlay);
			this.Controls.Add(this.txtAudioFileSuccess);
			this.Controls.Add(this.btnFixedPlay);
			this.Controls.Add(this.chkAudioFixed);
			this.Controls.Add(this.btnFixedBrowse);
			this.Controls.Add(this.btnStillFailingBrowse);
			this.Controls.Add(this.btnBrokenPlay);
			this.Controls.Add(this.btnBrokenBrowse);
			this.Controls.Add(this.btnSuccessfulPlay);
			this.Controls.Add(this.btnSuccessfulBrowse);
			this.Controls.Add(this.chkAudioBroken);
			this.Controls.Add(this.chkAudioStillFailing);
			this.Controls.Add(this.txtAudioFileFixed);
			this.Controls.Add(this.txtAudioFileBroken);
			this.Controls.Add(this.txtAudioFileFailing);
			this.Name = "AudioSettingsControl";
			this.Size = new System.Drawing.Size(667, 289);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Button btnStillFailingPlay;
		private TextBox txtAudioFileSuccess;
		private Button btnFixedPlay;
		private CheckBox chkAudioFixed;
		private Button btnFixedBrowse;
		private Button btnStillFailingBrowse;
		private Button btnBrokenPlay;
		private Button btnBrokenBrowse;
		private Button btnSuccessfulPlay;
		private Button btnSuccessfulBrowse;
		private CheckBox chkAudioBroken;
		private CheckBox chkAudioStillFailing;
		private TextBox txtAudioFileFixed;
		private TextBox txtAudioFileBroken;
		private TextBox txtAudioFileFailing;
		private CheckBox chkAudioSuccessful;
		private OpenFileDialog dlgOpenFile;
	}
}
