using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            this.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture,"About {0}", AssemblyTitle);
            this.labelVersion.Text = string.Format(System.Globalization.CultureInfo.CurrentCulture,"Version {0}", AssemblyVersion);
            this.labelCopyright.Text += Environment.NewLine + Environment.NewLine + AssemblyCopyright;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != string.Empty)
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return string.Empty;
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }
        #endregion

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process urlLink = new Process();
            urlLink.StartInfo = new ProcessStartInfo("http://www.cruisecontrolnet.org/projects/ccnet");
            urlLink.Start();
        }

        private void famfamfamLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process urlLink = new Process();
            urlLink.StartInfo = new ProcessStartInfo("http://www.famfamfam.com");
            urlLink.Start();
        }
    }
}
