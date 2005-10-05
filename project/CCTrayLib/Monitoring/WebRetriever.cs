using System;
using System.IO;
using System.Net;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public class WebRetriever : IWebRetriever
	{
		public string Get(Uri uri)
		{
			WebRequest request = WebRequest.Create(uri);
			using (WebResponse response = request.GetResponse())
			using (Stream responseStream = response.GetResponseStream())
			{
				StreamReader streamReader = new StreamReader(responseStream);
				return streamReader.ReadToEnd();
			}
		}
	}
}