
namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class RedirectResponse : IResponse
	{
		private readonly string redirectUrl;

		public RedirectResponse(string redirectURL)
		{
			redirectUrl = redirectURL;
		}

		public string ResponseFragment
		{
			get { return redirectUrl; }
		}
	}
}
