using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
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
		private Label label2;
		private Label label3;
		private NumericUpDown numPollPeriod;
		private RadioButton rdoWebPage;
		private RadioButton rdoStatusWindow;
		private Label label4;
		private SelectAudioFileController successfulAudio;

		public CCTrayMultiSettingsForm(ICCTrayMultiConfiguration configuration)
		{
			this.configuration = configuration.Clone();

			InitializeComponent();

			chkShowBalloons.DataBindings.Add("Checked", configuration, "ShouldShowBalloonOnBuildTransition");

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
			ResourceManager resources = new ResourceManager(typeof (CCTrayMultiSettingsForm));
			this.chkShowBalloons = new CheckBox();
			this.grpServers = new GroupBox();
			this.btnEdit = new Button();
			this.lvProjects = new ListView();
			this.columnHeader1 = new ColumnHeader();
			this.columnHeader2 = new ColumnHeader();
			this.label1 = new Label();
			this.btnAdd = new Button();
			this.btnMoveDown = new Button();
			this.btnMoveUp = new Button();
			this.btnRemove = new Button();
			this.btnOK = new Button();
			this.btnCancel = new Button();
			this.grpAudio = new GroupBox();
			this.btnStillFailingPlay = new Button();
			this.btnStillFailingBrowse = new Button();
			this.btnBrokenPlay = new Button();
			this.btnBrokenBrowse = new Button();
			this.btnFixedPlay = new Button();
			this.btnFixedBrowse = new Button();
			this.btnSuccessfulPlay = new Button();
			this.btnSuccessfulBrowse = new Button();
			this.txtAudioFileSuccess = new TextBox();
			this.chkAudioSuccessful = new CheckBox();
			this.chkAudioBroken = new CheckBox();
			this.chkAudioFixed = new CheckBox();
			this.chkAudioStillFailing = new CheckBox();
			this.txtAudioFileFixed = new TextBox();
			this.txtAudioFileBroken = new TextBox();
			this.txtAudioFileFailing = new TextBox();
			this.dlgOpenFile = new OpenFileDialog();
			this.label2 = new Label();
			this.label3 = new Label();
			this.numPollPeriod = new NumericUpDown();
			this.rdoWebPage = new RadioButton();
			this.rdoStatusWindow = new RadioButton();
			this.label4 = new Label();
			this.grpServers.SuspendLayout();
			this.grpAudio.SuspendLayout();
			((ISupportInitialize) (this.numPollPeriod)).BeginInit();
			this.SuspendLayout();
			// 
			// chkShowBalloons
			// 
			this.chkShowBalloons.FlatStyle = FlatStyle.System;
			this.chkShowBalloons.Location = new Point(15, 10);
			this.chkShowBalloons.Name = "chkShowBalloons";
			this.chkShowBalloons.Size = new Size(248, 24);
			this.chkShowBalloons.TabIndex = 0;
			this.chkShowBalloons.Text = "Show balloon notifications";
			// 
			// grpServers
			// 
			this.grpServers.Anchor = ((AnchorStyles) ((((AnchorStyles.Top | AnchorStyles.Bottom)
			                                            | AnchorStyles.Left)
			                                           | AnchorStyles.Right)));
			this.grpServers.Controls.Add(this.btnEdit);
			this.grpServers.Controls.Add(this.lvProjects);
			this.grpServers.Controls.Add(this.label1);
			this.grpServers.Controls.Add(this.btnAdd);
			this.grpServers.Controls.Add(this.btnMoveDown);
			this.grpServers.Controls.Add(this.btnMoveUp);
			this.grpServers.Controls.Add(this.btnRemove);
			this.grpServers.FlatStyle = FlatStyle.System;
			this.grpServers.Location = new Point(15, 250);
			this.grpServers.Name = "grpServers";
			this.grpServers.Size = new Size(580, 245);
			this.grpServers.TabIndex = 8;
			this.grpServers.TabStop = false;
			this.grpServers.Text = "Build Servers";
			// 
			// btnEdit
			// 
			this.btnEdit.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnEdit.FlatStyle = FlatStyle.System;
			this.btnEdit.Location = new Point(495, 95);
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.TabIndex = 3;
			this.btnEdit.Text = "&Edit...";
			this.btnEdit.Click += new EventHandler(this.btnEdit_Click);
			// 
			// lvProjects
			// 
			this.lvProjects.Anchor = ((AnchorStyles) ((((AnchorStyles.Top | AnchorStyles.Bottom)
			                                            | AnchorStyles.Left)
			                                           | AnchorStyles.Right)));
			this.lvProjects.Columns.AddRange(new ColumnHeader[]
			                                 	{
			                                 		this.columnHeader1,
			                                 		this.columnHeader2
			                                 	});
			this.lvProjects.FullRowSelect = true;
			this.lvProjects.HideSelection = false;
			this.lvProjects.Location = new Point(10, 55);
			this.lvProjects.Name = "lvProjects";
			this.lvProjects.Size = new Size(475, 180);
			this.lvProjects.TabIndex = 1;
			this.lvProjects.View = View.Details;
			this.lvProjects.DoubleClick += new EventHandler(this.lvProjects_DoubleClick);
			this.lvProjects.SelectedIndexChanged += new EventHandler(this.lvProjects_SelectedIndexChanged);
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
			this.label1.Anchor = ((AnchorStyles) (((AnchorStyles.Top | AnchorStyles.Left)
			                                       | AnchorStyles.Right)));
			this.label1.Location = new Point(10, 25);
			this.label1.Name = "label1";
			this.label1.Size = new Size(560, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Use this section to define the CruiseControl.NET projects to monitor. ";
			// 
			// btnAdd
			// 
			this.btnAdd.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnAdd.FlatStyle = FlatStyle.System;
			this.btnAdd.Location = new Point(495, 60);
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.TabIndex = 2;
			this.btnAdd.Text = "&Add...";
			this.btnAdd.Click += new EventHandler(this.btnAdd_Click);
			// 
			// btnMoveDown
			// 
			this.btnMoveDown.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnMoveDown.FlatStyle = FlatStyle.System;
			this.btnMoveDown.Location = new Point(495, 200);
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.TabIndex = 6;
			this.btnMoveDown.Text = "Move &Down";
			this.btnMoveDown.Click += new EventHandler(this.btnMoveDown_Click);
			// 
			// btnMoveUp
			// 
			this.btnMoveUp.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnMoveUp.FlatStyle = FlatStyle.System;
			this.btnMoveUp.Location = new Point(495, 165);
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.TabIndex = 5;
			this.btnMoveUp.Text = "Move &Up";
			this.btnMoveUp.Click += new EventHandler(this.btnMoveUp_Click);
			// 
			// btnRemove
			// 
			this.btnRemove.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnRemove.FlatStyle = FlatStyle.System;
			this.btnRemove.Location = new Point(495, 130);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.TabIndex = 4;
			this.btnRemove.Text = "&Remove";
			this.btnRemove.Click += new EventHandler(this.btnRemove_Click);
			// 
			// btnOK
			// 
			this.btnOK.Anchor = AnchorStyles.Bottom;
			this.btnOK.DialogResult = DialogResult.OK;
			this.btnOK.FlatStyle = FlatStyle.System;
			this.btnOK.Location = new Point(224, 508);
			this.btnOK.Name = "btnOK";
			this.btnOK.TabIndex = 9;
			this.btnOK.Text = "OK";
			this.btnOK.Click += new EventHandler(this.btnOK_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = AnchorStyles.Bottom;
			this.btnCancel.DialogResult = DialogResult.Cancel;
			this.btnCancel.FlatStyle = FlatStyle.System;
			this.btnCancel.Location = new Point(314, 508);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 10;
			this.btnCancel.Text = "Cancel";
			// 
			// grpAudio
			// 
			this.grpAudio.Anchor = ((AnchorStyles) (((AnchorStyles.Top | AnchorStyles.Left)
			                                         | AnchorStyles.Right)));
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
			this.grpAudio.FlatStyle = FlatStyle.System;
			this.grpAudio.Location = new Point(15, 115);
			this.grpAudio.Name = "grpAudio";
			this.grpAudio.Size = new Size(580, 128);
			this.grpAudio.TabIndex = 7;
			this.grpAudio.TabStop = false;
			this.grpAudio.Text = "Audio";
			// 
			// btnStillFailingPlay
			// 
			this.btnStillFailingPlay.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnStillFailingPlay.FlatStyle = FlatStyle.System;
			this.btnStillFailingPlay.Location = new Point(495, 95);
			this.btnStillFailingPlay.Name = "btnStillFailingPlay";
			this.btnStillFailingPlay.TabIndex = 15;
			this.btnStillFailingPlay.Text = "Play!";
			// 
			// btnStillFailingBrowse
			// 
			this.btnStillFailingBrowse.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnStillFailingBrowse.FlatStyle = FlatStyle.System;
			this.btnStillFailingBrowse.Location = new Point(410, 95);
			this.btnStillFailingBrowse.Name = "btnStillFailingBrowse";
			this.btnStillFailingBrowse.TabIndex = 14;
			this.btnStillFailingBrowse.Text = "Browse...";
			// 
			// btnBrokenPlay
			// 
			this.btnBrokenPlay.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnBrokenPlay.FlatStyle = FlatStyle.System;
			this.btnBrokenPlay.Location = new Point(495, 71);
			this.btnBrokenPlay.Name = "btnBrokenPlay";
			this.btnBrokenPlay.TabIndex = 11;
			this.btnBrokenPlay.Text = "Play!";
			// 
			// btnBrokenBrowse
			// 
			this.btnBrokenBrowse.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnBrokenBrowse.FlatStyle = FlatStyle.System;
			this.btnBrokenBrowse.Location = new Point(410, 71);
			this.btnBrokenBrowse.Name = "btnBrokenBrowse";
			this.btnBrokenBrowse.TabIndex = 10;
			this.btnBrokenBrowse.Text = "Browse...";
			// 
			// btnFixedPlay
			// 
			this.btnFixedPlay.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnFixedPlay.FlatStyle = FlatStyle.System;
			this.btnFixedPlay.Location = new Point(495, 47);
			this.btnFixedPlay.Name = "btnFixedPlay";
			this.btnFixedPlay.TabIndex = 7;
			this.btnFixedPlay.Text = "Play!";
			// 
			// btnFixedBrowse
			// 
			this.btnFixedBrowse.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnFixedBrowse.FlatStyle = FlatStyle.System;
			this.btnFixedBrowse.Location = new Point(410, 47);
			this.btnFixedBrowse.Name = "btnFixedBrowse";
			this.btnFixedBrowse.TabIndex = 6;
			this.btnFixedBrowse.Text = "Browse...";
			// 
			// btnSuccessfulPlay
			// 
			this.btnSuccessfulPlay.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnSuccessfulPlay.FlatStyle = FlatStyle.System;
			this.btnSuccessfulPlay.Location = new Point(495, 23);
			this.btnSuccessfulPlay.Name = "btnSuccessfulPlay";
			this.btnSuccessfulPlay.TabIndex = 3;
			this.btnSuccessfulPlay.Text = "Play!";
			// 
			// btnSuccessfulBrowse
			// 
			this.btnSuccessfulBrowse.Anchor = ((AnchorStyles) ((AnchorStyles.Top | AnchorStyles.Right)));
			this.btnSuccessfulBrowse.FlatStyle = FlatStyle.System;
			this.btnSuccessfulBrowse.Location = new Point(410, 23);
			this.btnSuccessfulBrowse.Name = "btnSuccessfulBrowse";
			this.btnSuccessfulBrowse.TabIndex = 2;
			this.btnSuccessfulBrowse.Text = "Browse...";
			// 
			// txtAudioFileSuccess
			// 
			this.txtAudioFileSuccess.Anchor = ((AnchorStyles) (((AnchorStyles.Top | AnchorStyles.Left)
			                                                    | AnchorStyles.Right)));
			this.txtAudioFileSuccess.Location = new Point(112, 24);
			this.txtAudioFileSuccess.Name = "txtAudioFileSuccess";
			this.txtAudioFileSuccess.Size = new Size(288, 20);
			this.txtAudioFileSuccess.TabIndex = 1;
			this.txtAudioFileSuccess.Text = "";
			// 
			// chkAudioSuccessful
			// 
			this.chkAudioSuccessful.BackColor = SystemColors.Control;
			this.chkAudioSuccessful.FlatStyle = FlatStyle.System;
			this.chkAudioSuccessful.Location = new Point(16, 24);
			this.chkAudioSuccessful.Name = "chkAudioSuccessful";
			this.chkAudioSuccessful.Size = new Size(96, 16);
			this.chkAudioSuccessful.TabIndex = 0;
			this.chkAudioSuccessful.Text = "Successful";
			// 
			// chkAudioBroken
			// 
			this.chkAudioBroken.BackColor = SystemColors.Control;
			this.chkAudioBroken.FlatStyle = FlatStyle.System;
			this.chkAudioBroken.Location = new Point(16, 72);
			this.chkAudioBroken.Name = "chkAudioBroken";
			this.chkAudioBroken.Size = new Size(96, 16);
			this.chkAudioBroken.TabIndex = 8;
			this.chkAudioBroken.Text = "Broken";
			// 
			// chkAudioFixed
			// 
			this.chkAudioFixed.BackColor = SystemColors.Control;
			this.chkAudioFixed.FlatStyle = FlatStyle.System;
			this.chkAudioFixed.Location = new Point(16, 48);
			this.chkAudioFixed.Name = "chkAudioFixed";
			this.chkAudioFixed.Size = new Size(96, 16);
			this.chkAudioFixed.TabIndex = 4;
			this.chkAudioFixed.Text = "Fixed";
			// 
			// chkAudioStillFailing
			// 
			this.chkAudioStillFailing.BackColor = SystemColors.Control;
			this.chkAudioStillFailing.FlatStyle = FlatStyle.System;
			this.chkAudioStillFailing.Location = new Point(16, 96);
			this.chkAudioStillFailing.Name = "chkAudioStillFailing";
			this.chkAudioStillFailing.Size = new Size(96, 16);
			this.chkAudioStillFailing.TabIndex = 12;
			this.chkAudioStillFailing.Text = "Still failing";
			// 
			// txtAudioFileFixed
			// 
			this.txtAudioFileFixed.Anchor = ((AnchorStyles) (((AnchorStyles.Top | AnchorStyles.Left)
			                                                  | AnchorStyles.Right)));
			this.txtAudioFileFixed.Location = new Point(112, 48);
			this.txtAudioFileFixed.Name = "txtAudioFileFixed";
			this.txtAudioFileFixed.Size = new Size(288, 20);
			this.txtAudioFileFixed.TabIndex = 5;
			this.txtAudioFileFixed.Text = "";
			// 
			// txtAudioFileBroken
			// 
			this.txtAudioFileBroken.Anchor = ((AnchorStyles) (((AnchorStyles.Top | AnchorStyles.Left)
			                                                   | AnchorStyles.Right)));
			this.txtAudioFileBroken.Location = new Point(112, 72);
			this.txtAudioFileBroken.Name = "txtAudioFileBroken";
			this.txtAudioFileBroken.Size = new Size(288, 20);
			this.txtAudioFileBroken.TabIndex = 9;
			this.txtAudioFileBroken.Text = "";
			// 
			// txtAudioFileFailing
			// 
			this.txtAudioFileFailing.Anchor = ((AnchorStyles) (((AnchorStyles.Top | AnchorStyles.Left)
			                                                    | AnchorStyles.Right)));
			this.txtAudioFileFailing.Location = new Point(112, 96);
			this.txtAudioFileFailing.Name = "txtAudioFileFailing";
			this.txtAudioFileFailing.Size = new Size(288, 20);
			this.txtAudioFileFailing.TabIndex = 13;
			this.txtAudioFileFailing.Text = "";
			// 
			// dlgOpenFile
			// 
			this.dlgOpenFile.DefaultExt = "wav";
			this.dlgOpenFile.Filter = "Wave Files|*.wav|All Files|*.*";
			this.dlgOpenFile.Title = "Select wave file";
			// 
			// label2
			// 
			this.label2.Location = new Point(15, 40);
			this.label2.Name = "label2";
			this.label2.Size = new Size(100, 20);
			this.label2.TabIndex = 1;
			this.label2.Text = "Poll servers every";
			// 
			// label3
			// 
			this.label3.Location = new Point(175, 40);
			this.label3.Name = "label3";
			this.label3.Size = new Size(100, 20);
			this.label3.TabIndex = 3;
			this.label3.Text = "seconds";
			// 
			// numPollPeriod
			// 
			this.numPollPeriod.Location = new Point(115, 37);
			this.numPollPeriod.Name = "numPollPeriod";
			this.numPollPeriod.Size = new Size(50, 20);
			this.numPollPeriod.TabIndex = 2;
			// 
			// rdoWebPage
			// 
			this.rdoWebPage.Location = new Point(195, 85);
			this.rdoWebPage.Name = "rdoWebPage";
			this.rdoWebPage.Size = new Size(310, 20);
			this.rdoWebPage.TabIndex = 6;
			this.rdoWebPage.Text = "navigate to the web page of the first project on the list";
			// 
			// rdoStatusWindow
			// 
			this.rdoStatusWindow.Location = new Point(195, 65);
			this.rdoStatusWindow.Name = "rdoStatusWindow";
			this.rdoStatusWindow.Size = new Size(230, 20);
			this.rdoStatusWindow.TabIndex = 5;
			this.rdoStatusWindow.Text = "show the status window";
			// 
			// label4
			// 
			this.label4.Location = new Point(15, 65);
			this.label4.Name = "label4";
			this.label4.Size = new Size(185, 20);
			this.label4.TabIndex = 4;
			this.label4.Text = "When I double-click the tray icon,";
			this.label4.TextAlign = ContentAlignment.MiddleLeft;
			// 
			// CCTrayMultiSettingsForm
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleBaseSize = new Size(5, 13);
			this.CancelButton = this.btnCancel;
			this.ClientSize = new Size(612, 546);
			this.Controls.Add(this.rdoWebPage);
			this.Controls.Add(this.rdoStatusWindow);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.numPollPeriod);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.grpAudio);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.Controls.Add(this.grpServers);
			this.Controls.Add(this.chkShowBalloons);
			this.Icon = ((Icon) (resources.GetObject("$this.Icon")));
			this.Name = "CCTrayMultiSettingsForm";
			this.Text = "CruiseControl.NET Tray Settings";
			this.grpServers.ResumeLayout(false);
			this.grpAudio.ResumeLayout(false);
			((ISupportInitialize) (this.numPollPeriod)).EndInit();
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
			newProject.ServerUrl = "tcp://localhost:21234/CruiseManager.rem";

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


	}
}