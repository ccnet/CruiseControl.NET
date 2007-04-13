using System.Web;

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
	}
}