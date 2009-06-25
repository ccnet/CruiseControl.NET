using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class VersionSelectionPage : WizardPageBase
    {
        public VersionSelectionPage()
        {
            InitializeComponent();
            currentVersion.Items.AddRange(MigrationOptions.AllowedVersions);
            LinkNextPage(new ServerOptionsPage());
        }

        public override void RunPage()
        {
            currentVersion.SelectedItem = MigrationOptions.CurrentVersion;
        }

        public override void CompletePage()
        {
            MigrationOptions.CurrentVersion = currentVersion.SelectedText;
        }
    }
}
