using System;
using System.IO;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTray
{
	/// <summary>
	/// Displays user settings for the CruiseControl.NET monitor, allowing
	/// changes to be made.
	/// </summary>
	public class SettingsForm : Form
	{
		#region Gui control declarations

		Label lblSeconds;
		Label lblPollInterval;
		Label lblServerUrl;
		Label lblProjectName;
		Label lblAgent;
		GroupBox grpAudio;
		GroupBox grpAgents;
		CheckBox chkAudioSuccessful;
		CheckBox chkAudioBroken;
		CheckBox chkAudioFixed;
		CheckBox chkAudioStillFailing;
		TextBox txtAudioFileSuccess;
		TextBox txtAudioFileFixed;
		TextBox txtAudioFileBroken;
		TextBox txtAudioFileFailing;
		TextBox txtProjectName;
		TextBox txtServerUrl;
		Button btnFindAudioSuccess;
		Button btnFindAudioFixed;
		Button btnFindAudioBroken;
		Button btnFindAudioFailing;
		Button btnPlayBroken;
		Button btnPlayFailing;
		Button btnPlayFixed;
		Button btnPlaySuccess;
		Button btnOkay;
		Button btnCancel;
		CheckBox chkShowBalloons;
		CheckBox chkShowAgent;
		CheckBox chkShowExceptions;
		CheckBox chkHideAgent;
		ComboBox ddlAgent;
		NumericUpDown numPollInterval;
		OpenFileDialog dlgOpenFile;

		#endregion

		Settings _settings;
		StatusMonitor _statusMonitor;

		#region Constructors

		public SettingsForm(Settings settings, StatusMonitor statusMonitor)
		{
			_settings = settings;
			_statusMonitor = statusMonitor;

			InitializeComponent();
			ExtraInitialisation();
		}

		/// <summary>
		/// This constructor is for designer use only.
		/// </summary>
		public SettingsForm()
		{
			InitializeComponent();
			ExtraInitialisation();
		}

		void ExtraInitialisation()
		{
			dlgOpenFile.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
		}

		#endregion

		#region Windows Form Designer generated code

		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SettingsForm));
			this.btnOkay = new System.Windows.Forms.Button();
			this.numPollInterval = new System.Windows.Forms.NumericUpDown();
			this.lblSeconds = new System.Windows.Forms.Label();
			this.lblPollInterval = new System.Windows.Forms.Label();
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
			this.txtServerUrl = new System.Windows.Forms.TextBox();
			this.lblServerUrl = new System.Windows.Forms.Label();
			this.chkShowBalloons = new System.Windows.Forms.CheckBox();
			this.chkShowAgent = new System.Windows.Forms.CheckBox();
			this.grpAgents = new System.Windows.Forms.GroupBox();
			this.lblAgent = new System.Windows.Forms.Label();
			this.ddlAgent = new System.Windows.Forms.ComboBox();
			this.chkHideAgent = new System.Windows.Forms.CheckBox();
			this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.btnCancel = new System.Windows.Forms.Button();
			this.txtProjectName = new System.Windows.Forms.TextBox();
			this.lblProjectName = new System.Windows.Forms.Label();
			this.chkShowExceptions = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.numPollInterval)).BeginInit();
			this.grpAudio.SuspendLayout();
			this.grpAgents.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOkay
			// 
			this.btnOkay.BackColor = System.Drawing.SystemColors.Control;
			this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnOkay.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.btnOkay.Location = new System.Drawing.Point(116, 392);
			this.btnOkay.Name = "btnOkay";
			this.btnOkay.TabIndex = 0;
			this.btnOkay.Text = "&OK";
			this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
			// 
			// numPollInterval
			// 
			this.numPollInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.numPollInterval.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.numPollInterval.Location = new System.Drawing.Point(96, 16);
			this.numPollInterval.Name = "numPollInterval";
			this.numPollInterval.Size = new System.Drawing.Size(56, 21);
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
			this.lblSeconds.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.lblSeconds.Location = new System.Drawing.Point(160, 16);
			this.lblSeconds.Name = "lblSeconds";
			this.lblSeconds.Size = new System.Drawing.Size(64, 20);
			this.lblSeconds.TabIndex = 3;
			this.lblSeconds.Text = "seconds";
			this.lblSeconds.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblPollInterval
			// 
			this.lblPollInterval.BackColor = System.Drawing.Color.Transparent;
			this.lblPollInterval.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.lblPollInterval.Location = new System.Drawing.Point(8, 16);
			this.lblPollInterval.Name = "lblPollInterval";
			this.lblPollInterval.Size = new System.Drawing.Size(64, 20);
			this.lblPollInterval.TabIndex = 3;
			this.lblPollInterval.Text = "Poll every";
			this.lblPollInterval.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// grpAudio
			// 
			this.grpAudio.BackColor = System.Drawing.Color.Transparent;
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
			this.grpAudio.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.grpAudio.Location = new System.Drawing.Point(8, 256);
			this.grpAudio.Name = "grpAudio";
			this.grpAudio.Size = new System.Drawing.Size(368, 128);
			this.grpAudio.TabIndex = 4;
			this.grpAudio.TabStop = false;
			this.grpAudio.Text = "Audio";
			// 
			// btnFindAudioSuccess
			// 
			this.btnFindAudioSuccess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnFindAudioSuccess.Image = ((System.Drawing.Image)(resources.GetObject("btnFindAudioSuccess.Image")));
			this.btnFindAudioSuccess.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.btnFindAudioSuccess.Location = new System.Drawing.Point(304, 24);
			this.btnFindAudioSuccess.Name = "btnFindAudioSuccess";
			this.btnFindAudioSuccess.Size = new System.Drawing.Size(22, 20);
			this.btnFindAudioSuccess.TabIndex = 2;
			this.btnFindAudioSuccess.Click += new System.EventHandler(this.btnFindAudioSuccess_Click);
			// 
			// txtAudioFileSuccess
			// 
			this.txtAudioFileSuccess.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtAudioFileSuccess.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.txtAudioFileSuccess.Location = new System.Drawing.Point(112, 24);
			this.txtAudioFileSuccess.Name = "txtAudioFileSuccess";
			this.txtAudioFileSuccess.Size = new System.Drawing.Size(184, 21);
			this.txtAudioFileSuccess.TabIndex = 1;
			this.txtAudioFileSuccess.Text = "";
			// 
			// chkAudioSuccessful
			// 
			this.chkAudioSuccessful.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkAudioSuccessful.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkAudioSuccessful.Location = new System.Drawing.Point(16, 24);
			this.chkAudioSuccessful.Name = "chkAudioSuccessful";
			this.chkAudioSuccessful.Size = new System.Drawing.Size(96, 16);
			this.chkAudioSuccessful.TabIndex = 0;
			this.chkAudioSuccessful.Text = "Successful";
			// 
			// chkAudioBroken
			// 
			this.chkAudioBroken.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkAudioBroken.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkAudioBroken.Location = new System.Drawing.Point(16, 72);
			this.chkAudioBroken.Name = "chkAudioBroken";
			this.chkAudioBroken.Size = new System.Drawing.Size(96, 16);
			this.chkAudioBroken.TabIndex = 0;
			this.chkAudioBroken.Text = "Broken";
			// 
			// chkAudioFixed
			// 
			this.chkAudioFixed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkAudioFixed.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkAudioFixed.Location = new System.Drawing.Point(16, 48);
			this.chkAudioFixed.Name = "chkAudioFixed";
			this.chkAudioFixed.Size = new System.Drawing.Size(96, 16);
			this.chkAudioFixed.TabIndex = 0;
			this.chkAudioFixed.Text = "Fixed";
			// 
			// chkAudioStillFailing
			// 
			this.chkAudioStillFailing.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkAudioStillFailing.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkAudioStillFailing.Location = new System.Drawing.Point(16, 96);
			this.chkAudioStillFailing.Name = "chkAudioStillFailing";
			this.chkAudioStillFailing.Size = new System.Drawing.Size(96, 16);
			this.chkAudioStillFailing.TabIndex = 0;
			this.chkAudioStillFailing.Text = "Still failing";
			// 
			// txtAudioFileFixed
			// 
			this.txtAudioFileFixed.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtAudioFileFixed.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.txtAudioFileFixed.Location = new System.Drawing.Point(112, 48);
			this.txtAudioFileFixed.Name = "txtAudioFileFixed";
			this.txtAudioFileFixed.Size = new System.Drawing.Size(184, 21);
			this.txtAudioFileFixed.TabIndex = 1;
			this.txtAudioFileFixed.Text = "";
			// 
			// txtAudioFileBroken
			// 
			this.txtAudioFileBroken.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtAudioFileBroken.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.txtAudioFileBroken.Location = new System.Drawing.Point(112, 72);
			this.txtAudioFileBroken.Name = "txtAudioFileBroken";
			this.txtAudioFileBroken.Size = new System.Drawing.Size(184, 21);
			this.txtAudioFileBroken.TabIndex = 1;
			this.txtAudioFileBroken.Text = "";
			// 
			// txtAudioFileFailing
			// 
			this.txtAudioFileFailing.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtAudioFileFailing.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.txtAudioFileFailing.Location = new System.Drawing.Point(112, 96);
			this.txtAudioFileFailing.Name = "txtAudioFileFailing";
			this.txtAudioFileFailing.Size = new System.Drawing.Size(184, 21);
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
			this.btnFindAudioFixed.Click += new System.EventHandler(this.btnFindAudioFixed_Click);
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
			this.btnFindAudioBroken.Click += new System.EventHandler(this.btnFindAudioBroken_Click);
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
			this.btnFindAudioFailing.Click += new System.EventHandler(this.btnFindAudioFailing_Click);
			// 
			// btnPlayBroken
			// 
			this.btnPlayBroken.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlayBroken.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayBroken.Image")));
			this.btnPlayBroken.Location = new System.Drawing.Point(328, 72);
			this.btnPlayBroken.Name = "btnPlayBroken";
			this.btnPlayBroken.Size = new System.Drawing.Size(22, 20);
			this.btnPlayBroken.TabIndex = 2;
			this.btnPlayBroken.Click += new System.EventHandler(this.btnPlayBroken_Click);
			// 
			// btnPlayFailing
			// 
			this.btnPlayFailing.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlayFailing.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayFailing.Image")));
			this.btnPlayFailing.Location = new System.Drawing.Point(328, 96);
			this.btnPlayFailing.Name = "btnPlayFailing";
			this.btnPlayFailing.Size = new System.Drawing.Size(22, 20);
			this.btnPlayFailing.TabIndex = 2;
			this.btnPlayFailing.Click += new System.EventHandler(this.btnPlayFailing_Click);
			// 
			// btnPlayFixed
			// 
			this.btnPlayFixed.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlayFixed.Image = ((System.Drawing.Image)(resources.GetObject("btnPlayFixed.Image")));
			this.btnPlayFixed.Location = new System.Drawing.Point(328, 48);
			this.btnPlayFixed.Name = "btnPlayFixed";
			this.btnPlayFixed.Size = new System.Drawing.Size(22, 20);
			this.btnPlayFixed.TabIndex = 2;
			this.btnPlayFixed.Click += new System.EventHandler(this.btnPlayFixed_Click);
			// 
			// btnPlaySuccess
			// 
			this.btnPlaySuccess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnPlaySuccess.Image = ((System.Drawing.Image)(resources.GetObject("btnPlaySuccess.Image")));
			this.btnPlaySuccess.Location = new System.Drawing.Point(328, 24);
			this.btnPlaySuccess.Name = "btnPlaySuccess";
			this.btnPlaySuccess.Size = new System.Drawing.Size(22, 20);
			this.btnPlaySuccess.TabIndex = 2;
			this.btnPlaySuccess.Click += new System.EventHandler(this.btnPlaySuccess_Click);
			// 
			// txtServerUrl
			// 
			this.txtServerUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtServerUrl.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.txtServerUrl.Location = new System.Drawing.Point(96, 40);
			this.txtServerUrl.Name = "txtServerUrl";
			this.txtServerUrl.Size = new System.Drawing.Size(280, 21);
			this.txtServerUrl.TabIndex = 5;
			this.txtServerUrl.Text = "";
			// 
			// lblServerUrl
			// 
			this.lblServerUrl.BackColor = System.Drawing.Color.Transparent;
			this.lblServerUrl.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.lblServerUrl.Location = new System.Drawing.Point(8, 40);
			this.lblServerUrl.Name = "lblServerUrl";
			this.lblServerUrl.Size = new System.Drawing.Size(56, 20);
			this.lblServerUrl.TabIndex = 3;
			this.lblServerUrl.Text = "Server";
			this.lblServerUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkShowBalloons
			// 
			this.chkShowBalloons.BackColor = System.Drawing.Color.Transparent;
			this.chkShowBalloons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkShowBalloons.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkShowBalloons.Location = new System.Drawing.Point(96, 88);
			this.chkShowBalloons.Name = "chkShowBalloons";
			this.chkShowBalloons.Size = new System.Drawing.Size(248, 24);
			this.chkShowBalloons.TabIndex = 6;
			this.chkShowBalloons.Text = "Show balloon notifications";
			// 
			// chkShowAgent
			// 
			this.chkShowAgent.BackColor = System.Drawing.Color.Transparent;
			this.chkShowAgent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkShowAgent.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkShowAgent.Location = new System.Drawing.Point(16, 16);
			this.chkShowAgent.Name = "chkShowAgent";
			this.chkShowAgent.Size = new System.Drawing.Size(248, 24);
			this.chkShowAgent.TabIndex = 7;
			this.chkShowAgent.Text = "Show agent";
			// 
			// grpAgents
			// 
			this.grpAgents.BackColor = System.Drawing.Color.Transparent;
			this.grpAgents.Controls.Add(this.lblAgent);
			this.grpAgents.Controls.Add(this.ddlAgent);
			this.grpAgents.Controls.Add(this.chkHideAgent);
			this.grpAgents.Controls.Add(this.chkShowAgent);
			this.grpAgents.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.grpAgents.Location = new System.Drawing.Point(8, 144);
			this.grpAgents.Name = "grpAgents";
			this.grpAgents.Size = new System.Drawing.Size(368, 104);
			this.grpAgents.TabIndex = 8;
			this.grpAgents.TabStop = false;
			this.grpAgents.Text = "Agents";
			// 
			// lblAgent
			// 
			this.lblAgent.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.lblAgent.Location = new System.Drawing.Point(32, 72);
			this.lblAgent.Name = "lblAgent";
			this.lblAgent.Size = new System.Drawing.Size(48, 23);
			this.lblAgent.TabIndex = 10;
			this.lblAgent.Text = "Agent";
			this.lblAgent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ddlAgent
			// 
			this.ddlAgent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlAgent.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.ddlAgent.Location = new System.Drawing.Point(112, 72);
			this.ddlAgent.Name = "ddlAgent";
			this.ddlAgent.Size = new System.Drawing.Size(184, 21);
			this.ddlAgent.TabIndex = 9;
			// 
			// chkHideAgent
			// 
			this.chkHideAgent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkHideAgent.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkHideAgent.Location = new System.Drawing.Point(16, 40);
			this.chkHideAgent.Name = "chkHideAgent";
			this.chkHideAgent.Size = new System.Drawing.Size(248, 24);
			this.chkHideAgent.TabIndex = 8;
			this.chkHideAgent.Text = "Hide after announcement";
			// 
			// dlgOpenFile
			// 
			this.dlgOpenFile.DefaultExt = "*.wav";
			this.dlgOpenFile.Title = "Select wave file";
			// 
			// btnCancel
			// 
			this.btnCancel.BackColor = System.Drawing.SystemColors.Control;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.btnCancel.Location = new System.Drawing.Point(196, 392);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 0;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// txtProjectName
			// 
			this.txtProjectName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.txtProjectName.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.txtProjectName.Location = new System.Drawing.Point(96, 64);
			this.txtProjectName.Name = "txtProjectName";
			this.txtProjectName.Size = new System.Drawing.Size(280, 21);
			this.txtProjectName.TabIndex = 9;
			this.txtProjectName.Text = "";
			// 
			// lblProjectName
			// 
			this.lblProjectName.BackColor = System.Drawing.Color.Transparent;
			this.lblProjectName.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.lblProjectName.Location = new System.Drawing.Point(8, 64);
			this.lblProjectName.Name = "lblProjectName";
			this.lblProjectName.Size = new System.Drawing.Size(80, 20);
			this.lblProjectName.TabIndex = 10;
			this.lblProjectName.Text = "Project name";
			this.lblProjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkShowExceptions
			// 
			this.chkShowExceptions.BackColor = System.Drawing.Color.Transparent;
			this.chkShowExceptions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.chkShowExceptions.Font = new System.Drawing.Font("Verdana", 8.25F);
			this.chkShowExceptions.Location = new System.Drawing.Point(96, 112);
			this.chkShowExceptions.Name = "chkShowExceptions";
			this.chkShowExceptions.Size = new System.Drawing.Size(248, 24);
			this.chkShowExceptions.TabIndex = 6;
			this.chkShowExceptions.Text = "Show exceptions";
			// 
			// SettingsForm
			// 
			this.AcceptButton = this.btnOkay;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.BackColor = System.Drawing.Color.FromArgb(((System.Byte)(212)), ((System.Byte)(208)), ((System.Byte)(201)));
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(386, 426);
			this.ControlBox = false;
			this.Controls.Add(this.lblProjectName);
			this.Controls.Add(this.txtProjectName);
			this.Controls.Add(this.txtServerUrl);
			this.Controls.Add(this.grpAgents);
			this.Controls.Add(this.chkShowBalloons);
			this.Controls.Add(this.grpAudio);
			this.Controls.Add(this.lblSeconds);
			this.Controls.Add(this.numPollInterval);
			this.Controls.Add(this.btnOkay);
			this.Controls.Add(this.lblPollInterval);
			this.Controls.Add(this.lblServerUrl);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.chkShowExceptions);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "CruiseControl.NET Monitor Settings";
			((System.ComponentModel.ISupportInitialize)(this.numPollInterval)).EndInit();
			this.grpAudio.ResumeLayout(false);
			this.grpAgents.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public void Launch()
		{
			PopulateControlsFromSettings();
			this.Show();
		}


		#region Moving settings between Settings object and Gui controls

		/// <summary>
		/// Copies values from the 
		/// </summary>
		void PopulateControlsFromSettings()
		{
			numPollInterval.Value = _settings.PollingIntervalSeconds;
			txtServerUrl.Text = _settings.RemoteServerUrl;
			txtProjectName.Text = _settings.ProjectName;

			chkShowBalloons.Checked = _settings.NotificationBalloon.ShowBalloon;
			chkShowExceptions.Checked = _settings.ShowExceptions;
			chkShowAgent.Checked = _settings.Agents.ShowAgent;
			chkHideAgent.Checked = _settings.Agents.HideAfterMessage;

			ddlAgent.Items.Clear();
			foreach (AgentDetails agent in _settings.Agents.AvailableAgents)
			{
				ddlAgent.Items.Add(agent.Name);

				// select this one if it's the current selection
				if (agent.Name==_settings.Agents.CurrentAgentName)
					ddlAgent.SelectedItem = agent.Name;
			}
		
			chkAudioBroken.Checked = _settings.Sounds.BrokenBuildSound.Play;
			chkAudioFixed.Checked = _settings.Sounds.FixedBuildSound.Play;
			chkAudioStillFailing.Checked = _settings.Sounds.AnotherFailedBuildSound.Play;
			chkAudioSuccessful.Checked = _settings.Sounds.AnotherSuccessfulBuildSound.Play;

			txtAudioFileBroken.Text = _settings.Sounds.BrokenBuildSound.FileName;
			txtAudioFileFixed.Text = _settings.Sounds.FixedBuildSound.FileName;
			txtAudioFileFailing.Text = _settings.Sounds.AnotherFailedBuildSound.FileName;
			txtAudioFileSuccess.Text = _settings.Sounds.AnotherSuccessfulBuildSound.FileName;
		}

		void PopulateSettingsFromControls()
		{
			_settings.PollingIntervalSeconds = (int)numPollInterval.Value;
			_settings.RemoteServerUrl = txtServerUrl.Text;
			_settings.ProjectName = txtProjectName.Text;

			_settings.ShowExceptions = chkShowExceptions.Checked;

			_settings.NotificationBalloon.ShowBalloon = chkShowBalloons.Checked;
			_settings.Agents.ShowAgent = chkShowAgent.Checked;
			_settings.Agents.HideAfterMessage = chkHideAgent.Checked;

			if (ddlAgent.SelectedItem!=null)
			{
				_settings.Agents.CurrentAgentName = (string)ddlAgent.SelectedItem;
			}

			_settings.Sounds.BrokenBuildSound.Play = chkAudioBroken.Checked;
			_settings.Sounds.FixedBuildSound.Play = chkAudioFixed.Checked;
			_settings.Sounds.AnotherFailedBuildSound.Play = chkAudioStillFailing.Checked;
			_settings.Sounds.AnotherSuccessfulBuildSound.Play = chkAudioSuccessful.Checked;

			_settings.Sounds.BrokenBuildSound.FileName = txtAudioFileBroken.Text;
			_settings.Sounds.FixedBuildSound.FileName = txtAudioFileFixed.Text;
			_settings.Sounds.AnotherFailedBuildSound.FileName = txtAudioFileFailing.Text;
			_settings.Sounds.AnotherSuccessfulBuildSound.FileName = txtAudioFileSuccess.Text;
		}

		#endregion

		#region Ok & Cancel

		void btnOkay_Click(object sender, System.EventArgs e)
		{
			PopulateSettingsFromControls();
			
			// save settings
			SettingsManager.WriteSettings(_settings);

			this.Hide();

			// force a poll once the form is hidden
			_statusMonitor.Poll();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Hide();
			PopulateControlsFromSettings();
		}

		#endregion

		#region Finding audio files

		private void btnFindAudioSuccess_Click(object sender, System.EventArgs e)
		{
			FindAudioFile(txtAudioFileSuccess);
		}

		private void btnFindAudioFixed_Click(object sender, System.EventArgs e)
		{
			FindAudioFile(txtAudioFileFixed);
		}

		private void btnFindAudioBroken_Click(object sender, System.EventArgs e)
		{
			FindAudioFile(txtAudioFileBroken);
		}

		private void btnFindAudioFailing_Click(object sender, System.EventArgs e)
		{
			FindAudioFile(txtAudioFileFailing);
		}

		void FindAudioFile(TextBox textBox)
		{
			DialogResult result = dlgOpenFile.ShowDialog();

			if (result!=DialogResult.OK)
				return;
			
			string fileName = dlgOpenFile.FileName;

			// make relative path
			if (fileName.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
				fileName = fileName.Substring(AppDomain.CurrentDomain.BaseDirectory.Length);

			textBox.Text = fileName;
		}

		#endregion

		#region Previewing audio

		private void btnPlaySuccess_Click(object sender, System.EventArgs e)
		{
			PlayAudioFile(txtAudioFileSuccess.Text);
		}

		private void btnPlayFixed_Click(object sender, System.EventArgs e)
		{
			PlayAudioFile(txtAudioFileFixed.Text);
		}

		private void btnPlayBroken_Click(object sender, System.EventArgs e)
		{
			PlayAudioFile(txtAudioFileBroken.Text);
		}

		private void btnPlayFailing_Click(object sender, System.EventArgs e)
		{
			PlayAudioFile(txtAudioFileFailing.Text);
		}

		void PlayAudioFile(string fileName)
		{
			if (fileName==null || fileName.Trim().Length==0)
				return;

			if (!File.Exists(fileName))
			{
				MessageBox.Show("The specified audio file was not found.", "File not found", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			Audio.PlaySound(fileName, false, true);
		}

		#endregion
	}
}
