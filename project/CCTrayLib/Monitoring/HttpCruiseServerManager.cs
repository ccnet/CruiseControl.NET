using System;
using ThoughtWorks.CruiseControl.CCTrayLib.Configuration;
using ThoughtWorks.CruiseControl.Remote;
using System.Xml;
using System.Collections.Specialized;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class HttpCruiseServerManager : ICruiseServerManager
	{
		private readonly IWebRetriever webRetriever;
		private readonly IDashboardXmlParser dashboardXmlParser;
		private readonly BuildServer configuration;
		private readonly string displayName;
        private string sessionToken;

		public HttpCruiseServerManager(IWebRetriever webRetriever, IDashboardXmlParser dashboardXmlParser, 
			BuildServer buildServer)
		{
			this.webRetriever = webRetriever;
			this.dashboardXmlParser = dashboardXmlParser;
			this.configuration = buildServer;
			displayName = GetDisplayNameFromUri(buildServer.Uri);
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
			throw new NotImplementedException("Cancel pending not currently supported on servers monitored via HTTP");
		}

		/// <summary>
		/// Gets the projects and integration queues snapshot from this server.
		/// </summary>
        public CruiseServerSnapshot GetCruiseServerSnapshot()
		{
			string xml = webRetriever.Get(configuration.Uri);
			return dashboardXmlParser.ExtractAsCruiseServerSnapshot(xml);
		}

		private static string GetDisplayNameFromUri(Uri uri)
		{
			const int DefaultHttpPort = 80;
			// TODO: The BuildServer.DisplayName property is coded such that it only strips out the server
			// name if it is a remoting connection, not an http one.
			// Rather than changing this behaviour in case it is seen as "desirable" the functionality is
			// implemented within here so that the server queues treeview can just display the server name.
			if (uri.Port == DefaultHttpPort)
				return uri.Host;

			return uri.Host + ":" + uri.Port;
		}

        public bool Login()
        {
            if (configuration.SecurityType != null)
            {
                if (sessionToken != null) Logout();
                IAuthenticationMode authentication = ExtensionHelpers.RetrieveAuthenticationMode(configuration.SecurityType);
                authentication.Settings = configuration.SecuritySettings;

                XmlElement result = SendSecurityAction("login", new NameValuePair("credentials", authentication.GenerateCredentials().Serialise()));
                if (string.Equals(result.GetAttribute("result"), "success", StringComparison.InvariantCultureIgnoreCase))
                {
                    sessionToken = result.SelectSingleNode("session").InnerText;
                }

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
                SendSecurityAction("logout", new NameValuePair("sessionToken", sessionToken));
                sessionToken = null;
            }
        }

        private XmlElement SendSecurityAction(string action, params NameValuePair[] values)
        {
            Uri securityUri = InitConnection();
            NameValueCollection input = new NameValueCollection();
            input.Add("action", action);
            input.Add("server", "local");
            foreach (NameValuePair namedValue in values)
            {
                input.Add(namedValue.Name, namedValue.Value);
            }
            string result = webRetriever.Post(securityUri, input);

            XmlDocument resultXml = new XmlDocument();
            resultXml.LoadXml(result);
            return resultXml.DocumentElement;
        }

        private Uri InitConnection()
        {
            string serverAlias = "local";
            return new Uri(new WebDashboardUrl(configuration.Url, serverAlias).Security);
        }

        private struct NameValuePair
        {
            public string Name;
            public string Value;

            public NameValuePair(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }
	}
}
