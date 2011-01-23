using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class RedirectResponse : IResponse
	{
		private readonly string redirectUrl;

		public RedirectResponse(string redirectURL)
		{
			redirectUrl = redirectURL;
		}

		public string Url
		{
			get { return redirectUrl; }
		}

		public void Process(HttpResponse response)
		{
			response.Redirect(redirectUrl);
		}

	    public ConditionalGetFingerprint ServerFingerprint
	    {
            get { return ConditionalGetFingerprint.NOT_AVAILABLE; }
	        set { /* ignore attempts to fingerprint redirects */ }
	    }
	}
}