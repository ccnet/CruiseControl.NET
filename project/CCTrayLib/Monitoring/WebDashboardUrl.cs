
namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
    /// <summary>
    /// Encapsulate the web page suffix added to make a url for communicating with a CC.Net server.
    /// </summary>
    /// <remarks>
    /// For servers prior to CC.Net 1.3 the URL was XmlStatusReport.aspx. As this class is only used in newer
    /// versions of CCTray this means thay can only communicate with 1.3 or later CC.Net instances.
    /// </remarks>
    public class WebDashboardUrl
    {
        private string serverUrl;
        private string serverAlias = "local";

        public WebDashboardUrl(string serverUrl)
        {
            this.serverUrl = serverUrl.TrimEnd('/');
        }

        public WebDashboardUrl(string serverUrl, string serverAlias) : this(serverUrl)
        {
            this.serverAlias = serverAlias.Trim('/');
        }

        public string XmlServerReport
        {
            get { return string.Format("{0}/XmlServerReport.aspx", serverUrl); }
        }

        public string ViewFarmReport
        {
            get { return string.Format("{0}/server/{1}/ViewFarmReport.aspx", serverUrl, serverAlias); }
        }

        public string Security
        {
            get { return string.Format("{0}/server/{1}/XmlSecurity.aspx", serverUrl, serverAlias); }
        }
    }
}
