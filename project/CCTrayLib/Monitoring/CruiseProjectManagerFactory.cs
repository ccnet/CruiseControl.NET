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
                        return new HttpCruiseProjectManager(client, project.ProjectName);
                    }
            }
        }

        public CCTrayProject[] GetProjectList(BuildServer server, bool newServer)
        {
            if (newServer)
            {
                // Clear the cache when adding a new server
                clientFactory.ResetCache(server.Url);
            }

            ICruiseServerManager manager;
            switch (server.Transport)
            {
                case BuildServerTransport.Remoting:
                    {
                        var client = GenerateRemotingClient(server);
                        manager = new RemotingCruiseServerManager(client, server);
                        break;
                    }

                case BuildServerTransport.Extension:
                    return new ExtensionTransportProjectListRetriever(server.ExtensionName).GetProjectList(server);
                default:
                    {
                        var client = GenerateHttpClient(server);
                        manager = new HttpCruiseServerManager(client, server);
                        break;
                    }
            }

            if (!string.IsNullOrEmpty(server.SecurityType))
            {
                manager.Login();
            }

            return manager.GetProjectList();
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