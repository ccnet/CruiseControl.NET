using System.Web;
using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	public class XmlFragmentResponse : IResponse
	{
		private readonly string xmlFragment;
	    private ConditionalGetFingerprint serverFingerprint;

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
            response.AppendHeader("Last-Modified", serverFingerprint.LastModifiedTime.ToString("r"));
            response.AppendHeader("ETag", serverFingerprint.ETag);
            response.AppendHeader("Cache-Control", "private, max-age=0");
            response.ContentType = MimeType.Xml.ContentType;
			response.Write(xmlFragment);
		}

	    public ConditionalGetFingerprint ServerFingerprint
	    {
            get { return serverFingerprint; }
	        set { serverFingerprint = value; }
	    }
	}
}