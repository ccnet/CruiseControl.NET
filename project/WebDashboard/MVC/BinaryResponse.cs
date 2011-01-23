using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
    public class BinaryResponse : IResponse
    {
        private byte[] content;
        private ConditionalGetFingerprint serverFingerprint;

        public BinaryResponse(byte[] content)
        {
            this.content = content;
        }

        public void Process(HttpResponse response)
        {
            response.AppendHeader("Last-Modified", serverFingerprint.LastModifiedTime.ToString("r"));
            response.AppendHeader("ETag", serverFingerprint.ETag);
            response.AppendHeader("Cache-Control", "private, max-age=0");
            response.BinaryWrite(content);
        }

        public ConditionalGetFingerprint ServerFingerprint
        {
            get { return serverFingerprint; }
            set { serverFingerprint = value; }
        }
    }
}