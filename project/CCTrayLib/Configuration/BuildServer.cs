using System;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class BuildServer
	{
		private const int DefaultRemotingPort = 21234;

		private string url;

		public BuildServer()
		{
			
		}
		
		public BuildServer(string url)
		{
			this.url = url;
		}

		public string Url
		{
			get { return url; }
		}
		
		public Uri Uri
		{
			get { return new Uri(url); }
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
			get 
			{
				Uri serverUri = new Uri(Url);
				return serverUri.Scheme.ToLower() == "tcp" ? BuildServerTransport.Remoting : BuildServerTransport.HTTP;
			}
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
			return new BuildServer(url);
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
