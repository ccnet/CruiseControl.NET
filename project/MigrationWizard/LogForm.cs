using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.MigrationWizard
{
    public partial class LogForm : Form
    {
        public LogForm()
        {
            InitializeComponent();
        }

        public void LoadMessages(IEnumerable<MigrationEventArgs> messages)
        {
            foreach (var msg in messages)
            {
                var item = new ListViewItem(new string[] {
                    msg.Type.ToString(),
                    msg.Time.ToLongTimeString(),
                    msg.Message
                });
                messageList.Items.Add(item);
            }
        }

        private void messageList_DoubleClick(object sender, EventArgs e)
        {
            if (messageList.SelectedItems.Count > 0)
            {
                MessageBox.Show(this,
                    messageList.SelectedItems[0].SubItems[2].Text,
                    "Event Message",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}
