using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class IntroductionPage : WizardPageBase
    {
        public IntroductionPage()
        {
            InitializeComponent();

            // Set up the next page in the sequence
            LinkNextPage(new VersionSelectionPage());
        }
    }
}
