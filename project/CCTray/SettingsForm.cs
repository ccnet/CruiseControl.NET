using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace tw.ccnet.remote.monitor
{
	/// <summary>
	/// Summary description for SettingsForm.
	/// </summary>
	public class SettingsForm : Drew.Controls.WobblyForm
	{
		#region Gui control declarations

		Button btnOkay;
		Label lblHeading;
		Label lblSeconds;
		Label lblPollInterval;
		GroupBox grpAudio;
		CheckBox chkAudioSuccessful;
		CheckBox chkAudioBroken;
		CheckBox chkAudioFixed;
		CheckBox chkAudioStillFailing;
		NumericUpDown numPollInterval;

		#endregion

		MonitorSettings _settings;

		#region Constructors

		public SettingsForm(MonitorSettings settings)
		{
			_settings = settings;
			InitializeComponent();
		}

		/// <summary>
		/// This constructor is for designer use only.
		/// </summary>
		public SettingsForm()
		{
			InitializeComponent();
		}


		#endregion

		#region Windows Form Designer generated code

		private System.ComponentModel.Container components = null;

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

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.btnOkay = new System.Windows.Forms.Button();
			this.lblHeading = new System.Windows.Forms.Label();
			this.numPollInterval = new System.Windows.Forms.NumericUpDown();
			this.lblSeconds = new System.Windows.Forms.Label();
			this.lblPollInterval = new System.Windows.Forms.Label();
			this.grpAudio = new System.Windows.Forms.GroupBox();
			this.chkAudioSuccessful = new System.Windows.Forms.CheckBox();
			this.chkAudioBroken = new System.Windows.Forms.CheckBox();
			this.chkAudioFixed = new System.Windows.Forms.CheckBox();
			this.chkAudioStillFailing = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numPollInterval)).BeginInit();
			this.grpAudio.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOkay
			// 
			this.btnOkay.BackColor = System.Drawing.Color.LightSteelBlue;
			this.btnOkay.Location = new System.Drawing.Point(83, 240);
			this.btnOkay.Name = "btnOkay";
			this.btnOkay.TabIndex = 0;
			this.btnOkay.Text = "&OK";
			this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
			// 
			// lblHeading
			// 
			this.lblHeading.BackColor = System.Drawing.Color.Transparent;
			this.lblHeading.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblHeading.Location = new System.Drawing.Point(16, 24);
			this.lblHeading.Name = "lblHeading";
			this.lblHeading.Size = new System.Drawing.Size(208, 24);
			this.lblHeading.TabIndex = 1;
			this.lblHeading.Text = "CruiseControl.NET Monitor Settings";
			this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// numPollInterval
			// 
			this.numPollInterval.Location = new System.Drawing.Point(100, 64);
			this.numPollInterval.Name = "numPollInterval";
			this.numPollInterval.Size = new System.Drawing.Size(56, 20);
			this.numPollInterval.TabIndex = 2;
			this.numPollInterval.Value = new System.Decimal(new int[] {
																		  10,
																		  0,
																		  0,
																		  0});
			// 
			// lblSeconds
			// 
			this.lblSeconds.BackColor = System.Drawing.Color.Transparent;
			this.lblSeconds.Location = new System.Drawing.Point(156, 64);
			this.lblSeconds.Name = "lblSeconds";
			this.lblSeconds.Size = new System.Drawing.Size(48, 20);
			this.lblSeconds.TabIndex = 3;
			this.lblSeconds.Text = "seconds";
			this.lblSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblPollInterval
			// 
			this.lblPollInterval.BackColor = System.Drawing.Color.Transparent;
			this.lblPollInterval.Location = new System.Drawing.Point(36, 64);
			this.lblPollInterval.Name = "lblPollInterval";
			this.lblPollInterval.Size = new System.Drawing.Size(64, 20);
			this.lblPollInterval.TabIndex = 3;
			this.lblPollInterval.Text = "Poll interval";
			this.lblPollInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// grpAudio
			// 
			this.grpAudio.BackColor = System.Drawing.Color.Transparent;
			this.grpAudio.Controls.Add(this.chkAudioSuccessful);
			this.grpAudio.Controls.Add(this.chkAudioBroken);
			this.grpAudio.Controls.Add(this.chkAudioFixed);
			this.grpAudio.Controls.Add(this.chkAudioStillFailing);
			this.grpAudio.Location = new System.Drawing.Point(44, 96);
			this.grpAudio.Name = "grpAudio";
			this.grpAudio.Size = new System.Drawing.Size(152, 128);
			this.grpAudio.TabIndex = 4;
			this.grpAudio.TabStop = false;
			this.grpAudio.Text = "Audio";
			// 
			// chkAudioSuccessful
			// 
			this.chkAudioSuccessful.Location = new System.Drawing.Point(24, 24);
			this.chkAudioSuccessful.Name = "chkAudioSuccessful";
			this.chkAudioSuccessful.Size = new System.Drawing.Size(104, 16);
			this.chkAudioSuccessful.TabIndex = 0;
			this.chkAudioSuccessful.Text = "Successful";
			// 
			// chkAudioBroken
			// 
			this.chkAudioBroken.Location = new System.Drawing.Point(24, 72);
			this.chkAudioBroken.Name = "chkAudioBroken";
			this.chkAudioBroken.Size = new System.Drawing.Size(104, 16);
			this.chkAudioBroken.TabIndex = 0;
			this.chkAudioBroken.Text = "Broken";
			// 
			// chkAudioFixed
			// 
			this.chkAudioFixed.Location = new System.Drawing.Point(24, 48);
			this.chkAudioFixed.Name = "chkAudioFixed";
			this.chkAudioFixed.Size = new System.Drawing.Size(104, 16);
			this.chkAudioFixed.TabIndex = 0;
			this.chkAudioFixed.Text = "Fixed";
			// 
			// chkAudioStillFailing
			// 
			this.chkAudioStillFailing.Location = new System.Drawing.Point(24, 96);
			this.chkAudioStillFailing.Name = "chkAudioStillFailing";
			this.chkAudioStillFailing.Size = new System.Drawing.Size(104, 16);
			this.chkAudioStillFailing.TabIndex = 0;
			this.chkAudioStillFailing.Text = "Still failing";
			// 
			// SettingsForm
			// 
			this.AcceptButton = this.btnOkay;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(149)), ((System.Byte)(238)));
			this.ClientSize = new System.Drawing.Size(240, 280);
			this.Controls.Add(this.grpAudio);
			this.Controls.Add(this.lblSeconds);
			this.Controls.Add(this.numPollInterval);
			this.Controls.Add(this.lblHeading);
			this.Controls.Add(this.btnOkay);
			this.Controls.Add(this.lblPollInterval);
			this.Name = "SettingsForm";
			this.Text = "SettingsForm";
			this.TransparencyKey = System.Drawing.Color.FromArgb(((System.Byte)(100)), ((System.Byte)(149)), ((System.Byte)(238)));
			this.TweenSteps = 30;
			this.WobbleFactor = 10;
			((System.ComponentModel.ISupportInitialize)(this.numPollInterval)).EndInit();
			this.grpAudio.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public void Launch()
		{
			numPollInterval.Value = _settings.PollingIntervalSeconds;
		
			chkAudioBroken.Checked = _settings.PlaySoundOnBrokenBuild;
			chkAudioFixed.Checked = _settings.PlaySoundOnFixedBuild;
			chkAudioStillFailing.Checked = _settings.PlaySoundOnAnotherFailedBuild;
			chkAudioSuccessful.Checked = _settings.PlaySoundOnAnotherSuccessfulBuild;

			this.Show();
		}

		void StoreSettings()
		{
			_settings.PollingIntervalSeconds = (int)numPollInterval.Value;

			_settings.PlaySoundOnBrokenBuild = chkAudioBroken.Checked;
			_settings.PlaySoundOnFixedBuild = chkAudioFixed.Checked;
			_settings.PlaySoundOnAnotherFailedBuild = chkAudioStillFailing.Checked;
			_settings.PlaySoundOnAnotherSuccessfulBuild = chkAudioSuccessful.Checked;

			// TODO save these somehow...
		}

		void btnOkay_Click(object sender, System.EventArgs e)
		{
			StoreSettings();
			this.Hide();
		}
	}
}
