using System;
using System.Collections;
using System.Web.Caching;
using Objection;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET
{
	public class CachedTypeMap : TypeToTypeMap
	{
		private readonly Cache cache;
		private readonly string cacheKey;

		public CachedTypeMap(Cache cache, string cacheKey)
		{
			this.cache = cache;
			this.cacheKey = cacheKey;
		}

		public Type this[Type baseType]
		{
			get
			{
				IDictionary cachedDictionary = GetCachedDictionary();
				return (Type) cachedDictionary[baseType];
			}
			set 
			{
				IDictionary cachedDictionary = GetCachedDictionary();
				cachedDictionary[baseType] = value;
			}
		}

		private IDictionary GetCachedDictionary()
		{
			if(cache[cacheKey] == null)
			{
				cache[cacheKey] = Hashtable.Synchronized(new Hashtable());
			}
			return (IDictionary) cache[cacheKey];
		}
	}
}