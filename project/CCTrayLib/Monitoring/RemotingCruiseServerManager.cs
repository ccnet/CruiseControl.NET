using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control server, over remoting
	/// </summary>
	public class RemotingCruiseServerManager : ICruiseServerManager
	{
		private readonly ICruiseManager manager;
		private readonly BuildServer configuration;
		private readonly string displayName;
		private string sessionToken;

		public RemotingCruiseServerManager(ICruiseManager manager, BuildServer buildServer)
		{
			this.manager = manager;
			this.displayName = buildServer.DisplayName;
            this.configuration = buildServer;
		}

        public BuildServer Configuration
        {
            get { return configuration; }
        }

        public string SessionToken
        {
            get { return sessionToken; }
        }

		public string DisplayName
		{
			get { return displayName; }
		}

		public void CancelPendingRequest(string projectName)
		{
			manager.CancelPendingRequest(projectName);
		}

		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			return manager.GetCruiseServerSnapshot();
		}

        public bool Login()
        {
            if (configuration.SecurityType != null)
            {
                if (sessionToken != null) Logout();
                IAuthenticationMode authentication = ExtensionHelpers.RetrieveAuthenticationMode(configuration.SecurityType);
                authentication.Settings = configuration.SecuritySettings;
                sessionToken = manager.Login(authentication.GenerateCredentials());
                return (sessionToken != null);
            }
            else
            {
                return true;
            }
        }

        public void Logout()
        {
            if (sessionToken != null)
            {
                manager.Logout(sessionToken);
                sessionToken = null;
            }
        }
	}
}
