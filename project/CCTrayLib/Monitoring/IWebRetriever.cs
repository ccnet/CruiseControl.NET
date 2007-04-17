using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	public interface IWebRetriever
	{
		string Get(Uri uri);
	}
}