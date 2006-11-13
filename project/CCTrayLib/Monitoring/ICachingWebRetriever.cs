using System;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// Interface definition for a caching web retriever.
	/// </summary>
	public interface ICachingWebRetriever : IWebRetriever, ICache
	{
	}
}
