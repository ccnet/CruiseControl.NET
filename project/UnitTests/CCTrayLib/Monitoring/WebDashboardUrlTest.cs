using NUnit.Framework;
using ThoughtWorks.CruiseControl.CCTrayLib.Monitoring;

namespace ThoughtWorks.CruiseControl.UnitTests.CCTrayLib.Monitoring
{
    [TestFixture]
    public class WebDashboardUrlTest
    {
        [Test]
        public void ReturnsCorrectXmlServerReportUrl()
        {
            const string SERVER_URL = @"http://localhost/ccnet";
            WebDashboardUrl webDashboardUrl = new WebDashboardUrl(SERVER_URL);
            Assert.AreEqual(SERVER_URL + "/XmlServerReport.aspx", webDashboardUrl.XmlServerReport);

            // Try again with an extra trailing slash.
            webDashboardUrl = new WebDashboardUrl(SERVER_URL + @"/");
            Assert.AreEqual(SERVER_URL + "/XmlServerReport.aspx", webDashboardUrl.XmlServerReport);
        }
    }
}
