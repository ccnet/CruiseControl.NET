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
                    {
                        var manager = GenerateRemotingClient(buildServer);
                        return new CachingCruiseServerManager(new RemotingCruiseServerManager(manager, buildServer));
                    }
                case BuildServerTransport.Extension:
                    ITransportExtension extensionInstance = ExtensionHelpers.RetrieveExtension(buildServer.ExtensionName);
                    extensionInstance.Settings = buildServer.ExtensionSettings;
                    extensionInstance.Configuration = buildServer;
                    return extensionInstance.RetrieveServerManager();
                default:
                    {
                        var manager = GenerateHttpClient(buildServer);
                        return new CachingCruiseServerManager(
                            new HttpCruiseServerManager(manager, buildServer));
                    }
            }
        }

        private CruiseServerClientBase GenerateRemotingClient(BuildServer server)
        {
            var settings = new ClientStartUpSettings
            {
                BackwardsCompatable = (server.ExtensionSettings == "OLD")
            };
            var client = clientFactory.GenerateRemotingClient(server.Url, settings);
            return client;
        }

        private CruiseServerClientBase GenerateHttpClient(BuildServer server)
        {
            var settings = new ClientStartUpSettings
            {
                BackwardsCompatable = (server.ExtensionSettings == "OLD")
            };
            var client = clientFactory.GenerateHttpClient(server.Url, settings);
            return client;
        }
    }
}
