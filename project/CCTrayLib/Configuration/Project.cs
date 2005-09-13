using System;
using System.Xml.Serialization;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Configuration
{
	public class Project
	{
		private const int DefaultPort = 21234;

		public Project()
		{
		}

		public Project(string serverUrl, string projectName)
		{
			ServerUrl = serverUrl;
			ProjectName = projectName;
		}

		[XmlAttribute(AttributeName="serverUrl")]
		public string ServerUrl;

		[XmlAttribute(AttributeName="projectName")]
		public string ProjectName;

		[XmlIgnore]
		public string ServerDisplayName
		{
			get
			{
				Uri serverUri = new Uri(ServerUrl);
				if (serverUri.Port == DefaultPort)
					return serverUri.Host;

				return serverUri.Host + ":" + serverUri.Port;
			}
		}

		public void SetServerUrlFromDisplayName(string displayName)
		{
			string[] displayNameParts = displayName.Split(':');

			if (displayNameParts.Length == 1)
			{
				ServerUrl = string.Format("tcp://{0}:{1}/CruiseManager.rem", displayNameParts[0], DefaultPort);
			}
			else if (displayNameParts.Length == 2)
			{
				try
				{
					ServerUrl =
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
		}
	}
}