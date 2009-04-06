using System;
using System.Reflection;
using System.Windows.Forms;
using System.Diagnostics;

namespace Validator
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            versionLabel.Text = String.Format("Version {0}", AssemblyVersion);
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        private void officalLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://ccnet.thoughtworks.com/");
        }

        private void blogLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://csut017.wordpress.com/");
        }
    }
}
