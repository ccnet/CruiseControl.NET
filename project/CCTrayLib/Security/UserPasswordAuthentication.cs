using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Security
{
    [Extension(DisplayName = "User name/password authentication")]
    public class UserPasswordAuthentication
        : IAuthenticationMode
    {
        private string settings;

        public string Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public bool Configure(IWin32Window owner)
        {
            string[] splitSettings = SplitSettings();
            ConfigureUserPassword newDialog = new ConfigureUserPassword(splitSettings[0]);
            newDialog.UserPasswordSet += delegate(object sender, EventArgs e) { settings = newDialog.UserName + "\r" + newDialog.Password; };
            DialogResult result = newDialog.ShowDialog(owner);
            return (result == DialogResult.OK);
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(settings);
        }

        public ISecurityCredentials GenerateCredentials()
        {
            string[] settings = SplitSettings();
            UserNameCredentials credentials = new UserNameCredentials(settings[0]);
            credentials["password"] = settings[1];
            return credentials;
        }

        private string[] SplitSettings()
        {
            if (string.IsNullOrEmpty(settings))
            {
                return new string[2] { string.Empty, string.Empty };
            }
            else
            {
                if (settings.Contains("\r"))
                {
                    string[] splitSettings = settings.Split('\r');
                    return splitSettings;
                }
                else
                {
                    return new string[2] { settings, string.Empty };
                }
            }
        }
    }
}
