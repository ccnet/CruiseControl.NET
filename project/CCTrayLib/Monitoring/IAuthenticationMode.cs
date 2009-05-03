using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.Remote;
using ThoughtWorks.CruiseControl.Remote.Security;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    public interface IAuthenticationMode
    {
        string Settings { get; set; }
        bool Configure(IWin32Window owner);
        bool Validate();
        ISecurityCredentials GenerateCredentials();
    }
}
