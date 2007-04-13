using System.Web;

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

		public void Process(HttpResponse response)
		{
			response.Write(htmlFragment);
		}
	}
}