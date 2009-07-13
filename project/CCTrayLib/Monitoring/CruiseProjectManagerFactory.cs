using System.Collections.Generic;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    public class CruiseProjectManagerFactory : ICruiseProjectManagerFactory
    {
        private ICruiseServerClientFactory clientFactory;

        public CruiseProjectManagerFactory(ICruiseServerClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public ICruiseProjectManager Create(CCTrayProject project, IDictionary<BuildServer, ICruiseServerManager> serverManagers)
        {
            BuildServer server = project.BuildServer;
            switch (server.Transport)
            {
                case BuildServerTransport.Remoting:
                    {
                        var client = GenerateRemotingClient(server);
                        return new RemotingCruiseProjectManager(client, project.ProjectName);
                    }
                case BuildServerTransport.Extension:
                    ITransportExtension extensionInstance = ExtensionHelpers.RetrieveExtension(server.ExtensionName);
                    extensionInstance.Settings = server.ExtensionSettings;
                    extensionInstance.Configuration = server;
                    return extensionInstance.RetrieveProjectManager(project.ProjectName);
                default:
                    {
                        var client = GenerateHttpClient(server);
                        return new HttpCruiseProjectManager(client, project.ProjectName, serverManagers[server]);
                    }
            }
        }

        public CCTrayProject[] GetProjectList(BuildServer server)
        {
            switch (server.Transport)
            {
                case BuildServerTransport.Remoting:
                    {
                        var client = GenerateRemotingClient(server);
                        return new RemotingProjectListRetriever(client).GetProjectList(server);
                    }
                case BuildServerTransport.Extension:
                    return new ExtensionTransportProjectListRetriever(server.ExtensionName).GetProjectList(server);
                default:
                    {
                        var client = GenerateHttpClient(server);
                        return new HttpProjectListRetriever(client).GetProjectList(server);
                    }
            }
        }

        private CruiseServerClientBase GenerateRemotingClient(BuildServer server)
        {
            var client = clientFactory.GenerateRemotingClient(server.Url,
                ClientStartUpSettingsExtensions.GenerateStartupSettings(server));
            return client;
        }

        private CruiseServerClientBase GenerateHttpClient(BuildServer server)
        {
            var client = clientFactory.GenerateHttpClient(server.Url,
                ClientStartUpSettingsExtensions.GenerateStartupSettings(server));
            return client;
        }
    }
}