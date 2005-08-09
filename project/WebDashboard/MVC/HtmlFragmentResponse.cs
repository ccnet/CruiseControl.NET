namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class HtmlFragmentResponse : IResponse
	{
		private readonly string htmlFragment;

		public HtmlFragmentResponse(string htmlFragment)
		{
			this.htmlFragment = htmlFragment;
		}

		public string ResponseFragment
		{
			get { return htmlFragment; }
		}
	}
}
