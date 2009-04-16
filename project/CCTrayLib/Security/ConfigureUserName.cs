using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Security
{
    public partial class ConfigureUserName : Form
    {
        public ConfigureUserName(string userName)
        {
            InitializeComponent();
            tUsername.Text = userName;
        }

        public string UserName
        {
            get { return tUsername.Text; }
        }

        public event EventHandler UserNameSet;

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (UserNameSet != null) UserNameSet(this, EventArgs.Empty);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void tUsername_TextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !string.IsNullOrEmpty(tUsername.Text);
        }
    }
}
