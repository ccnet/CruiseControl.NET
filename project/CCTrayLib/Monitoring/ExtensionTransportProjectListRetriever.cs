using System;
using System.Collections.Generic;
using System.Text;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    public class ExtensionTransportProjectListRetriever
    {
        private readonly ITransportExtension extensionInstance;

        public ExtensionTransportProjectListRetriever(string extensionName)
        {
            this.extensionInstance = ExtensionHelpers.RetrieveExtension(extensionName);
        }

        public CCTrayProject[] GetProjectList(BuildServer server)
        {
            extensionInstance.Settings = server.ExtensionSettings;
            extensionInstance.Configuration = server;
            return extensionInstance.GetProjectList(server);
        }
    }
}
