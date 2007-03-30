using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using Objection;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.WebDashboard.Dashboard;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.ASPNET
{
	public class CacheAsDictionary : IDictionary
	{
		private readonly Cache cache;

		public CacheAsDictionary(Cache cache)
		{
			this.cache = cache;
		}
		
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return cache.GetEnumerator();
		}

		public object this[object key]
		{
			get
			{
				return cache[GetFullTypeName(key)];
			}
			set
			{
				cache[GetFullTypeName(key)] = value;
			}
		}

		private static string GetFullTypeName(object key)
		{
			Type keyType = (Type) key;
			return keyType.FullName;
		}

		public void Remove(object key)
		{
			cache.Remove(GetFullTypeName(key));
		}

		public bool Contains(object key)
		{
			throw new NotImplementedException("Contains not available on cache");
		}

		public void Clear()
		{
			throw new NotImplementedException("Clear not available on cache");			
		}

		public ICollection Values
		{
			get
			{
				throw new NotImplementedException("Values not available on cache");
			}
		}

		public void Add(object key, object value)
		{
			cache.Insert(GetFullTypeName(key), value);
		}

		public ICollection Keys
		{
			get
			{
				throw new NotImplementedException("Keys not available on cache");
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return true;
			}
		}

		public int Count
		{
			get
			{
				return cache.Count;
			}
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotImplementedException("CopyTo not available on cache");
		}

		public object SyncRoot
		{
			get
			{
				throw new NotImplementedException("SyncRoot not available on cache");
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable) cache).GetEnumerator();
		}

	}

	
	// No need for session state yet, but if we do later then we should also add IRequiresSessionState to list of interfaces
	public class HttpHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			DoSecurityChecks(context);

			// ToDo - more on MimeTypes?
			if (context.Request.Path.EndsWith(".xml"))
			{
				// ToDo - if we are specifying XML, shouldn't we throw valid XML exceptions?
				context.Response.ContentType = "Text/XML";
			}
			else
			{
				string[] splits = context.Request.Path.Split('.');
				if (MimeType.Jpg.HasExtension(splits[splits.Length - 1]))
				{
					context.Response.ContentType = MimeType.Jpg.ContentType;
				}
			}

//			LogAssembliesInAppDomain();
			ObjectionStore objectionStore = new ObjectionStore(
				new CachingImplementationResolver(new NMockAwareImplementationResolver(), new CacheAsDictionary(context.Cache)), new MaxLengthConstructorSelectionStrategy());
			ObjectSource objectSource = new CruiseObjectSourceInitializer(objectionStore).SetupObjectSourceForRequest(context);
			IResponse response = ((RequestController) objectSource.GetByType(typeof (RequestController))).Do();

			if (response is RedirectResponse)
			{
				context.Response.Redirect(((RedirectResponse) response).Url);
			}
			else if (response is BinaryResponse )
			{
				context.Response.BinaryWrite(((BinaryResponse)response).Content);
			}
			else
			{
				context.Response.Write(((HtmlFragmentResponse) response).ResponseFragment);
			}

			context.Response.Flush();
		}

//		private void LogAssembliesInAppDomain()
//		{
//			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
//			{
//				Log.Debug("Assembly in app domain: " + assembly.GetName());
//			}			
//		}
//
		public bool IsReusable
		{
			get { return true; }
		}

		private void DoSecurityChecks(HttpContext context)
		{
			// Security Fix - see http://www.microsoft.com/security/incident/aspnet.mspx
			if (context.Request.Path.IndexOf('\\') >= 0 ||
				Path.GetFullPath(context.Request.PhysicalPath) != context.Request.PhysicalPath)
			{
				throw new HttpException(404, "not found");
			}
		}
	}

	internal class MimeType
	{
		//TODO: read from config file?
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