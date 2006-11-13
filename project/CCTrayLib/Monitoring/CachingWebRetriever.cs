using System;
using System.Collections;

namespace ThoughtWorks.CruiseControl.CCTrayLib.Monitoring
{
	/// <summary>
	/// CachingWebRetriever acts as a web retriever, but caches results for a configurable time span so as not to repeatedly hit the same web site continuously for the same information.
	/// </summary>
	public class CachingWebRetriever : ICachingWebRetriever
	{
		private IWebRetriever internalWebRetriever;
		private IDictionary webRetrieverValueCache;

		public CachingWebRetriever(IWebRetriever webRetrieverToUse)
		{
			if (webRetrieverToUse == null)
				throw new ArgumentNullException("webRetrieverToUse");

			this.internalWebRetriever = webRetrieverToUse;
			this.webRetrieverValueCache = Hashtable.Synchronized(new Hashtable());
		}

		public string Get(Uri uri)
		{
			string retrievedWebValue = null;

			if (webRetrieverValueCache.Contains(uri))
			{
				retrievedWebValue = (string)webRetrieverValueCache[uri];
			}
			else
			{
				retrievedWebValue = internalWebRetriever.Get(uri);
				webRetrieverValueCache[uri] = retrievedWebValue;
			}

			return retrievedWebValue;
		}

		public void Clear()
		{
			webRetrieverValueCache.Clear();
		}
	}
}
