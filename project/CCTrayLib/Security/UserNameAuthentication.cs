using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Messages;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Security
{
    [Extension(DisplayName="User name authentication")]
    public class UserNameAuthentication
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
            ConfigureUserName newDialog = new ConfigureUserName(settings);
            newDialog.UserNameSet += delegate(object sender, EventArgs e) { settings = newDialog.UserName; };
            DialogResult result = newDialog.ShowDialog(owner);
            return (result == DialogResult.OK);
        }

        public bool Validate()
        {
            return !string.IsNullOrEmpty(settings);
        }

        public LoginRequest GenerateCredentials()
        {
            LoginRequest credentials = new LoginRequest(settings);
            return credentials;
        }
    }
}
