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
    public partial class ConfirmationPage : WizardPageBase
    {
        public ConfirmationPage()
        {
            InitializeComponent();
            NextPage = new ProgressPage();
        }

        public override void RunPage()
        {
            var builder = new StringBuilder();
            if (MigrationOptions.MigrateServer)
            {
                builder.AppendFormat("Migrate server files from '{0}'{2}\tto '{1}'{2}",
                    MigrationOptions.CurrentServerLocation,
                    MigrationOptions.NewServerLocation,
                    Environment.NewLine);
                builder.AppendLine();
            }
            if (MigrationOptions.MigrateConfiguration)
            {
                builder.AppendFormat("Migrate configuration settings in '{0}'{1}",
                    MigrationOptions.ConfigurationLocation,
                    Environment.NewLine);
                if (MigrationOptions.BackupConfiguration)
                {
                    builder.AppendFormat("Back up '{0}'{2}\tto '{1}' before migration{2}",
                        MigrationOptions.ConfigurationLocation,
                        Path.Combine(Path.GetDirectoryName(MigrationOptions.ConfigurationLocation),
                            "ccnet.config.bak"),
                            Environment.NewLine);
                }
                builder.AppendLine();
            }
            if (MigrationOptions.MigrateWebDashboard)
            {
                builder.AppendFormat("Migrate web dashboard files from '{0}'{2}\tto '{1}'{2}",
                    MigrationOptions.CurrentWebDashboardLocation,
                    MigrationOptions.NewWebDashboardLocation,
                    Environment.NewLine);
                builder.AppendLine();
            }
            var message = builder.ToString();
            if (string.IsNullOrEmpty(message))
            {
                message = "No migration options selected!";
                IsValid = false;
            }
            else
            {
                IsValid = true;
            }
            confirmationBox.Text = message;
        }
    }
}
