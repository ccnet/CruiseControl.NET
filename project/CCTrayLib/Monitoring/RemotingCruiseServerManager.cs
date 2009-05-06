using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;
using System;
using ThoughtWorks.CruiseControl.Remote.Messages;
using ThoughtWorks.CruiseControl.Core;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Allows access to the state of a single cruise control server, over remoting
	/// </summary>
	public class RemotingCruiseServerManager : ICruiseServerManager
	{
        private readonly ICruiseServerClient manager;
		private readonly BuildServer configuration;
		private readonly string displayName;
		private string sessionToken;

		public RemotingCruiseServerManager(ICruiseServerClient manager, BuildServer buildServer)
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
            ProjectRequest request = new ProjectRequest();
            request.SessionToken = sessionToken;
            request.ProjectName = projectName;
            ValidateResponse(manager.CancelPendingRequest(request));
		}

		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
            SnapshotResponse response = manager.GetCruiseServerSnapshot(
                new ServerRequest(sessionToken));
            ValidateResponse(response);
            return response.Snapshot;
		}

        public bool Login()
        {
            if (configuration.SecurityType != null)
            {
                if (sessionToken != null) Logout();
                IAuthenticationMode authentication = ExtensionHelpers.RetrieveAuthenticationMode(configuration.SecurityType);
                authentication.Settings = configuration.SecuritySettings;
                sessionToken = manager.Login(authentication.GenerateCredentials()).SessionToken;
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
                manager.Logout(new ServerRequest(sessionToken));
                sessionToken = null;
            }
        }

        /// <summary>
        /// Validates that the request processed ok.
        /// </summary>
        /// <param name="value">The response to check.</param>
        private void ValidateResponse(Response value)
        {
            if (value.Result == ResponseResult.Failure)
            {
                string message = "Request request has failed on the remote server:" + Environment.NewLine +
                    value.ConcatenateErrors();
                throw new CruiseControlException(message);
            }
        }
	}
}
