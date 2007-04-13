using System.Web;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class XmlFragmentResponse : IResponse
	{
		private readonly string xmlFragment;

		public XmlFragmentResponse(string xmlFragment)
		{
			this.xmlFragment = xmlFragment;
		}

		public string ResponseFragment
		{
			get { return xmlFragment; }
		}

		public void Process(HttpResponse response)
		{
			response.ContentType = MimeType.Xml.ContentType;
			response.Write(xmlFragment);
		}
	}
}