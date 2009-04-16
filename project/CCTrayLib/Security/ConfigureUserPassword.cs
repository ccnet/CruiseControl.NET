using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Security
{
    public partial class ConfigureUserPassword : Form
    {
        public ConfigureUserPassword(string userName)
        {
            InitializeComponent();
            tUsername.Text = userName;
        }

        public string UserName
        {
            get { return tUsername.Text; }
        }

        public string Password
        {
            get { return tPassword1.Text; }
        }

        public event EventHandler UserPasswordSet;

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (UserPasswordSet != null) UserPasswordSet(this, EventArgs.Empty);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = !string.IsNullOrEmpty(tUsername.Text) &&
                !string.IsNullOrEmpty(tPassword1.Text) &&
                string.Equals(tPassword1.Text, tPassword2.Text);
        }
    }
}
