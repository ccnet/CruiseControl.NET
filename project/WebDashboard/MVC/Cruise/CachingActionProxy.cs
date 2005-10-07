using ThoughtWorks.CruiseControl.WebDashboard.IO;

namespace ThoughtWorks.CruiseControl.WebDashboard.MVC.Cruise
{
	public class CachingActionProxy : IAction
	{
		private readonly IAction proxiedAction;
		private readonly IResponseCache cache;

		public CachingActionProxy(IAction proxiedAction, IResponseCache cache)
		{
			this.proxiedAction = proxiedAction;
			this.cache = cache;
		}

		public IResponse Execute(IRequest request)
		{
			IResponse cachedResponse = cache.Get(request);
			if (cachedResponse != null)
				return cachedResponse;

			IResponse response = proxiedAction.Execute(request);
			cache.Insert(request, response);
			return response;
		}
	}
}
