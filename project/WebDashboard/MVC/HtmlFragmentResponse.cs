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
			if (IsClientSideCacheable())
			{
				AddHeadersToEnable403Cacheing(response);
			}
            response.ContentType = MimeType.Html.ContentType;
            response.Write(htmlFragment);
		}

		private void AddHeadersToEnable403Cacheing(HttpResponse response)
		{
            var browser = HttpContext.Current.Request.Browser;
            if (browser.IsBrowser("IE") && (browser.MajorVersion <= 6))
            {
                // Turn off caching due to issues with IE 6.0
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.Cache.SetNoStore();
            }
            else
            {
                response.AppendHeader("Last-Modified", serverFingerprint.LastModifiedTime.ToString("r"));
                response.AppendHeader("ETag", serverFingerprint.ETag);
                response.AppendHeader("Cache-Control", "no-store");
            }
		}

		private bool IsClientSideCacheable()
		{
			return !(ConditionalGetFingerprint.NOT_AVAILABLE.Equals(ServerFingerprint));
		}

		public ConditionalGetFingerprint ServerFingerprint
	    {
            get { return serverFingerprint; }
	        set { serverFingerprint = value; }
	    }
	}
}