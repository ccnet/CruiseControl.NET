using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class CompletedPage : WizardPageBase
    {
        public CompletedPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load all the relevant messages.
        /// </summary>
        public override void RunPage()
        {
            var builder = new StringBuilder();
            StringBuilder currentBuilder = null;
            
            // Start at the back and work forwards
            for (var loop = Controller.MigrationEvents.Count - 1; loop >= 0; loop--)
            {
                var msg = Controller.MigrationEvents[loop];
                switch (msg.Type)
                {
                    case MigrationEventType.Status:
                        if (currentBuilder != null)
                        {
                            if (builder.Length > 0) currentBuilder.AppendLine();
                            builder.Insert(0, currentBuilder.ToString());
                        }

                        currentBuilder = new StringBuilder();
                        currentBuilder.Append(msg.Message);
                        currentBuilder.AppendLine();
                        break;
                    case MigrationEventType.Warning:
                        currentBuilder.AppendFormat("\tWarning: {0}", msg.Message);
                        currentBuilder.AppendLine();
                        break;
                    case MigrationEventType.Error:
                        currentBuilder.AppendFormat("\tERROR:   {0}", msg.Message);
                        currentBuilder.AppendLine();
                        break;
                }
            }

            if (currentBuilder != null)
            {
                if (builder.Length > 0) currentBuilder.AppendLine();
                builder.Insert(0, currentBuilder.ToString());
            }

            // Display the results
            finishedBox.Text = builder.ToString();
        }

        private void viewLogButton_Click(object sender, EventArgs e)
        {
            Controller.ViewLog(FindForm());
        }
    }
}
