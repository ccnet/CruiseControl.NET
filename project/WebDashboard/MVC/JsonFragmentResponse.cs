using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
    public class JsonFragmentResponse : IResponse
    {
        private readonly string jsonFragment;
        private ConditionalGetFingerprint serverFingerprint;

        public JsonFragmentResponse(string jsonFragment)
        {
            this.jsonFragment = jsonFragment;
        }

        public string ResponseFragment
        {
            get { return jsonFragment; }
        }

        public void Process(HttpResponse response)
        {
            response.AppendHeader("Last-Modified", serverFingerprint.LastModifiedTime.ToString("r"));
            response.AppendHeader("ETag", serverFingerprint.ETag);
            response.AppendHeader("Cache-Control", "private, max-age=0");
            response.ContentType = MimeType.Json.ContentType;
            response.Write(jsonFragment);
        }

        public ConditionalGetFingerprint ServerFingerprint
        {
            get { return serverFingerprint; }
            set { serverFingerprint = value; }
        }
    }
}