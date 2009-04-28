using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    public interface ITransportExtension
    {
        string DisplayName { get; }
        string Settings { get; set; }
        BuildServer Configuration { get; set; }
        CCTrayProject[] GetProjectList(BuildServer server);
        ICruiseProjectManager RetrieveProjectManager(string projectName);
        ICruiseServerManager RetrieveServerManager();
        bool Configure(IWin32Window owner);
    }
}
