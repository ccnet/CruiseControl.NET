using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class ConfigurationOptionsPage : WizardPageBase
    {
        public ConfigurationOptionsPage()
        {
            InitializeComponent();
            LinkNextPage(new WebDashboardOptionsPage());
        }

        public override void RunPage()
        {
            // Set the default values
            migrateConfig.Checked = MigrationOptions.MigrateConfiguration;
            backupFile.Checked = MigrationOptions.BackupConfiguration;
            settingsLocation.Text = MigrationOptions.ConfigurationLocation;
        }

        public override void CompletePage()
        {
            // Set the new values
            MigrationOptions.MigrateConfiguration = migrateConfig.Checked;
            MigrationOptions.BackupConfiguration = backupFile.Checked;
            MigrationOptions.ConfigurationLocation = settingsLocation.Text;
        }

        private void settingsLocation_TextChanged(object sender, EventArgs e)
        {
            ValidateSettings();
        }

        private void ValidateSettings()
        {
            var isValid = true;

            if (migrateConfig.Checked)
            {
                if (string.IsNullOrEmpty(settingsLocation.Text))
                {
                    errorProvider.SetError(selectLocationButton, "Configuration file is required");
                    isValid = false;
                }
                else if (!File.Exists(settingsLocation.Text))
                {
                    errorProvider.SetError(selectLocationButton, "Configuration file does not exist");
                    isValid = false;
                }
                else
                {
                    errorProvider.SetError(selectLocationButton, null);
                }
            }
            else
            {
                errorProvider.SetError(selectLocationButton, null);
            }

            IsValid = isValid;
        }

        private void migrateConfig_CheckedChanged(object sender, EventArgs e)
        {
            settingsPanel.Enabled = migrateConfig.Checked;
            ValidateSettings();
        }

        private void selectLocationButton_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                AddExtension = false,
                AutoUpgradeEnabled = true,
                CheckFileExists = true,
                DefaultExt = "config",
                DereferenceLinks = true,
                FileName = settingsLocation.Text,
                Filter = "ccnet.config|ccnet.config",
                FilterIndex = 1,
                Multiselect = false,
                ShowHelp = false,
                ShowReadOnly = false,   
                Title = "Select Configuration File"
            };
            if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
            {
                settingsLocation.Text = dialog.FileName;
            }
        }
    }
}
