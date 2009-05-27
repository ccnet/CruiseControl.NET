using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    public class CruiseServerManagerFactory : ICruiseServerManagerFactory
    {
        private ICruiseServerClientFactory clientFactory;

        public CruiseServerManagerFactory(ICruiseServerClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
        }

        public ICruiseServerManager Create(BuildServer buildServer)
        {
            switch (buildServer.Transport)
            {
                case BuildServerTransport.Remoting:
                    var manager = GenerateRemotingClient(buildServer);
                    return new CachingCruiseServerManager(new RemotingCruiseServerManager(manager, buildServer));
                case BuildServerTransport.Extension:
                    ITransportExtension extensionInstance = ExtensionHelpers.RetrieveExtension(buildServer.ExtensionName);
                    extensionInstance.Settings = buildServer.ExtensionSettings;
                    extensionInstance.Configuration = buildServer;
                    return extensionInstance.RetrieveServerManager();
                default:
                    return new CachingCruiseServerManager(
                        new HttpCruiseServerManager(new WebRetriever(), new DashboardXmlParser(), buildServer));
            }
        }

        private CruiseServerClientBase GenerateRemotingClient(BuildServer server)
        {
            CruiseServerClientBase client;
            switch (server.ExtensionSettings)
            {
                case "OLD":
                    var settings = new ClientStartUpSettings
                    {
                        BackwardsCompatable = true
                    };
                    client = clientFactory.GenerateRemotingClient(server.Url, settings);
                    break;
                default:
                    client = clientFactory.GenerateRemotingClient(server.Url);
                    break;
            }
            return client;
        }
    }
}
