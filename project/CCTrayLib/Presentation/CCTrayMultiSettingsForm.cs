using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	/// <summary>
	/// Summary description for CCTrayMultiSettingsForm.
	/// </summary>
	public class CCTrayMultiSettingsForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox grpAudio;
		private System.Windows.Forms.Button btnFindAudioSuccess;
		private System.Windows.Forms.TextBox txtAudioFileSuccess;
		private System.Windows.Forms.CheckBox chkAudioSuccessful;
		private System.Windows.Forms.CheckBox chkAudioBroken;
		private System.Windows.Forms.CheckBox chkAudioFixed;
		private System.Windows.Forms.CheckBox chkAudioStillFailing;
		private System.Windows.Forms.TextBox txtAudioFileFixed;
		private System.Windows.Forms.TextBox txtAudioFileBroken;
		private System.Windows.Forms.TextBox txtAudioFileFailing;
		private System.Windows.Forms.Button btnFindAudioFixed;
		private System.Windows.Forms.Button btnFindAudioBroken;
		private System.Windows.Forms.Button btnFindAudioFailing;
		private System.Windows.Forms.Button btnPlayBroken;
		private System.Windows.Forms.Button btnPlayFailing;
		private System.Windows.Forms.Button btnPlayFixed;
		private System.Windows.Forms.Button btnPlaySuccess;
		private System.Windows.Forms.CheckBox chkShowBalloons;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public CCTrayMultiSettingsForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(CCTrayMultiSettingsForm));
			this.grpAudio = new System.Windows.Forms.GroupBox();
			this.btnFindAudioSuccess = new System.Windows.Forms.Button();
			this.txtAudioFileSuccess = new System.Windows.Forms.TextBox();
			this.chkAudioSuccessful = new System.Windows.Forms.CheckBox();
			this.chkAudioBroken = new System.Windows.Forms.CheckBox();
			this.chkAudioFixed = new System.Windows.Forms.CheckBox();
			this.chkAudioStillFailing = new System.Windows.Forms.CheckBox();
			this.txtAudioFileFixed = new System.Windows.Forms.TextBox();
			this.txtAudioFileBroken = new System.Windows.Forms.TextBox();
			this.txtAudioFileFailing = new System.Windows.Forms.TextBox();
			this.btnFindAudioFixed = new System.Windows.Forms.Button();
			this.btnFindAudioBroken = new System.Windows.Forms.Button();
			this.btnFindAudioFailing = new System.Windows.Forms.Button();
			this.btnPlayBroken = new System.Windows.Forms.Button();
			this.btnPlayFailing = new System.Windows.Forms.Button();
			this.btnPlayFixed = new System.Windows.Forms.Button();
			this.btnPlaySuccess = new System.Windows.Forms.Button();
			this.chkShowBalloons = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.grpAudio.SuspendLayout();
			this.SuspendLayout();
			// 
			// grpAudio
			// 
			this.grpAudio.Controls.Add(this.btnFindAudioSuccess);
			this.grpAudio.Controls.Add(this.txtAudioFileSuccess);
			this.grpAudio.Controls.Add(this.chkAudioSuccessful);
			this.grpAudio.Controls.Add(this.chkAudioBroken);
			this.grpAudio.Controls.Add(this.chkAudioFixed);
			this.grpAudio.Controls.Add(this.chkAudioStillFailing);
			this.grpAudio.Controls.Add(this.txtAudioFileFixed);
			this.grpAudio.Controls.Add(this.txtAudioFileBroken);
			this.grpAudio.Controls.Add(this.txtAudioFileFailing);
			this.grpAudio.Controls.Add(this.btnFindAudioFixed);
			this.grpAudio.Controls.Add(this.btnFindAudioBroken);
			this.grpAudio.Controls.Add(this.btnFindAudioFailing);
			this.grpAudio.Controls.Add(this.btnPlayBroken);
			this.grpAudio.Controls.Add(this.btnPlayFailing);
			this.grpAudio.Controls.Add(this.btnPlayFixed);
			this.grpAudio.Controls.Add(this.btnPlaySuccess);
			this.grpAudio.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpAudio.Location = new System.Drawing.Point(40, 60);
			this.grpAudio.Name = "grpAudio";
			this.grpAudio.Size = new System.Drawing.Size(368, 128);
			this.grpAudio.TabIndex = 5;
			this.grpAudio.TabStop = false;
			this.grpAudio.Text = "Audio";
			// 
			// btnFindAudioSuccess
			// 
			this.btnFindAudioSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnFindAudioSuccess.Location = new System.Drawing.Point(304, 24);
			this.btnFindAudioSuccess.Name = "btnFindAudioSuccess";
			this.btnFindAudioSuccess.Size = new System.Drawing.Size(22, 20);
			this.btnFindAudioSuccess.TabIndex = 2;
			this.btnFindAudioSuccess.Text = "...";
			// 
			// txtAudioFileSuccess
			// 
			this.txtAudioFileSuccess.Location = new System.Drawing.Point(112, 24);
			this.txtAudioFileSuccess.Name = "txtAudioFileSuccess";
			this.txtAudioFileSuccess.Size = new System.Drawing.Size(184, 20);
			this.txtAudioFileSuccess.TabIndex = 1;
			this.txtAudioFileSuccess.Text = "";
			// 
			// chkAudioSuccessful
			// 
			this.chkAudioSuccessful.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioSuccessful.Location = new System.Drawing.Point(16, 24);
			this.chkAudioSuccessful.Name = "chkAudioSuccessful";
			this.chkAudioSuccessful.Size = new System.Drawing.Size(96, 16);
			this.chkAudioSuccessful.TabIndex = 0;
			this.chkAudioSuccessful.Text = "Successful";
			// 
			// chkAudioBroken
			// 
			this.chkAudioBroken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioBroken.Location = new System.Drawing.Point(16, 72);
			this.chkAudioBroken.Name = "chkAudioBroken";
			this.chkAudioBroken.Size = new System.Drawing.Size(96, 16);
			this.chkAudioBroken.TabIndex = 0;
			this.chkAudioBroken.Text = "Broken";
			// 
			// chkAudioFixed
			// 
			this.chkAudioFixed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioFixed.Location = new System.Drawing.Point(16, 48);
			this.chkAudioFixed.Name = "chkAudioFixed";
			this.chkAudioFixed.Size = new System.Drawing.Size(96, 16);
			this.chkAudioFixed.TabIndex = 0;
			this.chkAudioFixed.Text = "Fixed";
			// 
			// chkAudioStillFailing
			// 
			this.chkAudioStillFailing.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioStillFailing.Location = new System.Drawing.Point(16, 96);
			this.chkAudioStillFailing.Name = "chkAudioStillFailing";
			this.chkAudioStillFailing.Size = new System.Drawing.Size(96, 16);
			this.chkAudioStillFailing.TabIndex = 0;
			this.chkAudioStillFailing.Text = "Still failing";
			// 
			// txtAudioFileFixed
			// 
			this.txtAudioFileFixed.Location = new System.Drawing.Point(112, 48);
			this.txtAudioFileFixed.Name = "txtAudioFileFixed";
			this.txtAudioFileFixed.Size = new System.Drawing.Size(184, 20);
			this.txtAudioFileFixed.TabIndex = 1;
			this.txtAudioFileFixed.Text = "";
			// 
			// txtAudioFileBroken
			// 
			this.txtAudioFileBroken.Location = new System.Drawing.Point(112, 72);
			this.txtAudioFileBroken.Name = "txtAudioFileBroken";
			this.txtAudioFileBroken.Size = new System.Drawing.Size(184, 20);
			this.txtAudioFileBroken.TabIndex = 1;
			this.txtAudioFileBroken.Text = "";
			// 
			// txtAudioFileFailing
			// 
			this.txtAudioFileFailing.Location = new System.Drawing.Point(112, 96);
			this.txtAudioFileFailing.Name = "txtAudioFileFailing";
			this.txtAudioFileFailing.Size = new System.Drawing.Size(184, 20);
			this.txtAudioFileFailing.TabIndex = 1;
			this.txtAudioFileFailing.Text = "";
			// 
			// btnFindAudioFixed
			// 
			this.btnFindAudioFixed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnFindAudioFixed.Image = ((System.Drawing.Image)(resources.GetObject("btnFindAudioFixed.Image")));
			this.btnFindAudioFixed.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnFindAudioFixed.Location = new System.Drawing.Point(304, 48);
			this.btnFindAudioFixed.Name = "btnFindAudioFixed";
			this.btnFindAudioFixed.Size = new System.Drawing.Size(22, 20);
			this.btnFindAudioFixed.TabIndex = 2;
			// 
			// btnFindAudioBroken
			// 
			this.btnFindAudioBroken.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnFindAudioBroken.Image = ((System.Drawing.Image)(resources.GetObject("btnFindAudioBroken.Image")));
			this.btnFindAudioBroken.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnFindAudioBroken.Location = new System.Drawing.Point(304, 72);
			this.btnFindAudioBroken.Name = "btnFindAudioBroken";
			this.btnFindAudioBroken.Size = new System.Drawing.Size(22, 20);
			this.btnFindAudioBroken.TabIndex = 2;
			// 
			// btnFindAudioFailing
			// 
			this.btnFindAudioFailing.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnFindAudioFailing.Image = ((System.Drawing.Image)(resources.GetObject("btnFindAudioFailing.Image")));
			this.btnFindAudioFailing.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnFindAudioFailing.Location = new System.Drawing.Point(304, 96);
			this.btnFindAudioFailing.Name = "btnFindAudioFailing";
			this.btnFindAudioFailing.Size = new System.Drawing.Size(22, 20);
			this.btnFindAudioFailing.TabIndex = 2;
			// 
			// btnPlayBroken
			// 
			this.btnPlayBroken.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlayBroken.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayBroken.Image")));
			this.btnPlayBroken.Location = new System.Drawing.Point(328, 72);
			this.btnPlayBroken.Name = "btnPlayBroken";
			this.btnPlayBroken.Size = new System.Drawing.Size(22, 20);
			this.btnPlayBroken.TabIndex = 2;
			// 
			// btnPlayFailing
			// 
			this.btnPlayFailing.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlayFailing.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayFailing.Image")));
			this.btnPlayFailing.Location = new System.Drawing.Point(328, 96);
			this.btnPlayFailing.Name = "btnPlayFailing";
			this.btnPlayFailing.Size = new System.Drawing.Size(22, 20);
			this.btnPlayFailing.TabIndex = 2;
			// 
			// btnPlayFixed
			// 
			this.btnPlayFixed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlayFixed.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayFixed.Image")));
			this.btnPlayFixed.Location = new System.Drawing.Point(328, 48);
			this.btnPlayFixed.Name = "btnPlayFixed";
			this.btnPlayFixed.Size = new System.Drawing.Size(22, 20);
			this.btnPlayFixed.TabIndex = 2;
			// 
			// btnPlaySuccess
			// 
			this.btnPlaySuccess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlaySuccess.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaySuccess.Image")));
			this.btnPlaySuccess.Location = new System.Drawing.Point(328, 24);
			this.btnPlaySuccess.Name = "btnPlaySuccess";
			this.btnPlaySuccess.Size = new System.Drawing.Size(22, 20);
			this.btnPlaySuccess.TabIndex = 2;
			// 
			// chkShowBalloons
			// 
			this.chkShowBalloons.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkShowBalloons.Location = new System.Drawing.Point(50, 25);
			this.chkShowBalloons.Name = "chkShowBalloons";
			this.chkShowBalloons.Size = new System.Drawing.Size(248, 24);
			this.chkShowBalloons.TabIndex = 7;
			this.chkShowBalloons.Text = "Show balloon notifications";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(30, 250);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(425, 55);
			this.label1.TabIndex = 8;
			this.label1.Text = "This form is not yet functional!";
			// 
			// CCTrayMultiSettingsForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(492, 356);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.chkShowBalloons);
			this.Controls.Add(this.grpAudio);
			this.Name = "CCTrayMultiSettingsForm";
			this.Text = "CCTrayMultiSettingsForm";
			this.grpAudio.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
