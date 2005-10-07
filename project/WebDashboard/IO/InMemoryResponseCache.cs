using System.Web;
using System.Web.Caching;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class InMemoryResponseCache : IResponseCache
	{		
		private static readonly Cache cache = HttpRuntime.Cache;

		public IResponse Get(IRequest request)
		{
			return (IResponse) cache.Get(request.RawUrl);
		}

		public void Insert(IRequest request, IResponse response)
		{
			cache.Insert(request.RawUrl, response);
		}
	}
}
using System.Web;
using System.Web.Caching;
using ThoughtWorks.CruiseControl.WebDashboard.MVC;

namespace ThoughtWorks.CruiseControl.WebDashboard.IO
{
	public class InMemoryResponseCache : IResponseCache
	{		
		private static readonly Cache cache = HttpRuntime.Cache;

		public IResponse Get(IRequest request)
		{
			return (IResponse) cache.Get(request.RawUrl);
		}

		public void Insert(IRequest request, IResponse response)
		{
			cache.Insert(request.RawUrl, response);
		}
	}
}