using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class CCTrayMultiSettingsForm : Form
	{
		private readonly ICCTrayMultiConfiguration configuration;
		private CheckBox chkShowBalloons;
		private GroupBox grpServers;
		private Button btnRemove;
		private Button btnMoveUp;
		private Button btnAdd;
		private Label label1;
		private Button btnOK;
		private Button btnCancel;
		private ListView lvProjects;
		private Button btnEdit;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;

		private ProjectConfigurationListViewItemAdaptor selected = null;
		private int selectedIndex = -1;
		private Button btnMoveDown;
		private GroupBox grpAudio;
		private TextBox txtAudioFileSuccess;
		private CheckBox chkAudioSuccessful;
		private CheckBox chkAudioBroken;
		private CheckBox chkAudioFixed;
		private CheckBox chkAudioStillFailing;
		private TextBox txtAudioFileFixed;
		private TextBox txtAudioFileBroken;
		private TextBox txtAudioFileFailing;
		private Button btnSuccessfulBrowse;
		private Button btnSuccessfulPlay;
		private Button btnFixedPlay;
		private Button btnFixedBrowse;
		private Button btnBrokenPlay;
		private Button btnBrokenBrowse;
		private Button btnStillFailingPlay;
		private Button btnStillFailingBrowse;
		private OpenFileDialog dlgOpenFile;

		private Container components = null;
		private SelectAudioFileController brokenAudio;
		private SelectAudioFileController fixedAudio;
		private SelectAudioFileController stillFailingAudio;
		private SelectAudioFileController successfulAudio;

		public CCTrayMultiSettingsForm(ICCTrayMultiConfiguration configuration)
		{
			this.configuration = configuration.Clone();

			InitializeComponent();

			chkShowBalloons.DataBindings.Add("Checked", configuration, "ShouldShowBalloonOnBuildTransition");

			BindListView();

			BindAudioControls();
		}

		private void BindListView()
		{
			lvProjects.Items.Clear();

			foreach (Project project in configuration.Projects)
			{
				lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(project).Item);
			}

			UpdateButtons();
		}

		private void BindAudioControls()
		{
			AudioFiles audioConfig = configuration.Audio;

			brokenAudio = new SelectAudioFileController(
				chkAudioBroken, txtAudioFileBroken, btnBrokenBrowse, btnBrokenPlay, dlgOpenFile, audioConfig.BrokenBuildSound);
			fixedAudio = new SelectAudioFileController(
				chkAudioFixed, txtAudioFileFixed, btnFixedBrowse, btnFixedPlay, dlgOpenFile, audioConfig.FixedBuildSound);
			stillFailingAudio = new SelectAudioFileController(
				chkAudioStillFailing, txtAudioFileFailing, btnStillFailingBrowse, btnStillFailingPlay, dlgOpenFile, audioConfig.StillFailingBuildSound);
			successfulAudio = new SelectAudioFileController(
				chkAudioSuccessful, txtAudioFileSuccess, btnSuccessfulBrowse, btnSuccessfulPlay, dlgOpenFile, audioConfig.StillSuccessfulBuildSound);
		}

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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.chkShowBalloons = new System.Windows.Forms.CheckBox();
			this.grpServers = new System.Windows.Forms.GroupBox();
			this.btnEdit = new System.Windows.Forms.Button();
			this.lvProjects = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.btnAdd = new System.Windows.Forms.Button();
			this.btnMoveDown = new System.Windows.Forms.Button();
			this.btnMoveUp = new System.Windows.Forms.Button();
			this.btnRemove = new System.Windows.Forms.Button();
			this.btnOK = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.grpAudio = new System.Windows.Forms.GroupBox();
			this.btnStillFailingPlay = new System.Windows.Forms.Button();
			this.btnStillFailingBrowse = new System.Windows.Forms.Button();
			this.btnBrokenPlay = new System.Windows.Forms.Button();
			this.btnBrokenBrowse = new System.Windows.Forms.Button();
			this.btnFixedPlay = new System.Windows.Forms.Button();
			this.btnFixedBrowse = new System.Windows.Forms.Button();
			this.btnSuccessfulPlay = new System.Windows.Forms.Button();
			this.btnSuccessfulBrowse = new System.Windows.Forms.Button();
			this.txtAudioFileSuccess = new System.Windows.Forms.TextBox();
			this.chkAudioSuccessful = new System.Windows.Forms.CheckBox();
			this.chkAudioBroken = new System.Windows.Forms.CheckBox();
			this.chkAudioFixed = new System.Windows.Forms.CheckBox();
			this.chkAudioStillFailing = new System.Windows.Forms.CheckBox();
			this.txtAudioFileFixed = new System.Windows.Forms.TextBox();
			this.txtAudioFileBroken = new System.Windows.Forms.TextBox();
			this.txtAudioFileFailing = new System.Windows.Forms.TextBox();
			this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
			this.grpServers.SuspendLayout();
			this.grpAudio.SuspendLayout();
			this.SuspendLayout();
			// 
			// chkShowBalloons
			// 
			this.chkShowBalloons.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkShowBalloons.Location = new System.Drawing.Point(15, 10);
			this.chkShowBalloons.Name = "chkShowBalloons";
			this.chkShowBalloons.Size = new System.Drawing.Size(248, 24);
			this.chkShowBalloons.TabIndex = 0;
			this.chkShowBalloons.Text = "Show balloon notifications";
			// 
			// grpServers
			// 
			this.grpServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpServers.Controls.Add(this.btnEdit);
			this.grpServers.Controls.Add(this.lvProjects);
			this.grpServers.Controls.Add(this.label1);
			this.grpServers.Controls.Add(this.btnAdd);
			this.grpServers.Controls.Add(this.btnMoveDown);
			this.grpServers.Controls.Add(this.btnMoveUp);
			this.grpServers.Controls.Add(this.btnRemove);
			this.grpServers.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpServers.Location = new System.Drawing.Point(15, 185);
			this.grpServers.Name = "grpServers";
			this.grpServers.Size = new System.Drawing.Size(580, 230);
			this.grpServers.TabIndex = 2;
			this.grpServers.TabStop = false;
			this.grpServers.Text = "Build Servers";
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnEdit.Location = new System.Drawing.Point(495, 95);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.TabIndex = 3;
			this.btnEdit.Text = "&Edit...";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			// 
			// lvProjects
			// 
			this.lvProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						 this.columnHeader1,
																						 this.columnHeader2});
			this.lvProjects.FullRowSelect = true;
			this.lvProjects.HideSelection = false;
			this.lvProjects.Location = new System.Drawing.Point(10, 55);
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new System.Drawing.Size(475, 165);
			this.lvProjects.TabIndex = 1;
			this.lvProjects.View = System.Windows.Forms.View.Details;
			this.lvProjects.DoubleClick += new System.EventHandler(this.lvProjects_DoubleClick);
			this.lvProjects.SelectedIndexChanged += new System.EventHandler(this.lvProjects_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Build Server";
			this.columnHeader1.Width = 339;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Project";
			this.columnHeader2.Width = 124;
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(10, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(560, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Use this section to define the CruiseControl.NET projects to monitor. ";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnAdd.Location = new System.Drawing.Point(495, 60);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "&Add...";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveDown.Location = new System.Drawing.Point(495, 200);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.TabIndex = 6;
			this.btnMoveDown.Text = "Move &Down";
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnMoveUp.Location = new System.Drawing.Point(495, 165);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.TabIndex = 5;
			this.btnMoveUp.Text = "Move &Up";
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnRemove.Location = new System.Drawing.Point(495, 130);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 4;
			this.btnRemove.Text = "&Remove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnOK.Location = new System.Drawing.Point(224, 433);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 3;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnCancel.Location = new System.Drawing.Point(314, 433);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			// 
			// grpAudio
			// 
			this.grpAudio.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.grpAudio.Controls.Add(this.btnStillFailingPlay);
			this.grpAudio.Controls.Add(this.btnStillFailingBrowse);
			this.grpAudio.Controls.Add(this.btnBrokenPlay);
			this.grpAudio.Controls.Add(this.btnBrokenBrowse);
			this.grpAudio.Controls.Add(this.btnFixedPlay);
			this.grpAudio.Controls.Add(this.btnFixedBrowse);
			this.grpAudio.Controls.Add(this.btnSuccessfulPlay);
			this.grpAudio.Controls.Add(this.btnSuccessfulBrowse);
			this.grpAudio.Controls.Add(this.txtAudioFileSuccess);
			this.grpAudio.Controls.Add(this.chkAudioSuccessful);
			this.grpAudio.Controls.Add(this.chkAudioBroken);
			this.grpAudio.Controls.Add(this.chkAudioFixed);
			this.grpAudio.Controls.Add(this.chkAudioStillFailing);
			this.grpAudio.Controls.Add(this.txtAudioFileFixed);
			this.grpAudio.Controls.Add(this.txtAudioFileBroken);
			this.grpAudio.Controls.Add(this.txtAudioFileFailing);
			this.grpAudio.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.grpAudio.Location = new System.Drawing.Point(15, 45);
			this.grpAudio.Name = "grpAudio";
			this.grpAudio.Size = new System.Drawing.Size(580, 128);
			this.grpAudio.TabIndex = 1;
			this.grpAudio.TabStop = false;
			this.grpAudio.Text = "Audio";
			// 
			// btnStillFailingPlay
			// 
			this.btnStillFailingPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStillFailingPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnStillFailingPlay.Location = new System.Drawing.Point(495, 95);
			this.btnStillFailingPlay.Name = "btnStillFailingPlay";
			this.btnStillFailingPlay.TabIndex = 15;
			this.btnStillFailingPlay.Text = "Play!";
			// 
			// btnStillFailingBrowse
			// 
			this.btnStillFailingBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnStillFailingBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnStillFailingBrowse.Location = new System.Drawing.Point(410, 95);
			this.btnStillFailingBrowse.Name = "btnStillFailingBrowse";
			this.btnStillFailingBrowse.TabIndex = 14;
			this.btnStillFailingBrowse.Text = "Browse...";
			// 
			// btnBrokenPlay
			// 
			this.btnBrokenPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrokenPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrokenPlay.Location = new System.Drawing.Point(495, 71);
			this.btnBrokenPlay.Name = "btnBrokenPlay";
			this.btnBrokenPlay.TabIndex = 11;
			this.btnBrokenPlay.Text = "Play!";
			// 
			// btnBrokenBrowse
			// 
			this.btnBrokenBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBrokenBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnBrokenBrowse.Location = new System.Drawing.Point(410, 71);
			this.btnBrokenBrowse.Name = "btnBrokenBrowse";
			this.btnBrokenBrowse.TabIndex = 10;
			this.btnBrokenBrowse.Text = "Browse...";
			// 
			// btnFixedPlay
			// 
			this.btnFixedPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFixedPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnFixedPlay.Location = new System.Drawing.Point(495, 47);
			this.btnFixedPlay.Name = "btnFixedPlay";
			this.btnFixedPlay.TabIndex = 7;
			this.btnFixedPlay.Text = "Play!";
			// 
			// btnFixedBrowse
			// 
			this.btnFixedBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnFixedBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnFixedBrowse.Location = new System.Drawing.Point(410, 47);
			this.btnFixedBrowse.Name = "btnFixedBrowse";
			this.btnFixedBrowse.TabIndex = 6;
			this.btnFixedBrowse.Text = "Browse...";
			// 
			// btnSuccessfulPlay
			// 
			this.btnSuccessfulPlay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSuccessfulPlay.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSuccessfulPlay.Location = new System.Drawing.Point(495, 23);
			this.btnSuccessfulPlay.Name = "btnSuccessfulPlay";
			this.btnSuccessfulPlay.TabIndex = 3;
			this.btnSuccessfulPlay.Text = "Play!";
			// 
			// btnSuccessfulBrowse
			// 
			this.btnSuccessfulBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSuccessfulBrowse.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSuccessfulBrowse.Location = new System.Drawing.Point(410, 23);
			this.btnSuccessfulBrowse.Name = "btnSuccessfulBrowse";
			this.btnSuccessfulBrowse.TabIndex = 2;
			this.btnSuccessfulBrowse.Text = "Browse...";
			// 
			// txtAudioFileSuccess
			// 
			this.txtAudioFileSuccess.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileSuccess.Location = new System.Drawing.Point(112, 24);
			this.txtAudioFileSuccess.Name = "txtAudioFileSuccess";
			this.txtAudioFileSuccess.Size = new System.Drawing.Size(288, 20);
			this.txtAudioFileSuccess.TabIndex = 1;
			this.txtAudioFileSuccess.Text = "";
			// 
			// chkAudioSuccessful
			// 
			this.chkAudioSuccessful.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioSuccessful.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioSuccessful.Location = new System.Drawing.Point(16, 24);
			this.chkAudioSuccessful.Name = "chkAudioSuccessful";
			this.chkAudioSuccessful.Size = new System.Drawing.Size(96, 16);
			this.chkAudioSuccessful.TabIndex = 0;
			this.chkAudioSuccessful.Text = "Successful";
			// 
			// chkAudioBroken
			// 
			this.chkAudioBroken.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioBroken.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioBroken.Location = new System.Drawing.Point(16, 72);
			this.chkAudioBroken.Name = "chkAudioBroken";
			this.chkAudioBroken.Size = new System.Drawing.Size(96, 16);
			this.chkAudioBroken.TabIndex = 8;
			this.chkAudioBroken.Text = "Broken";
			// 
			// chkAudioFixed
			// 
			this.chkAudioFixed.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioFixed.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioFixed.Location = new System.Drawing.Point(16, 48);
			this.chkAudioFixed.Name = "chkAudioFixed";
			this.chkAudioFixed.Size = new System.Drawing.Size(96, 16);
			this.chkAudioFixed.TabIndex = 4;
			this.chkAudioFixed.Text = "Fixed";
			// 
			// chkAudioStillFailing
			// 
			this.chkAudioStillFailing.BackColor = System.Drawing.SystemColors.Control;
			this.chkAudioStillFailing.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.chkAudioStillFailing.Location = new System.Drawing.Point(16, 96);
			this.chkAudioStillFailing.Name = "chkAudioStillFailing";
			this.chkAudioStillFailing.Size = new System.Drawing.Size(96, 16);
			this.chkAudioStillFailing.TabIndex = 12;
			this.chkAudioStillFailing.Text = "Still failing";
			// 
			// txtAudioFileFixed
			// 
			this.txtAudioFileFixed.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileFixed.Location = new System.Drawing.Point(112, 48);
			this.txtAudioFileFixed.Name = "txtAudioFileFixed";
			this.txtAudioFileFixed.Size = new System.Drawing.Size(288, 20);
			this.txtAudioFileFixed.TabIndex = 5;
			this.txtAudioFileFixed.Text = "";
			// 
			// txtAudioFileBroken
			// 
			this.txtAudioFileBroken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileBroken.Location = new System.Drawing.Point(112, 72);
			this.txtAudioFileBroken.Name = "txtAudioFileBroken";
			this.txtAudioFileBroken.Size = new System.Drawing.Size(288, 20);
			this.txtAudioFileBroken.TabIndex = 9;
			this.txtAudioFileBroken.Text = "";
			// 
			// txtAudioFileFailing
			// 
			this.txtAudioFileFailing.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtAudioFileFailing.Location = new System.Drawing.Point(112, 96);
			this.txtAudioFileFailing.Name = "txtAudioFileFailing";
			this.txtAudioFileFailing.Size = new System.Drawing.Size(288, 20);
			this.txtAudioFileFailing.TabIndex = 13;
			this.txtAudioFileFailing.Text = "";
			// 
			// dlgOpenFile
			// 
			this.dlgOpenFile.DefaultExt = "wav";
			this.dlgOpenFile.Filter = "Wave Files|*.wav|All Files|*.*";
			this.dlgOpenFile.Title = "Select wave file";
			// 
			// CCTrayMultiSettingsForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(612, 471);
			this.Controls.Add(this.grpAudio);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grpServers);
			this.Controls.Add(this.chkShowBalloons);
			this.Name = "CCTrayMultiSettingsForm";
			this.Text = "CruiseControl.NET Tray Settings";
			this.grpServers.ResumeLayout(false);
			this.grpAudio.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void lvProjects_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (lvProjects.SelectedItems.Count == 0)
			{
				selected = null;
				selectedIndex = -1;
			}
			else
			{
				selected = (ProjectConfigurationListViewItemAdaptor) lvProjects.SelectedItems[0].Tag;
				selectedIndex = lvProjects.SelectedIndices[0];
			}

			UpdateButtons();
		}

		private void UpdateButtons()
		{
			btnRemove.Enabled = btnEdit.Enabled = selected != null;

			if (selected != null)
			{
				btnMoveDown.Enabled = selectedIndex < (lvProjects.Items.Count - 1);
				btnMoveUp.Enabled = selectedIndex != 0;
			}
			else
			{
				btnMoveDown.Enabled = btnMoveUp.Enabled = false;
			}

		}

		private void btnRemove_Click(object sender, EventArgs e)
		{
			lvProjects.Items.RemoveAt(selectedIndex);
		}

		private void btnMoveUp_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(-1);
		}

		private void btnMoveDown_Click(object sender, EventArgs e)
		{
			MoveSelectedItem(+1);
		}

		private void MoveSelectedItem(int delta)
		{
			ProjectConfigurationListViewItemAdaptor currentlySelected = selected;
			int reinsertIndex = selectedIndex + delta;

			lvProjects.Items.RemoveAt(selectedIndex);
			lvProjects.Items.Insert(reinsertIndex, currentlySelected.Item);
		}

		private void lvProjects_DoubleClick(object sender, EventArgs e)
		{
			btnEdit_Click(sender, e);
		}

		private void btnEdit_Click(object sender, EventArgs e)
		{
			AddEditProject addEditProject = new AddEditProject(selected.Project);
			addEditProject.ShowDialog(this);

			selected.Rebind();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			Project newProject = new Project();
			AddEditProject addEditProject = new AddEditProject(newProject);
			if (addEditProject.ShowDialog(this) == DialogResult.OK)
			{
				lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(newProject).Item).Selected = true;
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			Project[] newProjectList = new Project[lvProjects.Items.Count];

			for (int i = 0; i < lvProjects.Items.Count; i++)
			{
				ProjectConfigurationListViewItemAdaptor adaptor = (ProjectConfigurationListViewItemAdaptor) lvProjects.Items[i].Tag;
				newProjectList[i] = adaptor.Project;
			}

			configuration.Projects = newProjectList;
			configuration.ShouldShowBalloonOnBuildTransition = chkShowBalloons.Checked;

			configuration.Audio.BrokenBuildSound = brokenAudio.Value;
			configuration.Audio.FixedBuildSound = fixedAudio.Value;
			configuration.Audio.StillFailingBuildSound = stillFailingAudio.Value;
			configuration.Audio.StillSuccessfulBuildSound = successfulAudio.Value;

			configuration.Persist();
		}


	}
}