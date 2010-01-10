using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Presentation
{
    public partial class ConfigureServer : Form
    {
        private BuildServer server;
        private IAuthenticationMode authMode;
        private string lastMode;
        private string settings;

        public ConfigureServer(BuildServer server)
        {
            InitializeComponent();
            this.server = server;
            Text = "Configure server: " + server.DisplayName; 
            LoadModes();
            if (server.SecurityType != null)
            {
                chkIsSecure.Checked = true;
                for (int loop = 0; loop < cmbAuthMode.Items.Count; loop++)
                {
                    ExtensionDetails details = (ExtensionDetails)cmbAuthMode.Items[loop];
                    if (details.TypeName == server.SecurityType) cmbAuthMode.SelectedIndex = loop;
                }
            }
        }

        public static bool Configure(IWin32Window parent, BuildServer server)
        {
            ConfigureServer configureDialog = new ConfigureServer(server);
            bool hasChanged = (configureDialog.ShowDialog(parent) == DialogResult.OK);
            return hasChanged;
        }

        private void chkIsSecure_CheckedChanged(object sender, EventArgs e)
        {
            lblAuthMode.Enabled = chkIsSecure.Checked;
            cmbAuthMode.Enabled = chkIsSecure.Checked;
            butConfigureAuth.Enabled = chkIsSecure.Checked;
        }

        private void LoadModes()
        {
            cmbAuthMode.Items.Clear();
            string appPath = Environment.CurrentDirectory;
            AddExtensions(ExtensionHelpers.QueryAssembliesForTypes(appPath, "IAuthenticationMode"));
            string extensionsPath = Path.Combine(Environment.CurrentDirectory, "Extensions");
            AddExtensions(ExtensionHelpers.QueryAssembliesForTypes(extensionsPath, "IAuthenticationMode"));
        }

        private void AddExtensions(string[] extensions)
        {
            foreach (string extensionName in extensions)
            {
                string displayName = ExtensionHelpers.CheckForDisplayName(extensionName);
                cmbAuthMode.Items.Add(new ExtensionDetails(displayName, extensionName));
            }
        }

        private void butConfigureAuth_Click(object sender, EventArgs e)
        {
            try
            {
                InitialiseAuthMode();
                if (authMode.Configure(this))
                {
                    settings = authMode.Settings;
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(this,
                    "Unable to configure authorisation mode:" + Environment.NewLine + error.Message,
                    "Extension Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void InitialiseAuthMode()
        {
            if (chkIsSecure.Checked && (lastMode != cmbAuthMode.Text))
            {
                ExtensionDetails details = (ExtensionDetails)cmbAuthMode.Items[cmbAuthMode.SelectedIndex];
                authMode = ExtensionHelpers.RetrieveAuthenticationMode(details.TypeName);
                authMode.Settings = settings;
                lastMode = cmbAuthMode.Text;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (chkIsSecure.Checked)
                {
                    InitialiseAuthMode();
                    if (authMode.Validate())
                    {
                        ExtensionDetails details = (ExtensionDetails)cmbAuthMode.Items[cmbAuthMode.SelectedIndex];
                        server.SecurityType = details.TypeName;
                        server.SecuritySettings = authMode.Settings;

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        throw new Exception("Unable to validate authorisation");
                    }
                }
                else
                {
                    server.SecurityType = null;
                    server.SecuritySettings = null;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(this,
                    "Unable to validate authorisation mode:" + Environment.NewLine + error.Message,
                    "Extension Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private struct ExtensionDetails
        {
            public string DisplayName;
            public string TypeName;

            public ExtensionDetails(string display, string type)
            {
                DisplayName = display;
                TypeName = type;
            }

            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
}
