namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class StringView : IView
	{
		private readonly string htmlFragment;

		public StringView(string htmlFragment)
		{
			this.htmlFragment = htmlFragment;
		}

		public string ResponseFragment
		{
			get { return htmlFragment; }
		}
	}
}
