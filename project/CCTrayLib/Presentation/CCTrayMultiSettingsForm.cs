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

		private AudioSettingsControl audioSettings;
		private IconSettingsControl iconSettings;
		private GeneralSettingsControl generalSettings;
		private BuildProjectsControl buildProjectsSettings;
        private X10SettingsControl x10Settings;
        private SpeechSettingsControl speechSettings;

		private TabPage tabAudio;
		private TabPage tabIcons;
		private TabPage tabGeneral;
		private TabPage tabBuildProjects;
        private TabPage tabX10;
        private TabPage tabSpeech;
        private TabControl tabControl1;

		private Container components = null;

		public CCTrayMultiSettingsForm(ICCTrayMultiConfiguration existingConfiguration)
		{
			// We will clone the existing configuration, so if the user cancels the dialog
			// our in-memory configuration will still have the old values.
			configuration = existingConfiguration.Clone();

			InitializeComponent();

			generalSettings.BindGeneralTabControls(configuration);
			buildProjectsSettings.BindListView(configuration);
			audioSettings.BindAudioControls(configuration);
			iconSettings.BindIconControls(configuration);
			x10Settings.BindX10Controls(configuration.X10);
			speechSettings.BindSpeechControls(configuration.Speech);
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.generalSettings = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.GeneralSettingsControl();
            this.tabBuildProjects = new System.Windows.Forms.TabPage();
            this.buildProjectsSettings = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.BuildProjectsControl();
            this.tabAudio = new System.Windows.Forms.TabPage();
            this.audioSettings = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.AudioSettingsControl();
			this.tabIcons = new System.Windows.Forms.TabPage();
			this.iconSettings = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.IconSettingsControl();
            this.tabX10 = new System.Windows.Forms.TabPage();
            this.x10Settings = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.X10SettingsControl();
            this.tabSpeech = new System.Windows.Forms.TabPage();
            this.speechSettings = new ThoughtWorks.CruiseControl.CCTrayLib.Presentation.SpeechSettingsControl();
            this.tabControl1.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.tabBuildProjects.SuspendLayout();
            this.tabAudio.SuspendLayout();
			this.tabIcons.SuspendLayout();
            this.tabX10.SuspendLayout();
            this.tabSpeech.SuspendLayout();
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
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabBuildProjects);
            this.tabControl1.Controls.Add(this.tabAudio);
			this.tabControl1.Controls.Add(this.tabIcons);
            this.tabControl1.Controls.Add(this.tabX10);
            this.tabControl1.Controls.Add(this.tabSpeech);
            this.tabControl1.Location = new System.Drawing.Point(1, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(675, 315);
            this.tabControl1.TabIndex = 11;
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.generalSettings);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Size = new System.Drawing.Size(667, 289);
            this.tabGeneral.TabIndex = 0;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // generalSettings
            // 
            this.generalSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.generalSettings.Location = new System.Drawing.Point(0, 0);
            this.generalSettings.Name = "generalSettings";
            this.generalSettings.Size = new System.Drawing.Size(667, 289);
            this.generalSettings.TabIndex = 1;
            // 
            // tabBuildProjects
            // 
            this.tabBuildProjects.Controls.Add(this.buildProjectsSettings);
            this.tabBuildProjects.Location = new System.Drawing.Point(4, 22);
            this.tabBuildProjects.Name = "tabBuildProjects";
            this.tabBuildProjects.Size = new System.Drawing.Size(192, 74);
            this.tabBuildProjects.TabIndex = 2;
            this.tabBuildProjects.Text = "Build Projects";
            this.tabBuildProjects.UseVisualStyleBackColor = true;
            this.tabBuildProjects.Visible = false;
            // 
            // buildProjectsSettings
            // 
            this.buildProjectsSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.buildProjectsSettings.Location = new System.Drawing.Point(0, 0);
            this.buildProjectsSettings.Name = "buildProjectsSettings";
            this.buildProjectsSettings.Size = new System.Drawing.Size(192, 74);
            this.buildProjectsSettings.TabIndex = 2;
            // 
            // tabAudio
            // 
            this.tabAudio.Controls.Add(this.audioSettings);
            this.tabAudio.Location = new System.Drawing.Point(4, 22);
            this.tabAudio.Name = "tabAudio";
            this.tabAudio.Size = new System.Drawing.Size(192, 74);
            this.tabAudio.TabIndex = 1;
            this.tabAudio.Text = "Audio";
            this.tabAudio.UseVisualStyleBackColor = true;
            this.tabAudio.Visible = false;
            // 
            // audioSettings
            // 
            this.audioSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.audioSettings.Location = new System.Drawing.Point(0, 0);
            this.audioSettings.Name = "audioSettings";
            this.audioSettings.Size = new System.Drawing.Size(192, 74);
            this.audioSettings.TabIndex = 2;
			// 
			// tabIcons
			// 
			this.tabIcons.Controls.Add(this.iconSettings);
			this.tabIcons.Location = new System.Drawing.Point(4, 22);
			this.tabIcons.Name = "tabIcons";
			this.tabIcons.Size = new System.Drawing.Size(192, 74);
			this.tabIcons.TabIndex = 2;
			this.tabIcons.Text = "Icons";
			this.tabIcons.UseVisualStyleBackColor = true;
			this.tabIcons.Visible = false;
			// 
			// iconSettings
			// 
			this.iconSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.iconSettings.Location = new System.Drawing.Point(0, 0);
			this.iconSettings.Name = "iconSettings";
			this.iconSettings.Size = new System.Drawing.Size(192, 74);
			this.iconSettings.TabIndex = 2;
            // 
            // tabX10
            // 
            this.tabX10.Controls.Add(this.x10Settings);
            this.tabX10.Location = new System.Drawing.Point(4, 22);
            this.tabX10.Name = "tabX10";
            this.tabX10.Padding = new System.Windows.Forms.Padding(3);
            this.tabX10.Size = new System.Drawing.Size(667, 289);
            this.tabX10.TabIndex = 3;
            this.tabX10.Text = "X10";
            this.tabX10.UseVisualStyleBackColor = true;
            // 
            // x10Settings
            // 
            this.x10Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.x10Settings.Location = new System.Drawing.Point(3, 3);
            this.x10Settings.Name = "x10Settings";
            this.x10Settings.Size = new System.Drawing.Size(1128, 472);
            this.x10Settings.TabIndex = 2;
            // 
            // tabSpeech
            //
            this.tabSpeech.Controls.Add(this.speechSettings);
            this.tabSpeech.Location = new System.Drawing.Point(4, 22);
            this.tabSpeech.Name = "tabSpeech";
            this.tabSpeech.Padding = new System.Windows.Forms.Padding(3);
            this.tabSpeech.Size = new System.Drawing.Size(667, 289);
            this.tabSpeech.TabIndex = 4;
            this.tabSpeech.Text = "Speech";
            this.tabX10.UseVisualStyleBackColor = true;
            
            //
            // speechSettings
            //
            this.speechSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.speechSettings.Location = new System.Drawing.Point(3, 3);
            this.speechSettings.Name = "speechSettings";
            this.speechSettings.Size = new System.Drawing.Size(1128, 472);
            this.speechSettings.TabIndex = 3;
            
            // 
            // CCTrayMultiSettingsForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(677, 361);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "CCTrayMultiSettingsForm";
            this.Text = "CruiseControl.NET Tray Settings";
            this.tabControl1.ResumeLayout(false);
            this.tabGeneral.ResumeLayout(false);
            this.tabBuildProjects.ResumeLayout(false);
            this.tabAudio.ResumeLayout(false);
			this.tabIcons.ResumeLayout(false);
            this.tabX10.ResumeLayout(false);
            this.tabSpeech.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private void btnOK_Click(object sender, EventArgs e)
		{
			buildProjectsSettings.PersistProjectTabSettings();
            generalSettings.PersistGeneralTabSettings(configuration);
            audioSettings.PersistAudioTabSettings(configuration);
			iconSettings.PersistIconTabSettings(configuration);
			x10Settings.PersistX10TabSettings(configuration.X10);
            speechSettings.PersistSpeechTabSettings(configuration.Speech);

			configuration.Persist();
		}
	}
}
