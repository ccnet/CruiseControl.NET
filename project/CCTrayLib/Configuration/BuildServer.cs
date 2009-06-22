using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class BuildServer
	{
		private const int DefaultRemotingPort = 21234;

		private string url;
        private BuildServerTransport transportMode = BuildServerTransport.HTTP;
        private string extensionName;
        private string settings;
        private string securityType;
        private string securitySettings;

		public BuildServer()
		{
		}
			
        public BuildServer(string url)
        {
            this.url = url;
            Uri serverUri = new Uri(Url);
            this.transportMode = (serverUri.Scheme.ToLower() == "tcp") ? BuildServerTransport.Remoting : BuildServerTransport.HTTP;
		}
		
        /// <summary>
        /// Start a new build server using the full options.
        /// </summary>
        /// <param name="url">The URI to the remote server.</param>
        /// <param name="transportMode">The transport mode to use.</param>
        /// <param name="extensionName">The name of the extension to use.</param>
        /// <param name="settings">The settings for the extension.</param>
        public BuildServer(string url, BuildServerTransport transportMode, string extensionName, string settings)
        {
            if ((transportMode == BuildServerTransport.Extension) && string.IsNullOrEmpty(extensionName))
            {
                throw new CCTrayLibException("Extension transport must always define an extension name");
            }
            this.url = url;
            this.transportMode = transportMode;
            this.extensionName = extensionName;
            this.settings = settings;
        }

		public string Url
		{
			get { return url; }
		}
		
		public Uri Uri
		{
			get { return new Uri(url); }
		}

        public string ExtensionName
        {
            get { return this.extensionName; }
            set { this.extensionName = value; }
        }

        public string ExtensionSettings
        {
            get { return this.settings; }
            set { this.settings = value; }
        }

        public string SecurityType
        {
            get { return this.securityType; }
            set { this.securityType = value; }
        }

        public string SecuritySettings
        {
            get { return this.securitySettings; }
            set { this.securitySettings = value; }
        }

		public string DisplayName
		{
			get 
			{
				if (Transport == BuildServerTransport.Remoting)
				{
					Uri serverUri = Uri;
					if (serverUri.Port == DefaultRemotingPort)
						return serverUri.Host;

					return serverUri.Host + ":" + serverUri.Port;
				}
				
				return Url;
			}
		}

		public BuildServerTransport Transport
		{
            get { return this.transportMode; }
            set { this.transportMode = value; }
		}

		public static BuildServer BuildFromRemotingDisplayName(string displayName)
		{
			string url;
			
			string[] displayNameParts = displayName.Split(':');

			if (displayNameParts.Length == 1)
			{
				url = string.Format("tcp://{0}:{1}/CruiseManager.rem", displayNameParts[0], DefaultRemotingPort);
			}
			else if (displayNameParts.Length == 2)
			{
				try
				{
					url =
						string.Format("tcp://{0}:{1}/CruiseManager.rem", displayNameParts[0], Convert.ToInt32(displayNameParts[1]));
				}
				catch (FormatException)
				{
					throw new ApplicationException("Port number must be an integer");
				}
			}
			else
			{
				throw new ApplicationException("Expected string in format server[:port]");
			}
			return new BuildServer(url, BuildServerTransport.Remoting, null, null);
		}
		
		public override bool Equals(object obj)
		{
			BuildServer other = obj as BuildServer;
			if (other == null)
				return false;
			
			return other.url.Equals (url);
		}

		public override int GetHashCode()
		{
			return url.GetHashCode ();
		}

		public override string ToString()
		{
			return DisplayName;
		}
	}
}
