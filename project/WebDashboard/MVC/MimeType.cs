using System.Collections;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC
{
	internal class MimeType
	{
		public static readonly MimeType Jpg = new MimeType("image/jpeg", "jpg", "jpe");
		public static readonly MimeType Png = new MimeType("image/png", "png");
		public static readonly MimeType Xml = new MimeType("text/xml", "xml");

		private ArrayList mimeExtension;
		private string mimeType;

		public MimeType(string mimeType, params string[] extensions)
		{
			mimeExtension = new ArrayList();
			mimeExtension.AddRange(extensions);
			this.mimeType = mimeType;
		}

		public bool HasExtension(string extension)
		{
			return mimeExtension.Contains(extension);
		}

		public string ContentType
		{
			get { return mimeType; }
		}
	}
}