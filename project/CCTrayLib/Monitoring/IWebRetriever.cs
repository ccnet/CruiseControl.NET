using System;
using System.IO;
using System.Net;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface IWebRetriever
	{
		string Get(Uri uri);
	}

}