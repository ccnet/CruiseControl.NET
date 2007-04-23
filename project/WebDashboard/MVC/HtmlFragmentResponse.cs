using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class HtmlFragmentResponse : IResponse
	{
		private readonly string htmlFragment;
	    private ConditionalGetFingerprint serverFingerprint;

	    public HtmlFragmentResponse(string htmlFragment)
		{
			this.htmlFragment = htmlFragment;
		}

		public string ResponseFragment
		{
			get { return htmlFragment; }
		}

		public void Process(HttpResponse response)
		{
//            response.Cache.SetLastModified(serverFingerprint.LastModifiedTime);
//            response.Cache.SetETag("\"" + serverFingerprint.ETag + "\"");
//            response.Cache.SetMaxAge(TimeSpan.Zero);
//            response.CacheControl = "private, max-age=0";
            response.AppendHeader("Last-Modified", serverFingerprint.LastModifiedTime.ToString("r"));
            response.AppendHeader("ETag", serverFingerprint.ETag);
            response.AppendHeader("Cache-Control", "private, max-age=0");
            response.Write(htmlFragment);
		}

	    public ConditionalGetFingerprint ServerFingerprint
	    {
            get { return serverFingerprint; }
	        set { serverFingerprint = value; }
	    }
	}
}