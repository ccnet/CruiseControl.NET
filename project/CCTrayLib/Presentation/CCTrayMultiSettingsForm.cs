using System;
using System.ComponentModel;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public class CCTrayMultiSettingsForm : Form
	{
		private readonly ICCTrayMultiConfiguration configuration;
		private Button btnOK;
		private Button btnCancel;

		private ProjectConfigurationListViewItemAdaptor selected = null;
		private int selectedIndex = -1;
		private OpenFileDialog dlgOpenFile;

		private Container components = null;
		private SelectAudioFileController brokenAudio;
		private SelectAudioFileController fixedAudio;
		private SelectAudioFileController stillFailingAudio;
		private TabControl tabControl1;
		private TabPage tabGeneral;
		private RadioButton rdoWebPage;
		private RadioButton rdoStatusWindow;
		private Label label4;
		private NumericUpDown numPollPeriod;
		private Label label3;
		private Label label2;
		private CheckBox chkShowBalloons;
		private TabPage tabAudio;
		private CheckBox chkAudioSuccessful;
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
		private TabPage tabBuildServers;
		private ListView lvProjects;
		private Label label1;
		private Button btnAdd;
		private Button btnMoveDown;
		private Button btnMoveUp;
		private Button btnRemove;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;
		private ColumnHeader columnHeader3;
		private CheckBox chkCheck;
        private CheckBox chkAlwaysOnTop;
		private SelectAudioFileController successfulAudio;

		public CCTrayMultiSettingsForm(ICCTrayMultiConfiguration configuration)
		{
			this.configuration = configuration.Clone();

			InitializeComponent();

			chkShowBalloons.DataBindings.Add("Checked", configuration, "ShouldShowBalloonOnBuildTransition");
            chkAlwaysOnTop.DataBindings.Add("Checked", configuration, "AlwaysOnTop");

			rdoStatusWindow.Checked = (configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.ShowStatusWindow);
			rdoWebPage.Checked =
				(configuration.TrayIconDoubleClickAction == TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject);

			numPollPeriod.Value = configuration.PollPeriodSeconds;

			BindListView();

			BindAudioControls();
		}

		private void BindListView()
		{
			lvProjects.Items.Clear();

			foreach (CCTrayProject project in configuration.Projects)
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
				chkAudioStillFailing, txtAudioFileFailing, btnStillFailingBrowse, btnStillFailingPlay, dlgOpenFile,
				audioConfig.StillFailingBuildSound);
			successfulAudio = new SelectAudioFileController(
				chkAudioSuccessful, txtAudioFileSuccess, btnSuccessfulBrowse, btnSuccessfulPlay, dlgOpenFile,
				audioConfig.StillSuccessfulBuildSound);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CCTrayMultiSettingsForm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dlgOpenFile = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabBuildServers = new System.Windows.Forms.TabPage();
            this.chkCheck = new System.Windows.Forms.CheckBox();
            this.lvProjects = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnMoveDown = new System.Windows.Forms.Button();
            this.btnMoveUp = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.rdoWebPage = new System.Windows.Forms.RadioButton();
            this.rdoStatusWindow = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.numPollPeriod = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkShowBalloons = new System.Windows.Forms.CheckBox();
            this.tabAudio = new System.Windows.Forms.TabPage();
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
            this.chkAlwaysOnTop = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tabBuildServers.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPollPeriod)).BeginInit();
            this.tabAudio.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(256, 323);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(346, 323);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            // 
            // dlgOpenFile
            // 
            this.dlgOpenFile.DefaultExt = "wav";
            this.dlgOpenFile.Filter = "Wave Files|*.wav|All Files|*.*";
            this.dlgOpenFile.Title = "Select wave file";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabBuildServers);
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabAudio);
            this.tabControl1.Location = new System.Drawing.Point(1, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(675, 315);
            this.tabControl1.TabIndex = 11;
            // 
            // tabBuildServers
            // 
            this.tabBuildServers.Controls.Add(this.chkCheck);
            this.tabBuildServers.Controls.Add(this.lvProjects);
            this.tabBuildServers.Controls.Add(this.label1);
            this.tabBuildServers.Controls.Add(this.btnAdd);
            this.tabBuildServers.Controls.Add(this.btnMoveDown);
            this.tabBuildServers.Controls.Add(this.btnMoveUp);
            this.tabBuildServers.Controls.Add(this.btnRemove);
            this.tabBuildServers.Location = new System.Drawing.Point(4, 22);
            this.tabBuildServers.Name = "tabBuildServers";
            this.tabBuildServers.Size = new System.Drawing.Size(667, 289);
            this.tabBuildServers.TabIndex = 2;
            this.tabBuildServers.Text = "Build Projects";
            this.tabBuildServers.Visible = false;
            // 
            // chkCheck
            // 
            this.chkCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkCheck.AutoSize = true;
            this.chkCheck.Checked = true;
            this.chkCheck.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chkCheck.Location = new System.Drawing.Point(16, 267);
            this.chkCheck.Name = "chkCheck";
            this.chkCheck.Size = new System.Drawing.Size(122, 17);
            this.chkCheck.TabIndex = 15;
            this.chkCheck.Text = "check / uncheck all";
            this.chkCheck.UseVisualStyleBackColor = true;
            this.chkCheck.CheckedChanged += new System.EventHandler(this.chkCheck_CheckedChanged);
            // 
            // lvProjects
            // 
            this.lvProjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvProjects.CheckBoxes = true;
            this.lvProjects.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
            this.lvProjects.FullRowSelect = true;
            this.lvProjects.HideSelection = false;
            this.lvProjects.Location = new System.Drawing.Point(8, 40);
            this.lvProjects.Name = "lvProjects";
            this.lvProjects.Size = new System.Drawing.Size(540, 220);
            this.lvProjects.TabIndex = 8;
            this.lvProjects.UseCompatibleStateImageBehavior = false;
            this.lvProjects.View = System.Windows.Forms.View.Details;
            this.lvProjects.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.lvProjects_ItemChecked);
            this.lvProjects.SelectedIndexChanged += new System.EventHandler(this.lvProjects_SelectedIndexChanged);
            this.lvProjects.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvProjects_KeyDown);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Build Server";
            this.columnHeader1.Width = 135;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Transport";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Project";
            this.columnHeader2.Width = 287;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(625, 20);
            this.label1.TabIndex = 7;
            this.label1.Text = "Use this section to define the projects to monitor. ";
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnAdd.Location = new System.Drawing.Point(560, 48);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(75, 23);
            this.btnAdd.TabIndex = 9;
            this.btnAdd.Text = "&Add...";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveDown.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnMoveDown.Location = new System.Drawing.Point(560, 184);
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(75, 23);
            this.btnMoveDown.TabIndex = 12;
            this.btnMoveDown.Text = "Move &Down";
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveUp.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnMoveUp.Location = new System.Drawing.Point(560, 152);
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(75, 23);
            this.btnMoveUp.TabIndex = 11;
            this.btnMoveUp.Text = "Move &Up";
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnRemove.Location = new System.Drawing.Point(560, 80);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 10;
            this.btnRemove.Text = "&Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.chkAlwaysOnTop);
            this.tabGeneral.Controls.Add(this.rdoWebPage);
            this.tabGeneral.Controls.Add(this.rdoStatusWindow);
            this.tabGeneral.Controls.Add(this.label4);
            this.tabGeneral.Controls.Add(this.numPollPeriod);
            this.tabGeneral.Controls.Add(this.label3);
            this.tabGeneral.Controls.Add(this.label2);
            this.tabGeneral.Controls.Add(this.chkShowBalloons);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(667, 289);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
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
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(8, 64);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(185, 20);
            this.label4.TabIndex = 11;
            this.label4.Text = "When I double-click the tray icon,";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numPollPeriod
            // 
            this.numPollPeriod.Location = new System.Drawing.Point(104, 40);
            this.numPollPeriod.Name = "numPollPeriod";
            this.numPollPeriod.Size = new System.Drawing.Size(50, 20);
            this.numPollPeriod.TabIndex = 9;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(168, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "seconds";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 8;
            this.label2.Text = "Poll servers every";
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
            // tabAudio
            // 
            this.tabAudio.Controls.Add(this.chkAudioSuccessful);
            this.tabAudio.Controls.Add(this.btnStillFailingPlay);
            this.tabAudio.Controls.Add(this.txtAudioFileSuccess);
            this.tabAudio.Controls.Add(this.btnFixedPlay);
            this.tabAudio.Controls.Add(this.chkAudioFixed);
            this.tabAudio.Controls.Add(this.btnFixedBrowse);
            this.tabAudio.Controls.Add(this.btnStillFailingBrowse);
            this.tabAudio.Controls.Add(this.btnBrokenPlay);
            this.tabAudio.Controls.Add(this.btnBrokenBrowse);
            this.tabAudio.Controls.Add(this.btnSuccessfulPlay);
            this.tabAudio.Controls.Add(this.btnSuccessfulBrowse);
            this.tabAudio.Controls.Add(this.chkAudioBroken);
            this.tabAudio.Controls.Add(this.chkAudioStillFailing);
            this.tabAudio.Controls.Add(this.txtAudioFileFixed);
            this.tabAudio.Controls.Add(this.txtAudioFileBroken);
            this.tabAudio.Controls.Add(this.txtAudioFileFailing);
            this.tabAudio.Location = new System.Drawing.Point(4, 22);
            this.tabAudio.Name = "tabAudio";
            this.tabAudio.Size = new System.Drawing.Size(667, 289);
            this.tabAudio.TabIndex = 1;
            this.tabAudio.Text = "Audio";
            this.tabAudio.Visible = false;
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
            this.btnBrokenBrowse.Location = new System.Drawing.Point(472, 64);
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
            // CCTrayMultiSettingsForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(677, 361);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CCTrayMultiSettingsForm";
            this.Text = "CruiseControl.NET Tray Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabBuildServers.ResumeLayout(false);
            this.tabBuildServers.PerformLayout();
            this.tabGeneral.ResumeLayout(false);
            this.tabGeneral.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numPollPeriod)).EndInit();
            this.tabAudio.ResumeLayout(false);
            this.tabAudio.PerformLayout();
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
			btnRemove.Enabled = selected != null;

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
			lvProjects.BeginUpdate();
			foreach (ListViewItem item in lvProjects.SelectedItems)
			{
				lvProjects.Items.Remove(item);
			}
			lvProjects.EndUpdate();
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

		private void btnAdd_Click(object sender, EventArgs e)
		{
			AddProjects addProjectDialog = new AddProjects(configuration.CruiseProjectManagerFactory, BuildProjectListFromListView());
			CCTrayProject[] projects = addProjectDialog.GetListOfNewProjects(this);

			if (projects != null)
			{
				foreach (CCTrayProject newProject in projects)
				{
					lvProjects.Items.Add(new ProjectConfigurationListViewItemAdaptor(newProject).Item);
				}
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			CCTrayProject[] newProjectList = BuildProjectListFromListView();

			configuration.Projects = newProjectList;
			configuration.ShouldShowBalloonOnBuildTransition = chkShowBalloons.Checked;
            configuration.AlwaysOnTop = chkAlwaysOnTop.Checked;
            configuration.PollPeriodSeconds = (int) numPollPeriod.Value;

			configuration.TrayIconDoubleClickAction =
				(rdoStatusWindow.Checked
				 	? TrayIconDoubleClickAction.ShowStatusWindow
				 	: TrayIconDoubleClickAction.NavigateToWebPageOfFirstProject);

			configuration.Audio.BrokenBuildSound = brokenAudio.Value;
			configuration.Audio.FixedBuildSound = fixedAudio.Value;
			configuration.Audio.StillFailingBuildSound = stillFailingAudio.Value;
			configuration.Audio.StillSuccessfulBuildSound = successfulAudio.Value;

			configuration.Persist();
		}

		private CCTrayProject[] BuildProjectListFromListView()
		{
			CCTrayProject[] newProjectList = new CCTrayProject[lvProjects.Items.Count];

			for (int i = 0; i < lvProjects.Items.Count; i++)
			{
				ProjectConfigurationListViewItemAdaptor adaptor = (ProjectConfigurationListViewItemAdaptor) lvProjects.Items[i].Tag;
				newProjectList[i] = adaptor.Project;
			}
			return newProjectList;
		}
		
		private void lvProjects_ItemChecked(object sender, ItemCheckedEventArgs e)
		{
			foreach(CCTrayProject project in configuration.Projects)
			{
				if(e.Item.SubItems[2].Text == project.ProjectName)
				{
					project.ShowProject = e.Item.Checked;
				}
			}
		}
		
		private void chkCheck_CheckedChanged(object sender, EventArgs e)
		{
			if(chkCheck.Checked)
			{
				foreach (ListViewItem item in lvProjects.Items)
				{
					item.Checked = true;
				}
			}
			else
			{
				foreach (ListViewItem item in lvProjects.Items)
				{
					item.Checked = false;
				}
			}
		}

        private void lvProjects_KeyDown(object sender, KeyEventArgs e)
        {          
            if ((e.Modifiers == Keys.Control) && (e.KeyCode == Keys.A))
            {
                foreach (ListViewItem item in lvProjects.Items)
                {
                    item.Selected = true;
                }
            }

            if (e.KeyCode == Keys.Delete)
            {
                lvProjects.BeginUpdate();
                foreach (ListViewItem item in lvProjects.SelectedItems)
                {
                    lvProjects.Items.Remove(item);
                }

                if (lvProjects.Items.Count > 0)
                {
                    lvProjects.Items[0].Focused = true;
                }
                
                lvProjects.EndUpdate();
            }

            if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
                this.Close();
            }
        }                                                                                


	}
}
