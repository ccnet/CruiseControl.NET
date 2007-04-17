
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

        public WebDashboardUrl(string serverUrl)
        {
            this.serverUrl = serverUrl.TrimEnd('/');
        }

        public string XmlServerReport
        {
            get { return serverUrl + "/XmlServerReport.aspx"; }
        }
    }
}
