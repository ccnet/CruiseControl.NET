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
    public partial class WebDashboardOptionsPage : WizardPageBase
    {
        public WebDashboardOptionsPage()
        {
            InitializeComponent();
            LinkNextPage(new ConfirmationPage());
        }

        public override void RunPage()
        {
            // Set the default values
            migrateDashboard.Checked = MigrationOptions.MigrateWebDashboard;
            settingsLocation.Text = MigrationOptions.CurrentWebDashboardLocation;
        }

        public override void CompletePage()
        {
            // Set the new values
            MigrationOptions.MigrateWebDashboard = migrateDashboard.Checked;
            MigrationOptions.CurrentWebDashboardLocation = settingsLocation.Text;
        }

        private void migrateDashboard_CheckedChanged(object sender, EventArgs e)
        {
            settingsPanel.Enabled = migrateDashboard.Checked;
            ValidateSettings();
        }

        private void settingsLocation_TextChanged(object sender, EventArgs e)
        {
            ValidateSettings();
        }

        private void ValidateSettings()
        {
            var isValid = true;

            if (migrateDashboard.Checked)
            {
                if (string.IsNullOrEmpty(settingsLocation.Text))
                {
                    errorProvider.SetError(selectLocationButton, "Web dashboard settings location is required");
                    isValid = false;
                }
                else if (!Directory.Exists(settingsLocation.Text))
                {
                    errorProvider.SetError(selectLocationButton, "Web dashboard settings location does not exist");
                    isValid = false;
                }
                else if (!File.Exists(Path.Combine(settingsLocation.Text, "dashboard.config")))
                {
                    errorProvider.SetError(selectLocationButton, "dashboard.config not found in web dashboard settings location");
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
                Description = "Select the current location of your CruiseControl.NET Web Dashboard installation",
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
