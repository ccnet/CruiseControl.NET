namespace ThoughtWorks.CruiseControl.WebDashboard.Plugins.LogViewerPlugin
{
	public class LogViewerResults
	{
		private readonly string redirectUrl;

		public LogViewerResults(string RedirectURL)
		{
			redirectUrl = RedirectURL;
		}

		public string RedirectURL
		{
			get { return redirectUrl; }
		}
	}
}
