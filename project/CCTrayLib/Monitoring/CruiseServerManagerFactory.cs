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
