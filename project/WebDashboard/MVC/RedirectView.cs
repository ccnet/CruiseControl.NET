
namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	// After 0.9, update API to use 'IResponse' to clean up this nastiness (See HttpHandler)
	public class RedirectView : IView
	{
		private readonly string redirectUrl;

		public RedirectView(string redirectURL)
		{
			redirectUrl = redirectURL;
		}

		public string ResponseFragment
		{
			get { return redirectUrl; }
		}
	}
}
