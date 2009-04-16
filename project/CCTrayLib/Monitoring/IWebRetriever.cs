using System;
using System.Collections.Specialized;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface IWebRetriever
	{
		string Get(Uri uri);
		string Post(Uri uri, NameValueCollection input);
	}
}
