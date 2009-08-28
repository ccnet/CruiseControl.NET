using System;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
	public partial class CCTrayMultiSettingsForm : Form
	{
		private readonly ICCTrayMultiConfiguration configuration;
		private PersistWindowState windowState;

		public CCTrayMultiSettingsForm(ICCTrayMultiConfiguration existingConfiguration)
		{
			// We clone the existing configuration, so if the user cancels the dialog
			// our in-memory configuration will still have the old values.
			configuration = existingConfiguration.Clone();

			InitializeComponent();

            BindAllSettings();
			HookPersistentWindowState();
            tabControl1.SelectedTab = tabGeneral;       // Default to the general settings tab
		}

        private void BindAllSettings()
        {
            generalSettings.BindGeneralTabControls(configuration);
            buildProjectsSettings.BindListView(configuration);
            audioSettings.BindAudioControls(configuration);
            iconSettings.BindIconControls(configuration);
            x10Settings.BindX10Controls(configuration.X10);
            speechSettings.BindSpeechControls(configuration.Speech);
			growlSettings.BindGrowlControls(configuration.Growl);
            execSettingsControl1.BindExecControls(configuration);
        }

		private void HookPersistentWindowState()
		{
			windowState = new PersistWindowState(this);
			windowState.RegistryPath = @"Software\ThoughtWorks\CCTray\Windows\CCTrayMultiSettingsForm";

			windowState.LoadState += OnLoadState;
			windowState.SaveState += OnSaveState;
		}

		private void OnSaveState(object sender, WindowStateEventArgs e)
		{
			e.Key.SetValue("ActiveTab", tabControl1.SelectedTab.Name);
		}

		private void OnLoadState(object sender, WindowStateEventArgs e)
		{
			string activeTabName = (string) e.Key.GetValue("ActiveTab");
			int i = tabControl1.TabPages.IndexOfKey(activeTabName);
			if (0 <= i && i < tabControl1.TabCount)
				tabControl1.SelectedIndex = i;
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			buildProjectsSettings.PersistProjectTabSettings();
            generalSettings.PersistGeneralTabSettings(configuration);
            audioSettings.PersistAudioTabSettings(configuration);
			iconSettings.PersistIconTabSettings(configuration);
			x10Settings.PersistX10TabSettings(configuration.X10);
            speechSettings.PersistSpeechTabSettings(configuration.Speech);
			growlSettings.PersistGrowlTabSettings(configuration.Growl);
            execSettingsControl1.PersistExecTabSettings(configuration);
			configuration.Persist();
		}

        private void saveSettingsButton_Click(object sender, EventArgs e)
        {
            var saveDialog = new SaveFileDialog
            {
                AddExtension = true,
                AutoUpgradeEnabled = true,
                CheckPathExists = true,
                DefaultExt = "cctray",
                Filter = "CCTray files (*.cctray)|*.cctray|All files (*.*)|*.*",
                FilterIndex = 1,
                OverwritePrompt = true,
                Title = "Save Current Settings",
                ValidateNames = true
            };
            if (saveDialog.ShowDialog(this) == DialogResult.OK)
            {
                configuration.Save(saveDialog.FileName);
                MessageBox.Show("Settings have been saved", "Save Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void loadSettingsButton_Click(object sender, EventArgs e)
        {
            var openDialog = new OpenFileDialog
            {
                AddExtension = true,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                DefaultExt = "cctray",
                Filter = "CCTray files (*.cctray)|*.cctray|All files (*.*)|*.*",
                FilterIndex = 1,
                Multiselect = false,
                ShowReadOnly = false,
                Title = "Load Previous Settings",
                ValidateNames = true
            };
            if (openDialog.ShowDialog(this) == DialogResult.OK)
            {
                configuration.Load(openDialog.FileName);
                BindAllSettings();
                MessageBox.Show("Settings have been loaded", "Load Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
	}
}
