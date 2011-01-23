using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class ProgressPage : WizardPageBase
    {
        public ProgressPage()
        {
            InitializeComponent();
        }

        public override void RunPage()
        {
            dataMigrationWorker.RunWorkerAsync();
        }

        private void dataMigrationWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var engine = Controller.MigrationEngine;
            engine.Message += engine_Message;
            engine.Run();
            engine.Message -= engine_Message;
        }

        private void engine_Message(object sender, MigrationEventArgs e)
        {
            if (InvokeRequired)
            {
                progressDisplay.Invoke(new EventHandler<MigrationEventArgs>(engine_Message),
                    sender,
                    e);
            }
            else
            {
                progressDisplay.Items.Insert(0, e.Message);
            }
        }

        private void dataMigrationWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            NextPage = new CompletedPage();
            FirePageCompeleted();
        }
    }
}
