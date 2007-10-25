using System;
using System.Collections.Specialized;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface IWebRetriever
	{
		string Get(Uri uri);
		void Post(Uri uri, NameValueCollection input);
	}
}
