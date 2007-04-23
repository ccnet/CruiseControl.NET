using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
    public class NotModifiedResponse : IResponse
    {
        private ConditionalGetFingerprint serverFingerprint;

        public NotModifiedResponse(ConditionalGetFingerprint serverFingerprint)
        {
            this.serverFingerprint = serverFingerprint;
        }

        public void Process(HttpResponse response)
        {
            response.StatusCode = 304;
            response.AppendHeader("ETag", serverFingerprint.ETag);
            response.AppendHeader("Cache-Control", "private, max-age=0");
        }

        public ConditionalGetFingerprint ServerFingerprint
        {
            get { return serverFingerprint; }
            set { serverFingerprint = value; }
        }
    }
}