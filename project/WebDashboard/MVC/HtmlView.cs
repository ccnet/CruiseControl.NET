namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class HtmlView : IView
	{
		private readonly string htmlFragment;

		public HtmlView(string htmlFragment)
		{
			this.htmlFragment = htmlFragment;
		}

		public string HtmlFragment
		{
			get { return htmlFragment; }
		}
	}
}
