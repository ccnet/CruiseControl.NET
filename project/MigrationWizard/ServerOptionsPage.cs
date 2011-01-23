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
    public partial class ServerOptionsPage : WizardPageBase
    {
        public ServerOptionsPage()
        {
            InitializeComponent();
            LinkNextPage(new ConfigurationOptionsPage());
        }

        public override void RunPage()
        {
            // Set the default values
            migrateServer.Checked = MigrationOptions.MigrateServer;
            settingsLocation.Text = MigrationOptions.CurrentServerLocation;
            backupFile.Checked = MigrationOptions.BackupServerConfiguration;
            ValidateSettings();
        }

        public override void CompletePage()
        {
            if (Path.GetDirectoryName(MigrationOptions.ConfigurationLocation) == MigrationOptions.CurrentServerLocation)
            {
                MigrationOptions.ConfigurationLocation = Path.Combine(settingsLocation.Text, "ccnet.config");
            }

            // Set the new values
            MigrationOptions.MigrateServer = migrateServer.Checked;
            MigrationOptions.CurrentServerLocation = settingsLocation.Text;
            MigrationOptions.BackupServerConfiguration = backupFile.Checked;
        }

        private void migrateServer_CheckedChanged(object sender, EventArgs e)
        {
            settingsPanel.Enabled = migrateServer.Checked;
            ValidateSettings();
        }

        private void settingsLocation_TextChanged(object sender, EventArgs e)
        {
            ValidateSettings();
        }

        private void ValidateSettings()
        {
            var isValid = true;

            if (migrateServer.Checked)
            {
                if (string.IsNullOrEmpty(settingsLocation.Text))
                {
                    errorProvider.SetError(selectLocationButton, "Server settings location is required");
                    isValid = false;
                }
                else if (!Directory.Exists(settingsLocation.Text))
                {
                    errorProvider.SetError(selectLocationButton, "Server settings location does not exist");
                    isValid = false;
                }
                else if (!File.Exists(Path.Combine(settingsLocation.Text, "ccnet.config")))
                {
                    errorProvider.SetError(selectLocationButton, "ccnet.config not found in server settings location");
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

        private void selectLocationButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "Select the current location of your CruiseControl.NET server installation",
                SelectedPath = settingsLocation.Text,
                ShowNewFolderButton = false
            };
            if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
            {
                settingsLocation.Text = dialog.SelectedPath;
            }
        }
    }
}
